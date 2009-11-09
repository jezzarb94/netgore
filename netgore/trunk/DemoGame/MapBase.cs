using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DemoGame.DbObjs;
using log4net;
using Microsoft.Xna.Framework;
using NetGore;
using NetGore.Collections;
using NetGore.Globalization;
using NetGore.IO;

// FUTURE: Improve how characters handle when they hit the map's borders
// TODO: Rename WallGridSize to EntityGridSize

namespace DemoGame
{
    /// <summary>
    /// Event handler for basic events from the MapBase.
    /// </summary>
    /// <param name="map">MapBase that the event came from.</param>
    public delegate void MapBaseEventHandler(MapBase map);

    public class MapEntityGrid : IMapEntityCollection
    {
        /// <summary>
        /// Size of each segment of the wall grid in pixels (smallest requires more
        /// memory but often less checks (to an extent))
        /// </summary>
        protected const int WallGridSize = 128;

        /// <summary>
        /// Two-dimensional grid of references to entities in that sector.
        /// </summary>
        List<Entity>[,] _entityGrid;

        public Point GridSize
        {
            get { return new Point(_entityGrid.GetLength(0), _entityGrid.GetLength(1)); }
        }

        protected IEnumerable<Entity> this[int x, int y]
        {
            get
            {
                if (!IsLegalGridIndex(x, y))
                    return Enumerable.Empty<Entity>();

                return _entityGrid[x, y];
            }
        }

        protected IEnumerable<Entity> this[Point gridIndex]
        {
            get { return this[gridIndex.X, gridIndex.Y]; }
        }

        public void Add(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        /// <summary>
        /// Adds an <see cref="Entity"/> to the grid.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to add to the grid.</param>
        public void Add(Entity entity)
        {
            var minX = (int)entity.CB.Min.X / WallGridSize;
            var minY = (int)entity.CB.Min.Y / WallGridSize;
            var maxX = (int)entity.CB.Max.X / WallGridSize;
            var maxY = (int)entity.CB.Max.Y / WallGridSize;

            // Keep in range of the grid
            if (minX < 0)
                minX = 0;

            if (maxX >= GridSize.X)
                maxX = GridSize.X - 1;

            if (minY < 0)
                minY = 0;

            if (maxY >= GridSize.Y)
                maxY = GridSize.Y - 1;

            // Add to all the segments of the grid
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    if (!_entityGrid[x, y].Contains(entity))
                        _entityGrid[x, y].Add(entity);
                }
            }
        }

