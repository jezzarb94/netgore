using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using log4net;
using Microsoft.Xna.Framework;
using Platyform;
using Platyform.Extensions;

// FUTURE: Improve how characters handle when they hit the map's borders

namespace DemoGame
{
    /// <summary>
    /// Base map class
    /// </summary>
    public abstract class MapBase : IMap
    {
        /// <summary>
        /// The suffix used for map files. Does not include the period prefix.
        /// </summary>
        public const string MapFileSuffix = "xml";

        /// <summary>
        /// If the delta time for updating exceeds this value, it will
        /// be reduced to this value to prevent errors at the cost of
        /// potential visual artifacts.
        /// </summary>
        protected const float MaxUpdateDeltaTime = 35f; // ~30 FPS

        /// <summary>
        /// Rate in milliseconds at which server and client 
        /// synchronized physics (ie character movement) is updated. A
        /// higher update rate results in smoother and more accurate
        /// physics but much more strain on the server. Recommended
        /// to keep between 30 and 60 FPS.
        /// </summary>
        protected const float PhysicsUpdateRate = 1000f / 60f; // 60 FPS

        /// <summary>
        /// Size of each segment of the wall grid in pixels (smallest requires more
        /// memory but often less checks (to an extent))
        /// </summary>
        protected const int WallGridSize = 128;

        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Stack used for CheckCollisions(). Because entity updating can result in movement, which in
        /// turn can result in the entity grid being updated, enumerating over the grid directly won't work.
        /// To fix this problem, this stack provides a medium for enumerating through entities. It also
        /// allows us to easily check for and avoid duplicate entity collision tests.
        /// </summary>
        readonly Stack<Entity> _cdStack = new Stack<Entity>();

        /// <summary>
        /// Array of characters on the map
        /// </summary>
        readonly DArray<CharacterEntity> _characters = new DArray<CharacterEntity>(true);

        /// <summary>
        /// List of entities in the map
        /// </summary>
        readonly List<Entity> _entities = new List<Entity>();

        /// <summary>
        /// Lock used for updating entities that have moved in the entity grid
        /// </summary>
        readonly object _entityGridLock = new object();

        /// <summary>
        /// Interface used to get the time
        /// </summary>
        readonly IGetTime _getTime;

        /// <summary>
        /// Array of items on the map
        /// </summary>
        readonly DArray<ItemEntityBase> _items = new DArray<ItemEntityBase>(true);

        /// <summary>
        /// Index of the map
        /// </summary>
        readonly ushort _mapIndex;

        /// <summary>
        /// StopWatch used to update the map
        /// </summary>
        readonly Stopwatch _updateStopWatch = new Stopwatch();

        /// <summary>
        /// Two-dimensional grid of references to entities in that sector
        /// </summary>
        List<Entity>[,] _entityGrid;

        /// <summary>
        /// Height of the map in pixels
        /// </summary>
        float _height = float.MinValue;

        /// <summary>
        /// If the map is actively updating (set to false to "pause" the physics)
        /// </summary>
        bool _isUpdating = true;

        /// <summary>
        /// Time the map was last updated
        /// </summary>
        long _lastUpdateTime;

        /// <summary>
        /// Name of the map
        /// </summary>
        string _name = null;

        /// <summary>
        /// Width of the map in pixels
        /// </summary>
        float _width = float.MinValue;

        /// <summary>
        /// Gets an enumerator for all the characters on the map
        /// </summary>
        public IEnumerable<CharacterEntity> Characters
        {
            get { return _characters; }
        }

        /// <summary>
        /// Gets the index of the map.
        /// </summary>
        public ushort Index
        {
            get { return _mapIndex; }
        }

        /// <summary>
        /// Gets or sets if the map is updating every frame
        /// </summary>
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set
            {
                if (_isUpdating != value)
                {
                    _isUpdating = value;
                    if (_isUpdating)
                        _updateStopWatch.Start();
                    else
                        _updateStopWatch.Stop();
                }
            }
        }