        /// <summary>
        /// Builds a two-dimensional array of Lists to use as the grid of entities.
        /// </summary>
        /// <returns>A two-dimensional array of Lists to use as the grid of entities.</returns>
        static List<Entity>[,] BuildEntityGrid(float width, float height)
        {
            var gridWidth = (int)Math.Ceiling(width / WallGridSize);
            var gridHeight = (int)Math.Ceiling(height / WallGridSize);

            // Create the array
            var retGrid = new List<Entity>[gridWidth,gridHeight];

            // Create the lists
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    retGrid[x, y] = new List<Entity>();
                }
            }

            return retGrid;
        }

        /// <summary>
        /// Gets if the specified area or location contains any entities.
        /// </summary>
        /// <param name="rect">The map area to check.</param>
        /// <returns>True if the specified area or location contains any entities; otherwise false.</returns>
        public bool ContainsEntities(Rectangle rect)
        {
            return GetEntityGrids(rect).SelectMany(x => x).Any(x => x.CB.Intersect(rect));
        }

        /// <summary>
        /// Gets if the specified area or location contains any entities.
        /// </summary>
        /// <param name="rect">The map area to check.</param>
        /// <typeparam name="T">The type of <see cref="Entity"/> to check against. All other types of
        /// <see cref="Entity"/> will be ignored.</typeparam>
        /// <returns>True if the specified area or location contains any entities; otherwise false.</returns>
        public bool ContainsEntities<T>(Rectangle rect) where T : Entity
        {
            return GetEntityGrids(rect).SelectMany(x => x).OfType<T>().Any(x => x.CB.Intersect(rect));
        }

        /// <summary>
        /// Gets if the specified area or location contains any entities.
        /// </summary>
        /// <param name="point">The map point to check.</param>
        /// <returns>True if the specified area or location contains any entities; otherwise false.</returns>
        public bool ContainsEntities(Vector2 point)
        {
            var gridSegment = this[MapPositionToGridIndex(point)];
            return gridSegment.Any(x => x.CB.HitTest(point));
        }

        /// <summary>
        /// Gets if the specified area or location contains any entities.
        /// </summary>
        /// <param name="point">The map point to check.</param>
        /// <typeparam name="T">The type of <see cref="Entity"/> to check against. All other types of
        /// <see cref="Entity"/> will be ignored.</typeparam>
        /// <returns>True if the specified area or location contains any entities; otherwise false.</returns>
        public bool ContainsEntities<T>(Vector2 point) where T : Entity
        {
            var gridSegment = this[MapPositionToGridIndex(point)];
            return gridSegment.OfType<T>().Any(x => x.CB.HitTest(point));
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region.
        /// </summary>
        /// <param name="rect">Region to check for Entities.</param>
        /// <returns>All Entities found intersecting the given region.</returns>
        public IEnumerable<Entity> GetEntities(Rectangle rect)
        {
            var gridSegments = GetEntityGrids(rect);
            var matches = gridSegments.SelectMany(x => x).Where(x => x.CB.Intersect(rect)).Distinct();
            return matches;
        }

        /// <summary>
        /// Gets all entities at the given point.
        /// </summary>
        /// <param name="p">The point to find the entities at.</param>
        /// <typeparam name="T">The type of <see cref="Entity"/> to look for.</typeparam>
        /// <returns>All entities containing the given point that are of the given type.</returns>
        public IEnumerable<T> GetEntities<T>(Vector2 p) where T : Entity
        {
            var gridSegment = this[MapPositionToGridIndex(p)];
            var matches = gridSegment.OfType<T>().Where(x => x.CB.HitTest(p));
            Debug.Assert(!matches.HasDuplicates(), "Somehow we had duplicates even though we only used one grid segment!");
            return matches;
        }

        /// <summary>
        /// Gets the Entities found intersecting the given region.
        /// </summary>
        /// <param name="rect">Region to check for Entities.</param>
        /// <typeparam name="T">Type of Entity to look for.</typeparam>
        /// <returns>All Entities found intersecting the given region.</returns>
        public IEnumerable<T> GetEntities<T>(Rectangle rect) where T : Entity
        {
            var gridSegments = GetEntityGrids(rect);
            var matches = gridSegments.SelectMany(x => x).OfType<T>().Where(x => x.CB.Intersect(rect)).Distinct();
            return matches;
        }

        /// <summary>
        /// Gets all entities containing a given point.
        /// </summary>
        /// <param name="p">Point to find the entities at.</param>
        /// <returns>All of the entities at the given point.</returns>
        public IEnumerable<Entity> GetEntities(Vector2 p)
        {
            var gridSegment = this[MapPositionToGridIndex(p)];
            var matches = gridSegment.Where(x => x.CB.HitTest(p));
            Debug.Assert(!matches.HasDuplicates(), "Somehow we had duplicates even though we only used one grid segment!");
            return matches;
        }

        /// <summary>
        /// Gets the grid segment for each intersection on the entity grid.
        /// </summary>
        /// <param name="rect">Map area to get the grid segments for.</param>
        /// <returns>An IEnumerable of all grid segments intersected by the specified <paramref name="rect"/>.</returns>
        protected IEnumerable<IEnumerable<Entity>> GetEntityGrids(Rectangle rect)
        {
            var minX = rect.X / WallGridSize;
            var minY = rect.Y / WallGridSize;
            var maxX = rect.Right / WallGridSize;
            var maxY = rect.Bottom / WallGridSize;

            // Keep in range of the grid
            if (minX < 0)
                minX = 0;

            if (maxX >= GridSize.X)
                maxX = GridSize.X - 1;

            if (minY < 0)
                minY = 0;

            // TODO: For some reason this last check here likes to fail a lot. I think it is because gravity pushes an Entity out of the map temporarily when they are down low. Ideally, this condition is NEVER reached.
            if (maxY >= GridSize.Y)
                maxY = GridSize.Y - 1;

            // Return the grid segments
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    yield return this[x, y];
                }
            }
        }

        protected bool IsLegalGridIndex(Point point)
        {
            return IsLegalGridIndex(point.X, point.Y);
        }

        protected bool IsLegalGridIndex(int x, int y)
        {
            return x >= 0 && x < _entityGrid.GetLength(0) && y >= 0 && y < _entityGrid.GetLength(1);
        }

        static Point MapPositionToGridIndex(Vector2 position)
        {
            var x = position.X / WallGridSize;
            var y = position.Y / WallGridSize;
            return new Point((int)x, (int)y);
        }

        public void Remove(Entity entity)
        {
            foreach (var gridSegment in _entityGrid)
            {
                gridSegment.Remove(entity);
            }
        }

        /// <summary>
        /// Sets the source map to a new size and clears out all existing entities in the grid.
        /// </summary>
        /// <param name="size">The new size of the source map.</param>
        public void SetMapSize(Vector2 size)
        {
            _entityGrid = BuildEntityGrid(size.X, size.Y);
        }

        public void UpdateEntity(Entity entity, Vector2 oldPos)
        {
            // FUTURE: Can optimize this method quite a lot by only adding/removing from changed grid segments

            // Check that the entity changed grid segments by comparing the lowest grid segments
            // of the old position and current position
            var minX = (int)oldPos.X / WallGridSize;
            var minY = (int)oldPos.Y / WallGridSize;
            var newMinX = (int)entity.CB.Min.X / WallGridSize;
            var newMinY = (int)entity.CB.Min.Y / WallGridSize;

            if (minX == newMinX && minY == newMinY)
                return; // No change in grid segment

            var maxX = (int)(oldPos.X + entity.CB.Width) / WallGridSize;
            var maxY = (int)(oldPos.Y + entity.CB.Height) / WallGridSize;

            // Keep in range of the grid
            if (minX < 0)
                minX = 0;

            if (maxX >= GridSize.X)
                maxX = GridSize.X - 1;

            if (minY < 0)
                minY = 0;

            if (maxY >= GridSize.Y)
                maxY = GridSize.Y - 1;

            // Remove the entity from the old grid position
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    _entityGrid[x, y].Remove(entity);
                }
            }

            // Re-add the entity to the grid
            Add(entity);
        }
    }

    /// <summary>
    /// Base map class
    /// </summary>
    public abstract class MapBase : IMap, IMapTable
    {
        /// <summary>
        /// The suffix used for map files. Does not include the period prefix.
        /// </summary>
        public const string MapFileSuffix = "xml";

        const string _dynamicEntitiesNodeName = "DynamicEntities";

        /// <summary>
        /// The number of additional pixels to search in each direction around an entity when finding
        /// a valid place to put the entity. Larger values will allow for a better chance of finding a valid position,
        /// but will allow the entity to warp farther away from their initial position.
        /// </summary>
        const int _findValidPlacementPadding = 128;

        const string _headerNodeHeightKey = "Height";
        const string _headerNodeMusicKey = "Music";
        const string _headerNodeName = "Header";
        const string _headerNodeNameKey = "Name";
        const string _headerNodeWidthKey = "Width";
        const string _miscNodeName = "Misc";
        const string _rootNodeName = "Map";

        const string _wallsNodeName = "Walls";

        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Enumerator for the DynamicEntities.
        /// </summary>
        readonly SafeEnumerator<DynamicEntity> _dyanmicEntityEnumerator;

        /// <summary>
        /// Collection of DynamicEntities on this map.
        /// </summary>
        readonly DArray<DynamicEntity> _dynamicEntities;

        /// <summary>
        /// List of entities in the map
        /// </summary>
        readonly List<Entity> _entities;

        /// <summary>
        /// Enumerator for the Entities.
        /// </summary>
        readonly SafeEnumerator<Entity> _entityEnumerator;

        readonly MapEntityGrid _entityGrid = new MapEntityGrid();

        /// <summary>
        /// Interface used to get the time
        /// </summary>
        readonly IGetTime _getTime;

        /// <summary>
        /// Index of the map
        /// </summary>
        readonly MapIndex _mapIndex;

        /// <summary>
        /// StopWatch used to update the map
        /// </summary>
        readonly Stopwatch _updateStopWatch = new Stopwatch();

        /// <summary>
        /// Height of the map in pixels
        /// </summary>
        float _height = float.MinValue;

        /// <summary>
        /// If the map is actively updating (set to false to "pause" the physics)
        /// </summary>
        bool _isUpdating = true;

        /// <summary>
        /// Name of the map
        /// </summary>
        string _name = null;

        /// <summary>
        /// Width of the map in pixels
        /// </summary>
        float _width = float.MinValue;

        /// <summary>
        /// Notifies listeners that the Map has been saved.
        /// </summary>
        public event MapBaseEventHandler OnSave;

        /// <summary>
        /// Gets a thread-safe IEnumerable of all the DynamicEntities on the Map.
        /// </summary>
        public IEnumerable<DynamicEntity> DynamicEntities
        {
            get { return _dyanmicEntityEnumerator; }
        }

        public MapEntityGrid EntityGrid
        {
            get
            {
                // NOTE: This is only temporary. Should have an interface that only allows looking for entities (IMapEntityCollection?)
                return _entityGrid;
            }
        }

        /// <summary>
        /// Gets the index of the map.
        /// </summary>
        public MapIndex Index
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
        /// Gets or sets the name of the music to play for the map, or empty or null if there is no music.
        /// </summary>
        public string Music { get; set; }

        /// <summary>
        /// MapBase constructor
        /// </summary>
        /// <param name="mapIndex">Index of the map</param>
        /// <param name="getTime">Interface used to get the time</param>
        protected MapBase(MapIndex mapIndex, IGetTime getTime)
        {
            if (getTime == null)
                throw new ArgumentNullException("getTime");

            _getTime = getTime;
            _mapIndex = mapIndex;
            _updateStopWatch.Start();

            _entities = new List<Entity>();
            _entityEnumerator = new SafeEnumerator<Entity>(_entities);

            _dynamicEntities = new DArray<DynamicEntity>(true);
            _dyanmicEntityEnumerator = new SafeEnumerator<DynamicEntity>(_dynamicEntities);
        }

        /// <summary>
        /// Adds a DynamicEntity to the Map, using the pre-determined unique index.
        /// </summary>
        /// <param name="entity">DynamicEntity to add to the Map.</param>
        /// <param name="mapEntityIndex">Unique index to assign to the DynamicEntity.</param>
        public virtual void AddDynamicEntity(DynamicEntity entity, MapEntityIndex mapEntityIndex)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Debug.Assert(!_dynamicEntities.Contains(entity), "DynamicEntity is already in the DynamicEntity list!");

            if (_dynamicEntities.CanGet((int)mapEntityIndex))
            {
                var existingDE = _dynamicEntities[(int)mapEntityIndex];
                if (existingDE != null)
                {
                    Debug.Fail("A DynamicEntity already exists at this MapEntityIndex!");
                    RemoveEntity(existingDE);
                }
            }

            entity.MapEntityIndex = mapEntityIndex;
            _dynamicEntities[(int)mapEntityIndex] = entity;

            AddEntityFinish(entity);
        }

        /// <summary>
        /// Adds an Entity to the map.
        /// </summary>
        /// <param name="entity">Entity to add to the map.</param>
        public virtual void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            // Assign a DynamicEntity its unique map index
            DynamicEntity dynamicEntity;
            if ((dynamicEntity = entity as DynamicEntity) != null)
            {
                Debug.Assert(!_dynamicEntities.Contains(dynamicEntity), "DynamicEntity is already in the DynamicEntity list!");
                dynamicEntity.MapEntityIndex = new MapEntityIndex(_dynamicEntities.Insert(dynamicEntity));
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

            // Also add the entity to the grid
            _entityGrid.Add(entity);

            // Add the event hooks
            AddEntityEventHooks(entity);

            // Allow for additional processing
            EntityAdded(entity);
        }

        /// <summary>
        /// If the given point is in the map's boundaries
        /// </summary>
        /// <param name="p">Point to check</param>
        /// <returns>True if the point is in the map's boundaries, else false</returns>
        public bool Contains(Vector2 p)
        {
            // TODO: Get rid of this method? That, or rename it.
            return p.X >= 0 && p.Y >= 0 && p.X <= Width && p.Y <= Height;
        }

        /// <summary>
        /// If the given Rectangle is in the map's boundaries
        /// </summary>
        /// <param name="rect">Rectangle to check</param>
        /// <returns>True if the Rectangle is in the map's boundaries, else false</returns>
        public bool Contains(Rectangle rect)
        {
            // TODO: Get rid of this method? That, or rename it.
            return rect.X >= 0 && rect.Y >= 0 & rect.Right <= Width && rect.Bottom <= Height;
        }

        /// <summary>
        /// If the given CollisionBox is in the map's boundaries
        /// </summary>
        /// <param name="cb">CollisionBox to check</param>
        /// <returns>True if the CollisionBox is in the map's boundaries, else false</returns>
        public bool Contains(CollisionBox cb)
        {
            // TODO: Get rid of this method? That, or rename it.
            return Contains(cb.ToRectangle());
        }

        /// <summary>
        /// When overridden in the derived class, creates a new WallEntityBase instance.
        /// </summary>
        /// <returns>WallEntityBase that is to be used on the map.</returns>
        protected abstract WallEntityBase CreateWall(IValueReader r);

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
            _entityGrid.UpdateEntity(entity, oldPos);
            ForceEntityInMapBoundaries(entity);
        }

        /// <summary>
        /// When overridden in the derived class, allows for additional processing on Entities added to the map.
        /// This is called after the Entity has finished being added to the map.
        /// </summary>
        /// <param name="entity">Entity that was added to the map.</param>
        protected virtual void EntityAdded(Entity entity)
        {
        }

        /// <summary>
        /// When overriddeni n the derived class, allows for additional processing on Entities removed from the map.
        /// This is called after the Entity has finished being removed from the map.
        /// </summary>
        /// <param name="entity">Entity that was removed from the map.</param>
        protected virtual void EntityRemoved(Entity entity)
        {
        }

        /// <summary>
        /// Checks if an Entity is in the map's boundaries and, if it is not, moves the Entity into the map's boundaries.
        /// </summary>
        /// <param name="entity">Entity to check.</param>
        void ForceEntityInMapBoundaries(Entity entity)
        {
            var min = entity.CB.Min;
            var max = entity.CB.Max;

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
        /// Gets the DynamicEntity with the specified index.
        /// </summary>
        /// <param name="mapEntityIndex">Index of the DynamicEntity to get.</param>
        /// <returns>The DynamicEntity with the specified <paramref name="mapEntityIndex"/>, or null if
        /// no <see cref="DynamicEntity"/> was found..</returns>
        public DynamicEntity GetDynamicEntity(MapEntityIndex mapEntityIndex)
        {
            if (!_dynamicEntities.CanGet((int)mapEntityIndex))
                return null;

            try
            {
                return _dynamicEntities[(int)mapEntityIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the DynamicEntity with the specified index.
        /// </summary>
        /// <typeparam name="T">Type of DynamicEntity to get.</typeparam>
        /// <param name="mapEntityIndex">Index of the DynamicEntity to get.</param>
        /// <returns>The DynamicEntity with the specified <paramref name="mapEntityIndex"/>, or null if
        /// no <see cref="DynamicEntity"/> was found.</returns>
        public T GetDynamicEntity<T>(MapEntityIndex mapEntityIndex) where T : DynamicEntity
        {
            var dynamicEntity = GetDynamicEntity(mapEntityIndex);
            if (dynamicEntity == null)
                return null;

            var casted = dynamicEntity as T;

            if (casted == null)
            {
                const string errmsg = "DynamicEntity found, but was of type `{0}`, not type `{1}` like expected!";
                Debug.Fail(string.Format(errmsg, dynamicEntity.GetType(), typeof(T)));
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, dynamicEntity.GetType(), typeof(T));
            }

            return casted;
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
        public static MapIndex GetNextFreeIndex(ContentPaths path)
        {
            var mapFiles = GetMapFiles(path);

            // Get the used map indices
            var usedIndices = new List<MapIndex>(mapFiles.Count());
            foreach (var file in mapFiles)
            {
                MapIndex o;
                if (TryGetIndexFromPath(file, out o))
                    usedIndices.Add(o);
            }

            usedIndices.Sort();

            // Check every map index starting at 1, returning the first free value found
            var expected = 1;
            for (var i = 0; i < usedIndices.Count; i++)
            {
                if ((int)usedIndices[i] != expected)
                    return new MapIndex(expected);

                expected++;
            }

            return new MapIndex(expected);
        }

        /// <summary>
        /// Gets the possible positions to place the given <see cref="CollisionBox"/> around an
        /// <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to place around.</param>
        /// <param name="cb">The <see cref="CollisionBox"/> describing the area to be placed.</param>
        /// <returns>An IEnumerable of the min positions to place the <paramref name="cb"/> around the
        /// given <paramref name="entity"/>.</returns>
        static IEnumerable<Vector2> GetPositionsAroundEntity(Entity entity, CollisionBox cb)
        {
            var srcCB = entity.CB;

            // Top
            yield return new Vector2(cb.Min.X, srcCB.Min.Y - cb.Height - 1);

            // Bottom
            yield return new Vector2(cb.Min.X, srcCB.Max.Y + 1);

            // Left
            yield return new Vector2(srcCB.Min.X - cb.Width - 1, cb.Min.Y);

            // Right
            yield return new Vector2(srcCB.Max.X + 1, cb.Min.Y);

            // Top, left-aligned
            yield return new Vector2(srcCB.Min.X, srcCB.Min.Y - cb.Height - 1);

            // Top, right-aligned
            yield return new Vector2(srcCB.Max.X - cb.Width, srcCB.Min.Y - cb.Height - 1);

            // Bottom, left-aligned
            yield return new Vector2(srcCB.Min.X, srcCB.Max.Y + 1);

            // Bottom, right-aligned
            yield return new Vector2(srcCB.Max.X - cb.Width, srcCB.Max.Y + 1);

            // Left, top-aligned
            yield return new Vector2(srcCB.Min.X - cb.Width - 1, srcCB.Min.Y);

            // Left, bottom-aligned
            yield return new Vector2(srcCB.Min.X - cb.Width - 1, srcCB.Max.Y - cb.Height);

            // Right, top-aligned
            yield return new Vector2(srcCB.Max.X + 1, srcCB.Min.Y);

            // Right, bottom-aligned
            yield return new Vector2(srcCB.Max.X + 1, srcCB.Max.Y - cb.Height);
        }

        public bool IsInMapBoundaries(Vector2 min, Vector2 max)
        {
            return IsInMapBoundaries(min) && IsInMapBoundaries(max);
        }

        public bool IsInMapBoundaries(CollisionBox cb)
        {
            return IsInMapBoundaries(cb.ToRectangle());
        }

        public bool IsInMapBoundaries(Entity entity)
        {
            return IsInMapBoundaries(entity.CB);
        }

        public bool IsInMapBoundaries(Rectangle rect)
        {
            var min = new Vector2(rect.X, rect.Y);
            var max = new Vector2(rect.Right, rect.Bottom);
            return IsInMapBoundaries(min, max);
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
            if (!filePath.EndsWith("." + MapFileSuffix, StringComparison.OrdinalIgnoreCase))
                return false;

            // Check if the file is named properly
            MapIndex index;
            if (!Parser.Invariant.TryParse(Path.GetFileNameWithoutExtension(filePath), out index))
                return false;

            // Validate the file index
            if (index < 1)
                return false;

            // All checks passed
            return true;
        }

        /// <summary>
        /// Checks if a <see cref="CollisionBox"/> is in a place that is inside of the map and does not intersect
        /// any <see cref="WallEntityBase"/>s.
        /// </summary>
        /// <param name="cb">The <see cref="CollisionBox"/> containing the placement to check.</param>
        /// <returns>True if the <see cref="CollisionBox"/> is in an area that does not intersect any
        /// <see cref="WallEntityBase"/>s; otherwise false.</returns>
        public bool IsValidPlacementPosition(CollisionBox cb)
        {
            return IsInMapBoundaries(cb) && !_entityGrid.ContainsEntities(cb);
        }

        /// <summary>
        /// Checks if a <see cref="Rectangle"/> is in a place that is inside of the map and does not intersect
        /// any <see cref="WallEntityBase"/>s.
        /// </summary>
        /// <param name="rect">The <see cref="Rectangle"/> containing the placement to check.</param>
        /// <returns>True if the <see cref="Rectangle"/> is in an area that does not intersect any
        /// <see cref="WallEntityBase"/>s; otherwise false.</returns>
        public bool IsValidPlacementPosition(Rectangle rect)
        {
            return IsInMapBoundaries(rect) && !_entityGrid.ContainsEntities<WallEntity>(rect);
        }

        public bool IsValidPlacementPosition(Vector2 position, Vector2 size)
        {
            var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            return IsValidPlacementPosition(rect);
        }

        /// <summary>
        /// Checks if a <see cref="CollisionBox"/> is in a place that is inside of the map and does not intersect
        /// any <see cref="WallEntityBase"/>s.
        /// </summary>
        /// <param name="cb">The <see cref="CollisionBox"/> containing the placement to check.</param>
        /// <param name="closestValidPosition">When this method returns false, contains the closest valid position
        /// that the <see cref="CollisionBox"/> can be at to not intersect any <see cref="WallEntityBase"/>s.</param>
        /// <param name="validPositionFound">When this method returns false, contains if the
        /// <see cref="closestValidPosition"/> that was found is valid. If false, no near-by legal position could be
        /// found that the <see cref="CollisionBox"/> could occupy without causing any intersections.</param>
        /// <returns>True if the <see cref="CollisionBox"/> is in an area that does not intersect any
        /// <see cref="WallEntityBase"/>s; otherwise false.</returns>
        public bool IsValidPlacementPosition(CollisionBox cb, out Vector2 closestValidPosition, out bool validPositionFound)
        {
            // Perform the initial check to see if we need to even find a new position
            if (IsValidPlacementPosition(cb))
            {
                // No intersections - already a valid position
                closestValidPosition = cb.Min;
                validPositionFound = true;
                return true;
            }

            // Intersections were found, so we have to find a valid position
            // First, grab the walls in the region around the cb
            var nearbyWallsRect = new Rectangle((int)cb.Min.X - _findValidPlacementPadding,
                                                (int)cb.Min.Y - _findValidPlacementPadding,
                                                (int)cb.Width + (_findValidPlacementPadding * 2),
                                                (int)cb.Height + (_findValidPlacementPadding * 2));
            var nearbyWalls = _entityGrid.GetEntities<WallEntityBase>(nearbyWallsRect);

            // Next, find the legal positions we can place the cb
            var cbSize = cb.Size;
            var validPlacementPositions =
                nearbyWalls.SelectMany(wall => GetPositionsAroundEntity(wall, cb)).Where(p => IsValidPlacementPosition(p, cbSize));

            // If there are 0 legal positions, we're F'd in the A
            if (validPlacementPositions.Count() == 0)
            {
                // Failure :(
                validPositionFound = false;
                closestValidPosition = Vector2.Zero;
            }
            else
            {
                // One or more legal positions found, so find the closest one and use that
                validPositionFound = true;
                var cbMin = cb.Min;
                closestValidPosition = validPlacementPositions.MinElement(x => x.QuickDistance(cbMin));
            }

            return false;
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
        /// <param name="loadDynamicEntities">If true, the DynamicEntities will be loaded from the file. If false,
        /// all DynamicEntities will be skipped.</param>
        public void Load(ContentPaths contentPath, bool loadDynamicEntities)
        {
            string path = contentPath.Maps.Join(Index + "." + MapFileSuffix);
            Load(path, loadDynamicEntities);
        }

        /// <summary>
        /// Loads the map.
        /// </summary>
        /// <param name="filePath">Path to the file to load</param>
        /// <param name="loadDynamicEntities">If true, the DynamicEntities will be loaded from the file. If false,
        /// all DynamicEntities will be skipped.</param>
        protected virtual void Load(string filePath, bool loadDynamicEntities)
        {
            if (!File.Exists(filePath))
            {
                const string errmsg = "Map file `{0}` does not exist - unable to load map.";
                Debug.Fail(string.Format(errmsg, filePath));
                log.ErrorFormat(errmsg, filePath);
                throw new ArgumentException("filePath");
            }

            var r = new XmlValueReader(filePath, _rootNodeName);
            LoadHeader(r);
            LoadWalls(r);
            LoadDynamicEntities(r, loadDynamicEntities);
            LoadMisc(r.ReadNode(_miscNodeName));
        }

        void LoadDynamicEntities(IValueReader r, bool loadDynamicEntities)
        {
            var loadedDynamicEntities = r.ReadManyNodes<DynamicEntity>(_dynamicEntitiesNodeName, DynamicEntityFactory.Read);

            // Add the loaded DynamicEntities to the map
            if (loadDynamicEntities)
            {
                foreach (var dynamicEntity in loadedDynamicEntities)
                {
                    AddEntity(dynamicEntity);
                }
            }
        }

        /// <summary>
        /// Loads the header information for the map
        /// </summary>
        /// <param name="r">XmlReader used to load the map file</param>
        void LoadHeader(IValueReader r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            var nodeReader = r.ReadNode(_headerNodeName);

            // Read the values
            Name = nodeReader.ReadString(_headerNodeNameKey);
            Music = nodeReader.ReadString(_headerNodeMusicKey);
            _width = nodeReader.ReadFloat(_headerNodeWidthKey);
            _height = nodeReader.ReadFloat(_headerNodeHeightKey);

            // Build the entity grid
            _entityGrid.SetMapSize(Size);
        }

        /// <summary>
        /// Handles loading of custom values.
        /// </summary>
        /// <param name="r">IValueReader to read the misc values from.</param>
        protected virtual void LoadMisc(IValueReader r)
        {
        }

        /// <summary>
        /// Loads the wall information for the map
        /// </summary>
        /// <param name="r">XmlReader used to load the map file</param>
        void LoadWalls(IValueReader r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            var loadedWalls = r.ReadManyNodes<WallEntityBase>(_wallsNodeName, CreateWall);

            // Add the loaded walls to the map
            foreach (var wall in loadedWalls)
            {
                AddEntity(wall);
            }
        }

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

            // If a DynamicEntity, remove it from the DynamicEntities list
            DynamicEntity dynamicEntity;
            if ((dynamicEntity = entity as DynamicEntity) != null)
            {
                Debug.Assert(_dynamicEntities[(int)dynamicEntity.MapEntityIndex] == dynamicEntity,
                             "DynamicEntity is holding an invalid MapEntityIndex!");
                _dynamicEntities.RemoveAt((int)dynamicEntity.MapEntityIndex);
            }

            // Remove the entity from the grid
            _entityGrid.Remove(entity);

            // Allow for additional processing
            EntityRemoved(entity);
        }

        void RemoveEntityEventHooks(Entity entity)
        {
            entity.OnDispose -= Entity_OnDispose;
            entity.OnMove -= Entity_OnMove;
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
            var newMax = entity.Position + size;
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

            var max = pos + entity.CB.Size;
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
        public void Save(MapIndex mapIndex, ContentPaths contentPath)
        {
            var path = contentPath.Maps.Join(mapIndex + "." + MapFileSuffix);
            Save(path);

            if (OnSave != null)
                OnSave(this);
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
            using (IValueWriter w = new XmlValueWriter(filePath, _rootNodeName))
            {
                SaveHeader(w);
                SaveWalls(w);
                SaveDynamicEntities(w);

                w.WriteStartNode(_miscNodeName);
                SaveMisc(w);
                w.WriteEndNode(_miscNodeName);
            }
        }

        void SaveDynamicEntities(IValueWriter w)
        {
            w.WriteManyNodes(_dynamicEntitiesNodeName, DynamicEntities, DynamicEntityFactory.Write);
        }

        /// <summary>
        /// Saves the map header
        /// </summary>
        /// <param name="w">IValueWriter to write to.</param>
        void SaveHeader(IValueWriter w)
        {
            if (w == null)
                throw new ArgumentNullException("w");

            w.WriteStartNode(_headerNodeName);
            {
                w.Write(_headerNodeNameKey, "INSERT VALUE");
                w.Write(_headerNodeMusicKey, Music);
                w.Write(_headerNodeWidthKey, Width);
                w.Write(_headerNodeHeightKey, Height);
            }
            w.WriteEndNode(_headerNodeName);
        }

        /// <summary>
        /// When overridden in the derived class, saves misc map information specific to the derived class.
        /// </summary>
        /// <param name="w">IValueWriter to write to.</param>
        protected virtual void SaveMisc(IValueWriter w)
        {
        }

        /// <summary>
        /// Saves the map walls
        /// </summary>
        /// <param name="w">IValueWriter to write to.</param>
        void SaveWalls(IValueWriter w)
        {
            if (w == null)
                throw new ArgumentNullException("w");

            var wallsToSave = Entities.OfType<WallEntityBase>();
            w.WriteManyNodes(_wallsNodeName, wallsToSave, ((writer, item) => item.Write(writer)));
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
                for (var i = 0; i < _entities.Count; i++)
                {
                    var entity = _entities[i];
                    if (entity == null)
                        continue;

                    if (entity is WallEntityBase)
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
                            var newEntitySize = entity.Size;

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

            // Update the grid's size
            _entityGrid.SetMapSize(Size);
            _entityGrid.Add(Entities);
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
            var ret = new Vector2(entity.Position.X, entity.Position.Y);
            var pos = entity.Position - new Vector2(maxDiff / 2, maxDiff / 2);
            var newCB = new CollisionBox(pos, entity.CB.Width + maxDiff, entity.CB.Height + maxDiff);

            foreach (var e in Entities)
            {
                var w = e as WallEntityBase;
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

        /// <summary>
        /// Tries to get the DynamicEntity at the specified index.
        /// </summary>
        /// <param name="index">Unique index of the DynamicEntity to get.</param>
        /// <param name="dynamicEntity">DynamicEntity found at the specified index, or null if none found.</param>
        /// <returns>True if the DynamicEntity was successfully found, else false.</returns>
        public bool TryGetDynamicEntity(MapEntityIndex index, out DynamicEntity dynamicEntity)
        {
            if (!_dynamicEntities.CanGet((int)index))
                dynamicEntity = null;
            else
                dynamicEntity = _dynamicEntities[(int)index];

            return (dynamicEntity != null);
        }

        /// <summary>
        /// Tries to get the DynamicEntity at the specified index.
        /// </summary>
        /// <typeparam name="T">Type of DynamicEntity to find.</typeparam>
        /// <param name="index">Unique index of the DynamicEntity to get.</param>
        /// <param name="dynamicEntity">DynamicEntity found at the specified index and of
        /// the specified type, otherwise null.</param>
        /// <returns>True if the DynamicEntity was successfully found, else false.</returns>
        public bool TryGetDynamicEntity<T>(MapEntityIndex index, out T dynamicEntity) where T : DynamicEntity
        {
            if (!_dynamicEntities.CanGet((int)index))
                dynamicEntity = null;
            else
                dynamicEntity = _dynamicEntities[(int)index] as T;

            return (dynamicEntity != null);
        }

        /// <summary>
        /// Tries to get the Map's index from the file path.
        /// </summary>
        /// <param name="path">File path to the map.</param>
        /// <param name="mapIndex">If this method returns true, contains the index of the map.</param>
        /// <returns>True if the parsing was successful; otherwise false.</returns>
        public static bool TryGetIndexFromPath(string path, out MapIndex mapIndex)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var fileName = Path.GetFileNameWithoutExtension(path);
            return Parser.Invariant.TryParse(fileName, out mapIndex);
        }

        /// <summary>
        /// Updates the map
        /// </summary>
        public virtual void Update(int deltaTime)
        {
            // Check for a valid map
            if (_entityGrid == null)
                return;

            // Update the Entities
            foreach (var entity in Entities)
            {
                if (entity == null)
                {
                    Debug.Fail("Entity is null... didn't think this would ever hit. o.O");
                    return;
                }

                entity.Update(this, deltaTime);
            }
        }

        #region IMap Members

        /// <summary>
        /// Gets the size of the map in pixels.
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
        }

        /// <summary>
        /// Gets a thrad-safe IEnumerable of all the Entities on the Map.
        /// </summary>
        public IEnumerable<Entity> Entities
        {
            get { return _entityEnumerator; }
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
        /// CollisionType.None, or if <paramref name="entity"/> is null, this will always return 0 and no
        /// collision detection will take place.</param>
        /// <returns>
        /// Number of collisions the <paramref name="entity"/> made with other entities.
        /// </returns>
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

            // Check that this entity even supports collision detection
            if (entity.CollisionType == CollisionType.None)
                return 0;

            int collisions = 0;

            // Get the entities we have a rectangular collision with

            // It is absolutely VITAL that we have the ToArray() at the end (or to any other type of collection). Even
            // though it may look like it is useless since we just enumerate over it immediately after, this will
            // ensure that any logic in CollideInto and CollideFrom in the Entity updating that moves the Entity will
            // not break the enumeration by changing the underlying collection.
            var rectCollisionEntities =
                _entityGrid.GetEntities(entity.CB.ToRectangle(), x => x.CollisionType != CollisionType.None).ToArray();

            // Do real collision detection on the entities, and handle it if the collision test passes
            foreach (var collideEntity in rectCollisionEntities)
            {
                // Get the displacement vector if the two entities collided
                var displacement = CollisionBox.MTD(entity.CB, collideEntity.CB, collideEntity.CollisionType);

                // If there is a displacement value, forward it to the collision notifiers
                if (displacement != Vector2.Zero)
                {
                    entity.CollideInto(collideEntity, displacement);
                    collideEntity.CollideFrom(entity, displacement);
                    collisions++;
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

        #region IMapTable Members

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        IMapTable IMapTable.DeepCopy()
        {
            return new MapTable(this);
        }

        MapIndex IMapTable.ID
        {
            get { return Index; }
        }

        /// <summary>
        /// Gets or sets the name of the map.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion
    }
}