        /// <summary>
        /// Gets an enumerator for all the items on the map.
        /// </summary>
        public IEnumerable<ItemEntityBase> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the name of the map.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the size of the map in pixels.
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(_width, _height); }
        }

        /// <summary>
        /// MapBase constructor
        /// </summary>
        /// <param name="mapIndex">Index of the map</param>
        /// <param name="getTime">Interface used to get the time</param>
        protected MapBase(ushort mapIndex, IGetTime getTime)
        {
            if (getTime == null)
                throw new ArgumentNullException("getTime");

            _getTime = getTime;
            _mapIndex = mapIndex;
            _updateStopWatch.Start();
        }

        /// <summary>
        /// Adds a character to the map.
        /// </summary>
        /// <param name="charEntity">CharacterEntity to add to the map.</param>
        /// <param name="mapCharIndex">Map character index to add the character at.</param>
        /// <exception cref="ArgumentException">Specified <paramref name="mapCharIndex"/> is already in use.</exception>
        public void AddCharacter(CharacterEntity charEntity, int mapCharIndex)
        {
            if (charEntity == null)
                throw new ArgumentNullException("charEntity");
            if (mapCharIndex < 0)
                throw new ArgumentOutOfRangeException("mapCharIndex");

            // Ensure the index is not already in use
            if (_characters.CanGet(mapCharIndex) && _characters[mapCharIndex] != null)
            {
                const string errmsg = "Specified mapCharIndex `{0}` [{1}] already in use by `{2}";
                throw new ArgumentException(string.Format(errmsg, mapCharIndex, charEntity, _characters[mapCharIndex]));
            }

            // Add the character to the characters list
            _characters[mapCharIndex] = charEntity;

            // Finish adding the Entity
            AddEntityFinish(charEntity);
        }

        /// <summary>
        /// Adds an entity to the map
        /// </summary>
        /// <param name="entity">Entity to add to the map</param>
        public void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            // For everything that is index-bound, assign the index
            CharacterEntity charEntity;
            ItemEntityBase itemEntity;

            if ((charEntity = entity as CharacterEntity) != null)
            {
                Debug.Assert(!_characters.Contains(charEntity), "Character is already in the Characters list!");
                charEntity.MapCharIndex = (ushort)_characters.Insert(charEntity);
            }
            else if ((itemEntity = entity as ItemEntityBase) != null)
            {
                Debug.Assert(!_items.Contains(itemEntity), "Item is already in the Items list!");
                itemEntity.MapItemIndex = (ushort)_items.Insert(itemEntity);
            }

            // Finish adding the Entity
            AddEntityFinish(entity);
        }

        void AddEntityEventHooks(Entity entity)
        {
            // Listen for when the entity is disposed so we can remove it
            entity.OnDispose += Entity_OnDispose;

            // Listen for movement from the entity so we can update their position in the grid
            entity.OnMove += Entity_OnMove;
        }

        void AddEntityFinish(Entity entity)
        {
            AddEntityToEntityList(entity);
            ForceEntityInMapBoundaries(entity);
        }

        /// <summary>
        /// Adds an entity to the entity list, which in turn adds the entity to the entity grid
        /// and creates the hooks to the entity's events.
        /// </summary>
        /// <param name="entity">Entity to add to the entity list</param>
        void AddEntityToEntityList(Entity entity)
        {
            // When in debug build, ensure we do not add an entity that is already added
            Debug.Assert(!_entities.Contains(entity), "entity already in the map's entity list!");

            // Add to the one-dimensional entity list
            _entities.Add(entity);

            // Also add the entity to the grid if it exists
            lock (_entityGridLock)
                AddEntityToGrid(entity);

            // Add the event hooks
            AddEntityEventHooks(entity);

            // Allow for additional processing
            EntityAdded(entity);
        }

        /// <summary>
        /// Adds an entity to an entity grid
        /// </summary>
        /// <param name="entity">Entity to add to the grid</param>
        void AddEntityToGrid(Entity entity)
        {
            int minX = (int)entity.CB.Min.X / WallGridSize;
            int minY = (int)entity.CB.Min.Y / WallGridSize;
            int maxX = (int)entity.CB.Max.X / WallGridSize;
            int maxY = (int)entity.CB.Max.Y / WallGridSize;

            // Keep in range of the grid
            if (minX < 0)
                minX = 0;
            if (maxX > _entityGrid.GetLength(0) - 1)
                maxX = _entityGrid.GetLength(0) - 1;
            if (minY < 0)
                minY = 0;
            if (maxY > _entityGrid.GetLength(1) - 1)
                maxY = _entityGrid.GetLength(1) - 1;

            // Add to all the segments of the grid
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!_entityGrid[x, y].Contains(entity))
                        _entityGrid[x, y].Add(entity);
                    else
                    {
                        // Debug.Fail(".Contains() should return false.");
                    }
                }
            }
        }

        /// <summary>
        /// Builds a two-dimensional array of Lists to use as the grid of entities.
        /// </summary>
        /// <returns>A two-dimensional array of Lists to use as the grid of entities.</returns>
        static List<Entity>[,] BuildEntityGrid(float width, float height)
        {
            int gridWidth = (int)Math.Ceiling(width / WallGridSize);
            int gridHeight = (int)Math.Ceiling(height / WallGridSize);

            // Create the array
            var retGrid = new List<Entity>[gridWidth,gridHeight];

            // Create the lists
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    retGrid[x, y] = new List<Entity>(32);
                }
            }

            return retGrid;
        }

        /// <summary>
        /// If the given point is in the map's boundaries
        /// </summary>
        /// <param name="p">Point to check</param>
        /// <returns>True if the point is in the map's boundaries, else false</returns>
        public bool Contains(Vector2 p)
        {
            return p.X >= 0 && p.Y >= 0 && p.X <= Width && p.Y <= Height;
        }

        /// <summary>
        /// If the given Rectangle is in the map's boundaries
        /// </summary>
        /// <param name="rect">Rectangle to check</param>
        /// <returns>True if the Rectangle is in the map's boundaries, else false</returns>
        public bool Contains(Rectangle rect)
        {
            return rect.X >= 0 && rect.Y >= 0 & rect.Right <= Width && rect.Bottom <= Height;
        }

        /// <summary>
        /// If the given CollisionBox is in the map's boundaries
        /// </summary>
        /// <param name="cb">CollisionBox to check</param>
        /// <returns>True if the CollisionBox is in the map's boundaries, else false</returns>
        public bool Contains(CollisionBox cb)
        {
            return Contains(cb.ToRectangle());
        }

        protected abstract TeleportEntityBase CreateTeleportEntity(XmlReader r);

        /// <summary>
        /// Handles when an Entity is disposed while still on the map.
        /// </summary>
        /// <param name="entity"></param>
        void Entity_OnDispose(Entity entity)
        {
            RemoveEntity(entity);
        }

        /// <summary>
        /// Handles when an entity belong to this map moves. Only needed for entities that can move,
        /// but no harm in being safe and adding to every entity. Failure to assign to an entity that
        /// moves will result in glitches in the collision detection.
        /// </summary>
        /// <param name="entity">Entity that moved</param>
        /// <param name="oldPos">Position the entity was at before moving</param>
        void Entity_OnMove(Entity entity, Vector2 oldPos)
        {
            UpdateEntityGrid(entity, oldPos);
            ForceEntityInMapBoundaries(entity);
        }

        /// <summary>
        /// Allows for additional processing for entities added to the map
        /// </summary>
        /// <param name="entity">Entity added to the map</param>
        protected virtual void EntityAdded(Entity entity)
        {
        }

        /// <summary>
        /// Allows for additional processing for entities removed from the map
        /// </summary>
        /// <param name="entity">Entity removed from the map</param>
        protected virtual void EntityRemoved(Entity entity)
        {
        }

        /// <summary>
        /// Checks if an Entity is in the map's boundaries and, if it is not, moves the Entity into the map's boundaries.
        /// </summary>
        /// <param name="entity">Entity to check.</param>
        void ForceEntityInMapBoundaries(Entity entity)
        {
            Vector2 min = entity.CB.Min;
            Vector2 max = entity.CB.Max;

            if (min.X < 0)
                min.X = 0;
            if (min.Y < 0)
                min.Y = 0;
            if (max.X >= Width)
                min.X = Width - entity.CB.Width;
            if (max.Y >= Height)
                min.Y = Height - entity.CB.Height;

            if (min != entity.CB.Min)
                entity.Teleport(min);
        }

        /// <summary>
        /// Retrieves a character on the map
        /// </summary>
        /// <param name="index">Index of the character</param>
        /// <returns>Character with the given index, or null if invalid</returns>
        public CharacterEntity GetCharacter(int index)
        {
            Debug.Assert(index >= 0, "Character index must be greater than or equal to zero.");
            Debug.Assert(index < _characters.Length, "Character index greater than Characters list length.");
            //Debug.Assert(_characters.CanGet(index), "Invalid character index.");
            //Debug.Assert(_characters[index] != null, "Specified character is null.");

            if (_characters.CanGet(index))
                return _characters[index];

            return null;
        }

        /// <summary>
        /// Gets the first CharacterEntity that intersects the specified <paramref name="rect"/>
        /// </summary>
        /// <param name="rect">Rectangle to look for the CharacterEntity in</param>
        /// <returns>First CharacterEntity found in the specified <paramref name="rect"/>, or null if none</returns>
        public CharacterEntity GetCharacter(Rectangle rect)
        {
            return GetEntity<CharacterEntity>(rect);
        }

        /// <summary>
        /// Gets all the CharacterEntities that intersects a specified area
        /// </summary>
        /// <param name="cb">CollisionBox containing the area to check</param>
        /// <returns>A list containing all CharacterEntities that intersect the specified area</returns>
        public List<CharacterEntity> GetCharacters(CollisionBox cb)
        {
            if (cb == null)
            {
                Debug.Fail("Parameter cb is null.");
                return new List<CharacterEntity>(0);
            }

            return GetCharacters(cb.ToRectangle());
        }

        /// <summary>
        /// Gets all the characters that intersect a specified area
        /// </summary>
        /// <param name="rect">Rectangle of the area to check</param>
        /// <returns>A list containing all CharacterEntities that intersect the specified area</returns>
        public List<CharacterEntity> GetCharacters(Rectangle rect)
        {
            return GetEntities<CharacterEntity>(rect);
        }

        /// <summary>
        /// Gets all the characters that intersect a specified area
        /// </summary>
        /// <param name="min">Min point of the collision area</param>
        /// <param name="max">Max point of the collision area</param>
        /// <returns>A list containing all CharacterEntities that intersect the specified area</returns>
        public List<CharacterEntity> GetCharacters(Vector2 min, Vector2 max)
        {
            Vector2 size = max - min;
            return GetCharacters(new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y));
        }

        /// <summary>
        /// Gets a list of all entities containing a given point
        /// </summary>
        /// <param name="p">Point to find the entities at</param>
        /// <returns>All entities containing the given point</returns>
        public List<Entity> GetEntities(Vector2 p)
        {
            // Get and validate the grid segment
            var gridSegment = GetEntityGrid(p);
            if (gridSegment == null)
                return new List<Entity>(0);

            var ret = new List<Entity>(gridSegment.Count);

            // Iterate through all entities and return those who contain the segment
            foreach (Entity entity in gridSegment)
            {
                // Hit test
                if (!entity.CB.HitTest(p))
                    continue;

                // Duplicates check
                if (ret.Contains(entity))
                    continue;

                ret.Add(entity);
            }

            return ret;
        }

        /// <summary>
        /// Gets a list of all entities containing a given point
        /// </summary>
        /// <param name="p">Point to find the entities at</param>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <returns>All entities containing the given point</returns>
        public List<T> GetEntities<T>(Vector2 p) where T : Entity
        {
            // Get and validate the grid segment
            var gridSegment = GetEntityGrid(p);
            if (gridSegment == null)
                return new List<T>(0);

            var ret = new List<T>(gridSegment.Count);

            // Iterate through all entities and return those who contain the segment
            foreach (Entity entity in gridSegment)
            {
                // Type cast check
                T entityT = entity as T;
                if (entityT == null)
                    continue;

                // Hit test
                if (!entityT.CB.HitTest(p))
                    continue;

                // Duplicates check
                if (ret.Contains(entityT))
                    continue;

                ret.Add(entityT);
            }

            return ret;
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region
        /// </summary>
        /// <param name="rect">Region to check for Entities</param>
        /// <typeparam name="T">Type of Entity to convert to</typeparam>
        /// <returns>List of all Entities found intersecting the given region</returns>
        public List<T> GetEntities<T>(Rectangle rect) where T : Entity
        {
            var ret = new List<T>(16);

            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the grid segment
                foreach (Entity entity in gridSegment)
                {
                    // Type cast check
                    T entityT = entity as T;
                    if (entityT == null)
                        continue;

                    // Intersection check
                    if (!entityT.CB.Intersect(rect))
                        continue;

                    // Duplicates check
                    if (ret.Contains(entityT))
                        continue;

                    ret.Add(entityT);
                }
            }

            // Return the results
            return ret;
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region
        /// </summary>
        /// <param name="cb">CollisionBox to check for Entities in</param>
        /// <param name="condition">Condition the Entities must meet</param>
        /// <returns>List of all Entities found intersecting the given region</returns>
        public List<Entity> GetEntities(CollisionBox cb, Predicate<Entity> condition)
        {
            if (cb == null)
            {
                Debug.Fail("Parameter cb is null.");
                return new List<Entity>(0);
            }

            return GetEntities(cb.ToRectangle(), condition);
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region
        /// </summary>
        /// <param name="cb">Region to check for Entities in</param>
        /// <param name="condition">Condition the Entities must meet</param>
        /// <typeparam name="T">Type of Entity to convert to</typeparam>
        /// <returns>List of all Entities found intersecting the given region</returns>
        public List<T> GetEntities<T>(CollisionBox cb, Predicate<T> condition) where T : Entity
        {
            if (cb == null)
            {
                Debug.Fail("Parameter cb is null.");
                return new List<T>(0);
            }

            return GetEntities(cb.ToRectangle(), condition);
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region
        /// </summary>
        /// <param name="rect">Region to check for Entities</param>
        /// <param name="condition">Condition the Entities must meet</param>
        /// <typeparam name="T">Type of Entity to convert to</typeparam>
        /// <returns>List of all Entities found intersecting the given region</returns>
        public List<T> GetEntities<T>(Rectangle rect, Predicate<T> condition) where T : Entity
        {
            var ret = new List<T>(16);

            // If condition is null, don't use it
            if (condition == null)
                return GetEntities<T>(rect);

            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the grid segment
                foreach (Entity entity in gridSegment)
                {
                    // Type cast check
                    T entityT = entity as T;
                    if (entityT == null)
                        continue;

                    // Intersection check
                    if (!entityT.CB.Intersect(rect))
                        continue;

                    // Duplicates check
                    if (ret.Contains(entityT))
                        continue;

                    // Condition check
                    if (!condition(entityT))
                        continue;

                    ret.Add(entityT);
                }
            }

            // Return the results
            return ret;
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region
        /// </summary>
        /// <param name="rect">Region to check for Entities</param>
        /// <param name="condition">Condition the Entities must meet</param>
        /// <returns>List of all Entities found intersecting the given region</returns>
        public List<Entity> GetEntities(Rectangle rect, Predicate<Entity> condition)
        {
            var ret = new List<Entity>(16);

            // If condition is null, don't use it
            if (condition == null)
                return GetEntities(rect);

            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the grid segment
                foreach (Entity entity in gridSegment)
                {
                    // Intersection check
                    if (!entity.CB.Intersect(rect))
                        continue;

                    // Duplicates check
                    if (ret.Contains(entity))
                        continue;

                    // Condition check
                    if (!condition(entity))
                        continue;

                    ret.Add(entity);
                }
            }

            // Return the results
            return ret;
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region
        /// </summary>
        /// <param name="rect">Region to check for Entities</param>
        /// <returns>List of all Entities found intersecting the given region</returns>
        public List<Entity> GetEntities(Rectangle rect)
        {
            var ret = new List<Entity>(16);

            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the grid segment
                foreach (Entity entity in gridSegment)
                {
                    // Intersection check
                    if (!entity.CB.Intersect(rect))
                        continue;

                    // Duplicates check
                    if (ret.Contains(entity))
                        continue;

                    ret.Add(entity);
                }
            }

            // Return the results
            return ret;
        }

        /// <summary>
        /// Gets the first Entity found in the given region
        /// </summary>
        /// <param name="rect">Region to check for the Entity</param>
        /// <param name="condition">Condition the Entity must meet</param>
        /// <returns>First Entity found at the given point, or null if none found</returns>
        public Entity GetEntity(Rectangle rect, Predicate<Entity> condition)
        {
            // If condition is null, don't use it
            if (condition == null)
                return GetEntity(rect);

            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the grid segment
                foreach (Entity entity in gridSegment)
                {
                    // Intersection check
                    if (!entity.CB.Intersect(rect))
                        continue;

                    // Condition check
                    if (!condition(entity))
                        continue;

                    return entity;
                }
            }

            // No entity found
            return null;
        }

        /// <summary>
        /// Gets the first Entity found in the given region
        /// </summary>
        /// <param name="rect">Region to check for the Entity</param>
        /// <returns>First Entity found at the given point, or null if none found</returns>
        public Entity GetEntity(Rectangle rect)
        {
            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the grid segment
                foreach (Entity entity in gridSegment)
                {
                    // Intersection check
                    if (!entity.CB.Intersect(rect))
                        continue;

                    return entity;
                }
            }

            // No entity found
            return null;
        }

        /// <summary>
        /// Gets the first Entity found in the given region
        /// </summary>
        /// <param name="rect">Region to check for the Entity</param>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <returns>First Entity found at the given point, or null if none found</returns>
        public T GetEntity<T>(Rectangle rect) where T : Entity
        {
            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                // Iterate through each entity in the segment
                foreach (Entity entity in gridSegment)
                {
                    // Type cast check
                    T entityT = entity as T;
                    if (entityT == null)
                        continue;

                    // Intersection check
                    if (!entity.CB.Intersect(rect))
                        continue;

                    return entityT;
                }
            }

            // No entity found
            return null;
        }

        /// <summary>
        /// Gets the first Entity found in the given region
        /// </summary>
        /// <param name="rect">Region to check for the Entity</param>
        /// <param name="condition">Condition the entity must meet</param>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <returns>First Entity found at the given point, or null if none found</returns>
        public T GetEntity<T>(Rectangle rect, Func<T, bool> condition) where T : Entity
        {
            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(rect))
            {
                var entities = gridSegment.OfType<T>().Where(entity => entity.CB.Intersect(rect));

                if (condition != null)
                    entities = entities.Where(condition);

                if (entities.Count() > 0)
                    return entities.First();
            }

            // No entity found
            return null;
        }

        /// <summary>
        /// Gets the first Entity found at the given point
        /// </summary>
        /// <param name="p">Point to find the entity at</param>
        /// <param name="condition">Condition the entity must meet</param>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <returns>First entity found at the given point, or null if none found</returns>
        public T GetEntity<T>(Vector2 p, Predicate<T> condition) where T : Entity
        {
            // Get and validate the grid segment
            var gridSegment = GetEntityGrid(p);
            if (gridSegment == null)
                return null;

            // If condition is null, don't use it
            if (condition == null)
                return GetEntity<T>(p);

            // Iterate through all entities and return the first one to contain the segment
            foreach (Entity entity in gridSegment)
            {
                // Type cast check
                T entityT = entity as T;
                if (entityT == null)
                    continue;

                // Hit test check
                if (!entityT.CB.HitTest(p))
                    continue;

                // Condition check
                if (!condition(entityT))
                    continue;

                return entityT;
            }

            // None found
            return null;
        }

        /// <summary>
        /// Gets the first Entity found at the given point
        /// </summary>
        /// <param name="p">Point to find the entity at</param>
        /// <param name="condition">Condition the entity must meet</param>
        /// <returns>First entity found at the given point, or null if none found</returns>
        public Entity GetEntity(Vector2 p, Predicate<Entity> condition)
        {
            // Get and validate the grid segment
            var gridSegment = GetEntityGrid(p);
            if (gridSegment == null)
                return null;

            // If condition is null, don't use it
            if (condition == null)
                return GetEntity(p);

            // Iterate through all entities and return the first one to contain the segment
            foreach (Entity entity in gridSegment)
            {
                // Hit test check
                if (!entity.CB.HitTest(p))
                    continue;

                // Condition check
                if (!condition(entity))
                    continue;

                return entity;
            }

            // None found
            return null;
        }

        /// <summary>
        /// Gets the first Entity found at the given point
        /// </summary>
        /// <param name="p">Point to find the entity at</param>
        /// <returns>First entity found at the given point, or null if none found</returns>
        /// <typeparam name="T">Type of Entity to look for</typeparam>
        public T GetEntity<T>(Vector2 p) where T : Entity
        {
            // Get and validate the grid segment
            var gridSegment = GetEntityGrid(p);
            if (gridSegment == null)
                return null;

            // Iterate through all entities and return the first one to contain the segment
            foreach (Entity entity in gridSegment)
            {
                // Type cast check
                T entityT = entity as T;
                if (entityT == null)
                    continue;

                // Hit test check
                if (!entity.CB.HitTest(p))
                    continue;

                return entityT;
            }

            // None found
            return null;
        }

        /// <summary>
        /// Gets an entity at the given point
        /// </summary>
        /// <param name="p">Point to find the entity at</param>
        /// <returns>First entity found at the given point, or null if none found</returns>
        public Entity GetEntity(Vector2 p)
        {
            // Get and validate the grid segment
            var gridSegment = GetEntityGrid(p);
            if (gridSegment == null)
                return null;

            // Iterate through all entities and return the first one to contain the segment
            foreach (Entity entity in gridSegment)
            {
                // Hit test check
                if (!entity.CB.HitTest(p))
                    continue;

                return entity;
            }

            // None found
            return null;
        }

        /// <summary>
        /// Allows an entity to be grabbed by index. Intended only for performance cases where iterating
        /// through the entire entity set would not be optimal (such as comparing the entities to each other).
        /// </summary>
        /// <param name="index">Index of the entity to get</param>
        /// <returns>Entity for the given index, or null if invalid</returns>
        protected Entity GetEntity(int index)
        {
            // Ensure the index is valid
            if (index < 0 || index >= _entities.Count)
            {
                Debug.Fail("Invalid index.");
                return null;
            }

            // Get the entity at the index
            return _entities[index];
        }

        /// <summary>
        /// Gets the Entity grid containing the given coordinate
        /// </summary>
        /// <param name="p">Coordinate to find the grid for</param>
        /// <returns>Entity grid containing the given coordinate, or null if an invalid location</returns>
        protected List<Entity> GetEntityGrid(Vector2 p)
        {
            int x = (int)p.X / WallGridSize;
            int y = (int)p.Y / WallGridSize;

            // Validate location
            if (x < 0 || x > _entityGrid.GetLength(0) - 1 || y < 0 || y > _entityGrid.GetLength(1) - 1)
                return null;

            // Return the grid segment
            return _entityGrid[x, y];
        }

        /// <summary>
        /// Gets the grid segment for each intersection on the entity grid
        /// </summary>
        /// <param name="cb">Map area to get the grid segments for</param>
        /// <returns>List of all grid segments intersected by <paramref name="cb"/></returns>
        protected List<List<Entity>> GetEntityGrids(CollisionBox cb)
        {
            if (cb == null)
            {
                Debug.Fail("Parameter cb is null.");
                return new List<List<Entity>>(0);
            }

            return GetEntityGrids(cb.ToRectangle());
        }

        /// <summary>
        /// Gets the grid segment for each intersection on the entity grid
        /// </summary>
        /// <param name="rect">Map area to get the grid segments for</param>
        /// <returns>List of all grid segments intersected by <paramref name="rect"/></returns>
        protected List<List<Entity>> GetEntityGrids(Rectangle rect)
        {
            int minX = rect.X / WallGridSize;
            int minY = rect.Y / WallGridSize;
            int maxX = rect.Right / WallGridSize;
            int maxY = rect.Bottom / WallGridSize;

            // Keep in range of the grid
            if (minX < 0)
            {
                Debug.Fail("Invalid entity position.");
                minX = 0;
            }
            if (maxX > _entityGrid.GetLength(0) - 1)
            {
                Debug.Fail("Invalid entity position.");
                maxX = _entityGrid.GetLength(0) - 1;
            }
            if (minY < 0)
            {
                Debug.Fail("Invalid entity position.");
                minY = 0;
            }
            if (maxY > _entityGrid.GetLength(1) - 1)
            {
                Debug.Fail("Invalid entity position.");
                maxY = _entityGrid.GetLength(1) - 1;
            }

            // Count the number of grid segments
            int segmentCount = (maxX - minX + 1) * (maxY - minY + 1);

            // Combine the grid segments
            var ret = new List<List<Entity>>(segmentCount);
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    ret.Add(_entityGrid[x, y]);
                }
            }

            return ret;
        }

        /// <summary>
        /// Finds the map's index from its file path
        /// </summary>
        /// <param name="path">File path to the map</param>
        /// <returns>Index of the map</returns>
        public static ushort GetIndexFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return ushort.Parse(Path.GetFileNameWithoutExtension(path));
        }

        /// <summary>
        /// Retrieves an item on the map
        /// </summary>
        /// <param name="index">Index of the item</param>
        /// <returns>Item with the given index, or null if invalid</returns>
        public ItemEntityBase GetItem(int index)
        {
            Debug.Assert(index >= 0, "Item index must be greater than or equal to zero.");
            Debug.Assert(index < _items.Length, "Item index greater than Items list length.");
            Debug.Assert(_items.CanGet(index), "Invalid item index.");
            Debug.Assert(_items[index] != null, "Specified item is null.");

            if (_items.CanGet(index))
                return _items[index];

            return null;
        }

        /// <summary>
        /// Gets the first ItemEntityBase that intersects the specified <paramref name="rect"/>
        /// </summary>
        /// <param name="rect">Rectangle to look for the ItemEntityBase in</param>
        /// <returns>First ItemEntityBase found in the specified <paramref name="rect"/>, or null if none</returns>
        public ItemEntityBase GetItem(Rectangle rect)
        {
            return GetEntity<ItemEntityBase>(rect);
        }

        /// <summary>
        /// Gets all the ItemEntityBases that intersects a specified area
        /// </summary>
        /// <param name="cb">CollisionBox containing the area to check</param>
        /// <returns>A list containing all ItemEntityBases that intersect the specified area</returns>
        public List<ItemEntityBase> GetItems(CollisionBox cb)
        {
            if (cb == null)
            {
                const string errmsg = "Parameter cb is null - falling back to returning empty list.";
                Debug.Fail(errmsg);
                if (log.IsErrorEnabled)
                    log.Error(errmsg);
                return new List<ItemEntityBase>(0);
            }

            return GetItems(cb.ToRectangle());
        }

        /// <summary>
        /// Gets all the items that intersect a specified area
        /// </summary>
        /// <param name="rect">Rectangle of the area to check</param>
        /// <returns>A list containing all ItemEntityBases that intersect the specified area</returns>
        public List<ItemEntityBase> GetItems(Rectangle rect)
        {
            return GetEntities<ItemEntityBase>(rect);
        }

        /// <summary>
        /// Gets all the ItemEntityBases that intersect a specified area
        /// </summary>
        /// <param name="min">Min point of the collision area</param>
        /// <param name="max">Max point of the collision area</param>
        /// <returns>A list containing all ItemEntityBases that intersect the specified area</returns>
        public List<ItemEntityBase> GetItems(Vector2 min, Vector2 max)
        {
            Vector2 size = max - min;
            return GetItems(new Rectangle((int)min.X, (int)min.Y, (int)size.X, (int)size.Y));
        }

        /// <summary>
        /// Gets an IEnumerable of the path of all the map files in the given ContentPaths.
        /// </summary>
        /// <param name="path">ContentPaths to load the map files from.</param>
        /// <returns>An IEnumerable of all the map files in <paramref name="path"/>.</returns>
        public static IEnumerable<string> GetMapFiles(ContentPaths path)
        {
            // Get all the files
            var allFiles = Directory.GetFiles(path.Maps);

            // Select only valid map files
            var mapFiles = allFiles.Where(IsValidMapFile);

            return mapFiles;
        }

        /// <summary>
        /// Gets the next free map index.
        /// </summary>
        /// <param name="path">ContentPaths containing the maps.</param>
        /// <returns>Next free map index.</returns>
        public static ushort GetNextFreeIndex(ContentPaths path)
        {
            var mapFiles = GetMapFiles(path);
            var indices = mapFiles.Select(x => GetIndexFromPath(x)).ToList();
            indices.Sort();

            // Check every map index starting at 1, returning the first free value found
            ushort expected = 1;
            for (int i = 0; i < indices.Count; i++)
            {
                if (indices[i] != expected)
                    return expected;

                expected++;
            }

            return expected;
        }

        /// <summary>
        /// Gets the first IPickupableEntity that intersects a specified area
        /// </summary>
        /// <param name="rect">Rectangle of the area to check</param>
        /// <returns>First IPickupableEntity that intersects the specified area, or null if none</returns>
        public IPickupableEntity GetPickupable(Rectangle rect)
        {
            return GetEntity(rect, entity => entity is IPickupableEntity) as IPickupableEntity;
        }

        /// <summary>
        /// Gets the first IPickupableEntity that intersects a specified area
        /// </summary>
        /// <param name="rect">Rectangle of the area to check</param>
        /// <param name="charEntity">CharacterEntity that must be able to pick up the IPickupableEntity</param>
        /// <returns>First IPickupableEntity that intersects the specified area that the charEntity is
        /// able to pick up, or null if none</returns>
        public IPickupableEntity GetPickupable(Rectangle rect, CharacterEntity charEntity)
        {
            // Predicate that will check if an Entity inherits interface IUseableEntity,
            // and if it can be used by the specified CharacterEntity
            Predicate<Entity> pred = delegate(Entity entity)
                                     {
                                         IPickupableEntity useable = entity as IPickupableEntity;
                                         if (useable == null)
                                             return false;

                                         return useable.CanPickup(charEntity);
                                     };

            return GetEntity(rect, pred) as IPickupableEntity;
        }

        /// <summary>
        /// Gets the first IPickupableEntity that intersects a specified area
        /// </summary>
        /// <param name="cb">CollisionBox of the area to check</param>
        /// <returns>First IUseableEntity that intersects the specified area, or null if none</returns>
        public IPickupableEntity GetPickupable(CollisionBox cb)
        {
            return GetPickupable(cb.ToRectangle());
        }

        /// <summary>
        /// Gets the first IPickupableEntity that intersects a specified area
        /// </summary>
        /// <param name="cb">CollisionBox of the area to check</param>
        /// <param name="charEntity">CharacterEntity that must be able to pick up the IPickupableEntity</param>
        /// <returns>First IPickupableEntity that intersects the specified area that the charEntity is
        /// able to pick up, or null if none</returns>
        public IPickupableEntity GetPickupable(CollisionBox cb, CharacterEntity charEntity)
        {
            return GetPickupable(cb.ToRectangle(), charEntity);
        }

        /// <summary>
        /// Gets the first IUseableEntity that intersects a specified area
        /// </summary>
        /// <param name="rect">Rectangle of the area to check</param>
        /// <returns>First IUseableEntity that intersects the specified area, or null if none</returns>
        public IUseableEntity GetUseable(Rectangle rect)
        {
            return GetEntity(rect, entity => entity is IUseableEntity) as IUseableEntity;
        }

        /// <summary>
        /// Gets the first IUseableEntity that intersects a specified area
        /// </summary>
        /// <param name="rect">Rectangle of the area to check</param>
        /// <param name="charEntity">CharacterEntity that must be able to use the IUseableEntity</param>
        /// <returns>First IUseableEntity that intersects the specified area that the charEntity
        /// is able to use, or null if none</returns>
        public IUseableEntity GetUseable(Rectangle rect, CharacterEntity charEntity)
        {
            // Predicate that will check if an Entity inherits interface IUseableEntity,
            // and if it can be used by the specified CharacterEntity
            Predicate<Entity> pred = delegate(Entity entity)
                                     {
                                         IUseableEntity useable = entity as IUseableEntity;
                                         if (useable == null)
                                             return false;

                                         return useable.CanUse(charEntity);
                                     };

            return GetEntity(rect, pred) as IUseableEntity;
        }

        /// <summary>
        /// Gets the first IUseableEntity that intersects a specified area
        /// </summary>
        /// <param name="cb">CollisionBox of the area to check</param>
        /// <returns>First IUseableEntity that intersects the specified area, or null if none</returns>
        public IUseableEntity GetUseable(CollisionBox cb)
        {
            return GetUseable(cb.ToRectangle());
        }

        /// <summary>
        /// Gets the first IUseableEntity that intersects a specified area
        /// </summary>
        /// <param name="cb">CollisionBox of the area to check</param>
        /// <param name="charEntity">CharacterEntity that must be able to use the IUseableEntity</param>
        /// <returns>First IUseableEntity that intersects the specified area that the charEntity
        /// is able to use, or null if none</returns>
        public IUseableEntity GetUseable(CollisionBox cb, CharacterEntity charEntity)
        {
            return GetUseable(cb.ToRectangle(), charEntity);
        }

        /// <summary>
        /// Gets a wall matching specified conditions
        /// </summary>
        /// <param name="p">Point on the map contained in the wall</param>
        /// <returns>First wall meeting the condition, null if none found</returns>
        public WallEntity GetWall(Vector2 p)
        {
            return GetEntity<WallEntity>(p);
        }

        /// <summary>
        /// Returns a list of all the walls in a given area
        /// </summary>
        /// <param name="cb">Collision box area to find walls from</param>
        /// <returns>List of walls in the area</returns>
        public List<WallEntity> GetWalls(CollisionBox cb)
        {
            if (cb == null)
            {
                const string errmsg = "Parameter cb is null - falling back to returning empty list.";
                Debug.Fail(errmsg);
                if (log.IsErrorEnabled)
                    log.Error(errmsg);
                return new List<WallEntity>(0);
            }

            return GetEntities<WallEntity>(cb.ToRectangle());
        }

        public bool IsInMapBoundaries(Vector2 point)
        {
            return (point.X >= 0) && (point.Y >= 0) && (point.X < Width) && (point.Y < Height);
        }

        /// <summary>
        /// Checks if a given file is a valid map file by taking a quick look at the file, but not
        /// actually loading it. Files that are validated by this can still fail to load if they
        /// are corrupt.
        /// </summary>
        /// <param name="filePath">Path to the file to check</param>
        /// <returns>True if the file is a valid map file, else false</returns>
        public static bool IsValidMapFile(string filePath)
        {
            // Check if the file exists
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            // Check the suffix
            if (!filePath.ToLower().EndsWith("." + MapFileSuffix))
                return false;

            // Check if the file is named properly
            ushort index;
            if (!ushort.TryParse(Path.GetFileNameWithoutExtension(filePath), out index))
                return false;

            // Validate the file index
            if (index < 1)
                return false;

            // All checks passed
            return true;
        }

        /// <summary>
        /// Confirms that an entity being moved by a given offset will
        /// keep the offset in the map boundaries, modifying the offset
        /// if needed to stay in the map.
        /// </summary>
        /// <param name="entity">Entity to be moved.</param>
        /// <param name="offset">Amount to add the entity's position.</param>
        public void KeepInMap(Entity entity, ref Vector2 offset)
        {
            if (entity == null)
            {
                const string errmsg = "Parameter `entity` is null.";
                Debug.Fail(errmsg);
                if (log.IsErrorEnabled)
                    log.Error(errmsg);
                return;
            }

            if (entity.CB.Min.X + offset.X < 0)
                offset.X = -entity.CB.Min.X;
            else if (entity.CB.Max.X + offset.X > Width)
                offset.X = entity.CB.Max.X - Width;

            if (entity.CB.Min.Y + offset.Y < 0)
                offset.Y = -entity.CB.Min.Y;
            else if (entity.CB.Max.Y + offset.Y > Height)
                offset.Y = entity.CB.Max.Y - Height;
        }

        /// <summary>
        /// Loads the map from the specified content path.
        /// </summary>
        /// <param name="contentPath">ContentPath to load the map from.</param>
        public void Load(ContentPaths contentPath)
        {
            string path = contentPath.Maps.Join(Index + "." + MapFileSuffix);
            Load(path);
        }

        /// <summary>
        /// Loads the map.
        /// </summary>
        /// <param name="filePath">Path to the file to load</param>
        protected virtual void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                const string errmsg = "Map file `{0}` does not exist - unable to load map.";
                Debug.Fail(string.Format(errmsg, filePath));
                log.ErrorFormat(errmsg, filePath);
                throw new ArgumentException("filePath");
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                using (XmlReader r = XmlReader.Create(stream))
                {
                    while (r.Read())
                    {
                        if (r.NodeType != XmlNodeType.Element)
                            continue;

                        switch (r.Name)
                        {
                            case "Map":
                                break;
                            case "Header":
                                LoadHeader(r.ReadSubtree());
                                break;
                            case "Entities":
                                LoadEntities(r.ReadSubtree());
                                break;
                            case "Misc":
                                LoadMiscs(r.ReadSubtree());
                                break;
                            default:
                                throw new Exception(string.Format("Unknown map element '{0}'", r.Name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles loading the entities
        /// </summary>
        /// <param name="r">XmlReader containing the subtree of the Entities element</param>
        void LoadEntities(XmlReader r)
        {
            // Parse the whole subtree
            while (r.Read())
            {
                // We're only interested in element starts
                if (r.NodeType != XmlNodeType.Element || r.Name == "Entities")
                    continue;

                XmlReader subTreeReader = r.ReadSubtree();
                subTreeReader.MoveToContent();

                // Handle the walls internally, forwarding the rest to the custom Entity loader
                if (r.Name == "Walls")
                    LoadWalls(subTreeReader);
                else
                    LoadEntity(subTreeReader);
            }
        }

        /// <summary>
        /// Handles loading of custom entities. Will be called for each subtree in the Entities
        /// tree, except for the Walls, which are handled internally. <paramref name="r"/> will only
        /// contain the subtree, so there are no concerns with over or under-parsing, or running
        /// into any unknowns.
        /// </summary>
        /// <param name="r">XmlReader containing the subtree for the custom Entities element</param>
        protected virtual void LoadEntity(XmlReader r)
        {
            switch (r.Name)
            {
                case "Teleports":
                    LoadTeleports(r.ReadSubtree());
                    break;
            }
        }

        /// <summary>
        /// Loads the header information for the map
        /// </summary>
        /// <param name="r">XmlReader used to load the map file</param>
        void LoadHeader(XmlReader r)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            if (r.ReadState == ReadState.Closed || r.ReadState == ReadState.Error)
                throw new ArgumentException("Invalid XmlReader state", "r");

            float width = 0;
            float height = 0;

            while (r.Read())
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "Name":
                        _name = r.ReadElementContentAsString();
                        break;
                    case "Width":
                        width = r.ReadElementContentAsFloat();
                        break;
                    case "Height":
                        height = r.ReadElementContentAsFloat();
                        break;
                }
            }

            if (width <= 0 || height <= 0)
                throw new Exception("Failed to load the Map's Width and/or Height, or an invalid value was supplied.");

            _width = width;
            _height = height;
            _entityGrid = BuildEntityGrid(Width, Height);
        }

        /// <summary>
        /// Handles loading of custom entities. Will be called for each subtree in the Misc
        /// tree. <paramref name="r"/> will only contain the subtree, so there are no concerns 
        /// with over or under-parsing, or running into any unknowns.
        /// </summary>
        /// <param name="r">XmlReader containing the subtree for the custom Misc element</param>
        protected virtual void LoadMisc(XmlReader r)
        {
        }

        /// <summary>
        /// Handles loading the misc elements
        /// </summary>
        /// <param name="r">XmlReader containing the subtree of the Misc element</param>
        void LoadMiscs(XmlReader r)
        {
            // Parse the whole subtree
            while (r.Read())
            {
                // Call LoadMisc for each element
                if (r.NodeType == XmlNodeType.Element && r.Name != "Misc")
                {
                    XmlReader subTreeReader = r.ReadSubtree();
                    subTreeReader.MoveToContent();
                    LoadMisc(subTreeReader);
                }
            }
        }

        void LoadTeleports(XmlReader r)
        {
            while (r.Read())
            {
                if (r.NodeType != XmlNodeType.Element || !r.HasAttributes)
                    continue;

                TeleportEntityBase te = CreateTeleportEntity(r);
                AddEntity(te);
            }
        }

        /// <summary>
        /// Loads the wall information for the map
        /// </summary>
        /// <param name="r">XmlReader used to load the map file</param>
        protected internal abstract void LoadWalls(XmlReader r);

        /// <summary>
        /// Removes an entity from the map
        /// </summary>
        /// <param name="entity">Entity to remove from the map</param>
        public void RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            // Remove the listeners
            RemoveEntityEventHooks(entity);

            // Remove the entity from the entity list
            if (!_entities.Remove(entity))
            {
                // Entity must have already been removed, since it wasn't in the entities list
                Debug.Fail("entity was not in the Entities list");
            }

            // If a character or item, remove from the respective list
            CharacterEntity charEntity;
            ItemEntityBase itemEntity;
            if ((charEntity = entity as CharacterEntity) != null)
            {
                Debug.Assert(_characters[charEntity.MapCharIndex] == charEntity, "Character is holding an invalid MapCharIndex!");
                _characters.RemoveAt(charEntity.MapCharIndex);
            }
            else if ((itemEntity = entity as ItemEntityBase) != null)
            {
                Debug.Assert(_items[itemEntity.MapItemIndex] == itemEntity, "Item is holding an invalid MapItemIndex!");
                _items.RemoveAt(itemEntity.MapItemIndex);
            }

            // Remove the entity from the grid, iterating through every segment to ensure we get all references
            lock (_entityGridLock)
            {
                foreach (var segment in _entityGrid)
                {
                    segment.Remove(entity);
                }
            }

            // Allow for additional processing
            EntityRemoved(entity);
        }

        void RemoveEntityEventHooks(Entity entity)
        {
            entity.OnDispose -= Entity_OnDispose;
            entity.OnMove -= Entity_OnMove;
        }

        /// <summary>
        /// Rebuilds the EntityGrid to the map's current size
        /// </summary>
        void ResizeEntityGrid()
        {
            lock (_entityGridLock)
            {
                // Get the new grid
                _entityGrid = BuildEntityGrid(Width, Height);

                // Re-add all the entities
                foreach (Entity entity in Entities)
                {
                    AddEntityToGrid(entity);
                }
            }
        }

        /// <summary>
        /// Resizes the Entity to the supplied size, but forces the Entity to remain in the map's boundaries.
        /// </summary>
        /// <param name="entity">Entity to teleport</param>
        /// <param name="size">New size of the entity</param>
        public void SafeResizeEntity(Entity entity, Vector2 size)
        {
            if (entity == null)
            {
                Debug.Fail("entity is null.");
                return;
            }

            // Ensure the entity will be in the map
            Vector2 newMax = entity.Position + size;
            if (newMax.X > Width)
                size.X = Width - entity.Position.X;
            if (newMax.Y > Height)
                size.Y = Height - entity.Position.Y;

            // Set the size
            entity.Resize(size);
        }

        /// <summary>
        /// Performs an Entity.Teleport() call to the supplied position, but forces the Entity to remain
        /// in the map's boundaries.
        /// </summary>
        /// <param name="entity">Entity to teleport</param>
        /// <param name="pos">Position to teleport to</param>
        public void SafeTeleportEntity(Entity entity, Vector2 pos)
        {
            if (entity == null)
            {
                Debug.Fail("entity is null.");
                return;
            }

            // Ensure the entity will be in the map
            if (pos.X < 0)
                pos.X = 0;
            if (pos.Y < 0)
                pos.Y = 0;

            Vector2 max = pos + entity.CB.Size;
            if (max.X > Width)
                pos.X = Width - entity.CB.Size.X;
            if (max.Y > Height)
                pos.Y = Height - entity.CB.Size.Y;

            // Teleport to the altered position
            entity.Teleport(pos);
        }

        /// <summary>
        /// Saves the map to a file to the specified content path.
        /// </summary>
        /// <param name="mapIndex">Map index to save as.</param>
        /// <param name="contentPath">Content path to save the map file to.</param>
        public void Save(ushort mapIndex, ContentPaths contentPath)
        {
            Save(contentPath.Maps.Join(mapIndex + "." + MapFileSuffix));
        }

        /// <summary>
        /// Saves the map to a file to the specified file path.
        /// </summary>
        /// <param name="filePath">Path to save the map file at.</param>
        void Save(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                using (XmlWriter w = XmlWriter.Create(stream, settings))
                {
                    if (w == null)
                        throw new Exception("Failed to create XmlWriter for map saving.");

                    // Write the start of the XML document
                    w.WriteStartDocument();
                    w.WriteStartElement("Map");

                    // Write the different parts of the map
                    SaveHeader(w);
                    SaveEntitiesInternal(w);
                    SaveMisc(w);

                    // Write the end of the XML document
                    w.WriteEndElement();
                    w.WriteEndDocument();
                }
            }
        }

        /// <summary>
        /// Saves additional entity information to the Entities element in the map
        /// </summary>
        /// <param name="w">XmlWriter to use to save the entity information</param>
        protected virtual void SaveEntities(XmlWriter w)
        {
            SaveTeleports(w);
        }

        /// <summary>
        /// Handles saving the wall entities, along with the custom entities, to the map file
        /// </summary>
        /// <param name="w">XmlWriter to use to save the entity information</param>
        void SaveEntitiesInternal(XmlWriter w)
        {
            w.WriteStartElement("Entities");
            SaveWalls(w);
            SaveEntities(w);
            w.WriteEndElement();
        }

        /// <summary>
        /// Saves the map header
        /// </summary>
        /// <param name="w">XmlWriter to write to the file</param>
        void SaveHeader(XmlWriter w)
        {
            if (w == null)
                throw new ArgumentNullException("w");
            if (w.WriteState == WriteState.Closed || w.WriteState == WriteState.Error)
                throw new ArgumentException("XmlWriter is in an invalid state", "w");

            w.WriteStartElement("Header");
            w.WriteElementString("Name", "INSERT VALUE");
            w.WriteElementString("Width", Width.ToString());
            w.WriteElementString("Height", Height.ToString());
            w.WriteEndElement();
        }

        /// <summary>
        /// Saves misc map information
        /// </summary>
        /// <param name="w">XmlWriter to write to the file</param>
        protected virtual void SaveMisc(XmlWriter w)
        {
        }

        void SaveTeleports(XmlWriter w)
        {
            w.WriteStartElement("Teleports");

            foreach (Entity entity in Entities)
            {
                TeleportEntityBase te = entity as TeleportEntityBase;
                if (te != null)
                    te.Save(w);
            }

            w.WriteEndElement();
        }

        /// <summary>
        /// Saves the map walls
        /// </summary>
        /// <param name="w">XmlWriter to write to the file</param>
        void SaveWalls(XmlWriter w)
        {
            if (w == null)
                throw new ArgumentNullException("w");
            if (w.WriteState == WriteState.Closed || w.WriteState == WriteState.Error)
                throw new ArgumentException("XmlWriter is in an invalid state", "w");

            w.WriteStartElement("Walls");
            foreach (Entity entity in Entities)
            {
                WallEntity wall = entity as WallEntity;
                if (wall != null)
                    wall.Save(w);
            }
            w.WriteEndElement();
        }

        /// <summary>
        /// Sets the new dimensions of the map and trims
        /// objects that exceed the new dimension
        /// </summary>
        /// <param name="newSize">New size of the map</param>
        public void SetDimensions(Vector2 newSize)
        {
            if (Size == newSize)
                return;

            // Remove any objects outside of the new dimensions
            if (Size.X > newSize.X || Size.Y > newSize.Y)
            {
                for (int i = 0; i < _entities.Count; i++)
                {
                    Entity entity = _entities[i];
                    if (entity == null)
                        continue;

                    if (entity is WallEntity)
                    {
                        // Remove a wall if the min value passes the new dimensions, 
                        if (entity.CB.Min.X > newSize.X || entity.CB.Max.Y > newSize.Y)
                        {
                            entity.Dispose();
                            i--;
                        }
                        else
                        {
                            // Trim down a wall if the max passes the new dimensions, but the min does not
                            Vector2 newEntitySize = entity.Size;

                            if (entity.CB.Max.X > newSize.X)
                                newSize.X = entity.CB.Max.X - newSize.X;
                            if (entity.CB.Max.Y > newSize.Y)
                                newSize.Y = entity.CB.Max.Y - newSize.Y;

                            entity.Resize(newEntitySize);
                        }
                    }
                    else
                    {
                        // Any entity who's max value is now out of bounds will be removed
                        entity.Dispose();
                    }
                }
            }

            // Update the map's size
            _width = newSize.X;
            _height = newSize.Y;
            ResizeEntityGrid();
        }

        /// <summary>
        /// Snaps a entity to any near-by entity
        /// </summary>
        /// <param name="entity">Entity to edit</param>
        /// <returns>New position for the entity</returns>
        public Vector2 SnapToWalls(Entity entity)
        {
            return SnapToWalls(entity, 20f);
        }

        /// <summary>
        /// Snaps a entity to any near-by entity
        /// </summary>
        /// <param name="entity">Entity to edit</param>
        /// <param name="maxDiff">Maximum position difference</param>
        /// <returns>New position for the entity</returns>
        public Vector2 SnapToWalls(Entity entity, float maxDiff)
        {
            Vector2 ret = new Vector2(entity.Position.X, entity.Position.Y);
            Vector2 pos = entity.Position - new Vector2(maxDiff / 2, maxDiff / 2);
            CollisionBox newCB = new CollisionBox(pos, entity.CB.Width + maxDiff, entity.CB.Height + maxDiff);

            foreach (Entity e in Entities)
            {
                WallEntity w = e as WallEntity;
                if (w == null || w == entity || !CollisionBox.Intersect(w.CB, newCB))
                    continue;

                // Selected wall right side to target wall left side
                if (Math.Abs(newCB.Max.X - w.CB.Min.X) < maxDiff)
                    ret.X = w.CB.Min.X - entity.CB.Width;

                // Selected wall left side to target wall right side
                if (Math.Abs(w.CB.Max.X - newCB.Min.X) < maxDiff)
                    ret.X = w.CB.Max.X;

                // Selected wall bottom to target wall top
                if (Math.Abs(newCB.Max.Y - w.CB.Min.Y) < maxDiff)
                    ret.Y = w.CB.Min.Y - entity.CB.Height;

                // Selected wall top to target wall bottom
                if (Math.Abs(w.CB.Max.Y - newCB.Min.Y) < maxDiff)
                    ret.Y = w.CB.Max.Y;
            }

            return ret;
        }

        public bool TryGetItem(int index, out ItemEntityBase item)
        {
            if (!_items.CanGet(index))
                item = null;
            else
                item = _items[index];

            return (item != null);
        }

        /// <summary>
        /// Updates the map
        /// </summary>
        public virtual void Update()
        {
            // Check for a valid map
            if (_entityGrid == null)
                return;

            // Calculate the elapsed time
            float deltaTime = _updateStopWatch.ElapsedMilliseconds - _lastUpdateTime;

            // Check if enough time has elapsed
            if (deltaTime < PhysicsUpdateRate)
                return;

            // Update the last update time
            _lastUpdateTime = _updateStopWatch.ElapsedMilliseconds;

            // Don't let the deltaTime go out of control
            if (deltaTime > MaxUpdateDeltaTime)
                deltaTime = MaxUpdateDeltaTime;

            // Update characters
            foreach (Entity entity in Entities)
            {
                if (entity == null)
                {
                    Debug.Fail("Entity is null... didn't think this would ever hit. o.O");
                    return;
                }

                entity.Update(this, deltaTime);
            }
        }

        void UpdateEntityGrid(Entity entity, Vector2 oldPos)
        {
            // Check that the entity changed grid segments by comparing the lowest grid segments
            // of the old position and current position
            int minX = (int)oldPos.X / WallGridSize;
            int minY = (int)oldPos.Y / WallGridSize;
            int newMinX = (int)entity.CB.Min.X / WallGridSize;
            int newMinY = (int)entity.CB.Min.Y / WallGridSize;

            if (minX == newMinX && minY == newMinY)
                return; // No change in grid segment

            int maxX = (int)(oldPos.X + entity.CB.Width) / WallGridSize;
            int maxY = (int)(oldPos.Y + entity.CB.Height) / WallGridSize;

            // Keep in range of the grid
            if (minX < 0)
                minX = 0;
            if (maxX > _entityGrid.GetLength(0) - 1)
                maxX = _entityGrid.GetLength(0) - 1;
            if (minY < 0)
                minY = 0;
            if (maxY > _entityGrid.GetLength(1) - 1)
                maxY = _entityGrid.GetLength(1) - 1;

            // Lock the entity grid
            lock (_entityGridLock)
            {
                // FUTURE: Can optimize by only adding/removing from changed grid segments

                // Remove the entity from the old grid position
                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        _entityGrid[x, y].Remove(entity);
                    }
                }

                // Re-add the entity to the grid
                AddEntityToGrid(entity);
            }
        }

        #region IMap Members

        /// <summary>
        /// Gets an IEnumerable of all the Entities on the map
        /// </summary>
        public IEnumerable<Entity> Entities
        {
            get { return _entities; }
        }

        /// <summary>
        /// Gets the height of the map in pixels
        /// </summary>
        public float Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Gets the width of the map in pixels.
        /// </summary>
        public float Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Checks if an Entity collides with any other entities. For each collision, <paramref name="entity"/>
        /// will call CollideInto(), and the Entity that <paramref name="entity"/> collided into will call
        /// CollideFrom().
        /// </summary>
        /// <param name="entity">Entity to check against other Entities. If the CollisionType is
        /// CollisionType.None, or if null, this will always return 0 and no collision detection
        /// will take place.</param>
        /// <returns>Number of collisions the <paramref name="entity"/> made with other entities</returns>
        public int CheckCollisions(Entity entity)
        {
            // Check for a null entity
            if (entity == null)
            {
                const string errmsg = "Parameter entity is null.";
                Debug.Fail(errmsg);
                if (log.IsWarnEnabled)
                    log.Warn(errmsg);
                return 0;
            }

            // Entity does not support collision detection
            if (entity.CollisionType == CollisionType.None)
                return 0;

            int collisions = 0;

            // Iterate through the grid segments
            foreach (var gridSegment in GetEntityGrids(entity.CB))
            {
                var segment = gridSegment;

                // Check that the segment even has any entities besides us
                if (segment.Count == 0 || (segment.Count == 1 && segment.Contains(entity)))
                    continue;

                // Clear our stack, since we use the same object for every segment
                _cdStack.Clear();

                // Lock the entity grid
                lock (_entityGridLock)
                {
                    // Iterate through each wall in the grid segment
                    foreach (Entity collideEntity in segment)
                    {
                        // Make sure we are not trying to collide with ourself
                        if (collideEntity == entity)
                            continue;

                        // Check that the entity even supports collision detection
                        if (collideEntity.CollisionType == CollisionType.None)
                            continue;

                        // Filter out quickly with basic rectangular collision detection
                        if (!entity.Intersect(collideEntity))
                            continue;

                        // Add the entity to our stack of entities to check for collision (if we haven't already)
                        if (!_cdStack.Contains(collideEntity))
                            _cdStack.Push(collideEntity);
                    }
                }

                // Now that we have our entities to test, test them all
                while (_cdStack.Count > 0)
                {
                    // Pop the entity to test against
                    Entity collideEntity = _cdStack.Pop();

                    // Get the displacement vector if the two entities collided
                    Vector2 displacement = CollisionBox.MTD(entity.CB, collideEntity.CB, collideEntity.CollisionType);

                    // If there is a displacement value, forward it to the collision notifiers
                    if (displacement != Vector2.Zero)
                    {
                        entity.CollideInto(collideEntity, displacement);
                        collideEntity.CollideFrom(entity, displacement);
                        collisions++;
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Gets the current time
        /// </summary>
        /// <returns>Current time</returns>
        public int GetTime()
        {
            return _getTime.GetTime();
        }

        #endregion
    }

    /// <summary>
    /// Base map class
    /// </summary>
    /// <typeparam name="TWall">Wall type</typeparam>
    public abstract class MapBase<TWall> : MapBase where TWall : WallEntity, new()
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// MapBase constructor
        /// </summary>
        /// <param name="mapIndex">Index of the map</param>
        /// <param name="getTime">Interface used to get the time</param>
        protected MapBase(ushort mapIndex, IGetTime getTime) : base(mapIndex, getTime)
        {
        }

        /// <summary>
        /// Loads the wall information for the map
        /// </summary>
        /// <param name="r">XmlReader used to load the map file</param>
        protected internal override void LoadWalls(XmlReader r)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            if (r.ReadState == ReadState.Closed || r.ReadState == ReadState.Error)
                throw new ArgumentException("Invalid XmlReader state", "r");

            while (r.Read())
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                if (r.Name == "Wall")
                {
                    Entity wall = WallEntity.Load<TWall>(r.ReadSubtree());
                    AddEntity(wall);
                }
                else
                {
                    const string errmsg = "Found element name `{0}` when expecting `Wall`";
                    Debug.Fail(string.Format(errmsg, r.Name));
                    if (log.IsErrorEnabled)
                        log.ErrorFormat(errmsg, r.Name);
                }
            }
        }
    }
}