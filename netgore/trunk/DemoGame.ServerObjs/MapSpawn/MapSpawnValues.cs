﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using DemoGame.Server.Queries;
using log4net;
using NetGore;

namespace DemoGame.Server
{
    /// <summary>
    /// Contains and internally synchronizes to the database the values used to specify how Characters spawn on a Map.
    /// </summary>
    public class MapSpawnValues
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        DBController _dbController;
        readonly MapSpawnValuesID _id;

        CharacterTemplateID _characterTemplateID;
        MapIndex _mapIndex;
        byte _spawnAmount;
        MapSpawnRect _spawnArea;

        /// <summary>
        /// Gets or sets the CharacterTemplateID of the CharacterTemplate to spawn.
        /// </summary>
        [Browsable(true)]
        [Description("The ID of the CharacterTemplate to spawn.")]
        public CharacterTemplateID CharacterTemplateID
        {
            get { return _characterTemplateID; }
            set
            {
                if (_characterTemplateID == value)
                    return;

                _characterTemplateID = value;
                UpdateDB();
            }
        }

        /// <summary>
        /// Gets the DBController used to synchronize changes to the values.
        /// </summary>
        [Browsable(false)]
        public DBController DBController
        {
            get { return _dbController; }
        }

        /// <summary>
        /// Gets the unique ID of this MapSpawnValues.
        /// </summary>
        [Browsable(false)]
        public MapSpawnValuesID ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets or sets the index of the Map that these values are for.
        /// </summary>
        [Browsable(false)]
        public MapIndex MapIndex
        {
            get { return _mapIndex; }
            set
            {
                if (_mapIndex == value)
                    return;

                _mapIndex = value;
                UpdateDB();
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of Characters that will be spawned by this MapSpawnValues.
        /// </summary>
        [Browsable(true)]
        [Description("The maximum number of Characters that will be spawned by this MapSpawnValues.")]
        public byte SpawnAmount
        {
            get { return _spawnAmount; }
            set
            {
                if (_spawnAmount == value)
                    return;

                _spawnAmount = value;
                UpdateDB();
            }
        }

        /// <summary>
        /// Gets the area on the map the spawning will take place at.
        /// </summary>
        [Browsable(true)]
        [Description("The area on the map the spawning will take place at.")]
        public MapSpawnRect SpawnArea
        {
            get { return _spawnArea; }
            private set
            {
                _spawnArea = value;
                UpdateDB();
            }
        }

        static MapSpawnValuesID GetFreeID(DBController dbController)
        {
            return new MapSpawnValuesID(dbController.GetQuery<MapSpawnValuesIDCreator>().GetNext());
        }

        /// <summary>
        /// MapSpawnValues constructor.
        /// </summary>
        /// <param name="dbController">The DBController used to synchronize changes to the values.</param>
        /// <param name="mapIndex">The index of the Map that these values are for.</param>
        /// <param name="characterTemplateID">The CharacterTemplateID of the CharacterTemplate to spawn.</param>
        public MapSpawnValues(DBController dbController, MapIndex mapIndex, CharacterTemplateID characterTemplateID)
            : this(dbController, GetFreeID(dbController), mapIndex, characterTemplateID, 1, new MapSpawnRect(null, null, null, null))
        {
            DBController.GetQuery<InsertMapSpawnQuery>().Execute(this);
        }

        /// <summary>
        /// MapSpawnValues constructor.
        /// </summary>
        /// <param name="dbController">The DBController used to synchronize changes to the values.</param>
        /// <param name="v">The SelectMapSpawnQueryValues containing the values to use.</param>
        MapSpawnValues(DBController dbController, SelectMapSpawnQueryValues v)
            : this(dbController, v.ID, v.MapIndex, v.CharacterTemplateID, v.Amount, v.MapSpawnRect)
        {
        }

        /// <summary>
        /// MapSpawnValues constructor.
        /// </summary>
        /// <param name="dbController">The DBController used to synchronize changes to the values.</param>
        /// <param name="id">The unique ID of this MapSpawnValues.</param>
        /// <param name="mapIndex">The index of the Map that these values are for.</param>
        /// <param name="characterTemplateID">The CharacterTemplateID of the CharacterTemplate to spawn.</param>
        /// <param name="spawnAmount">The maximum number of Characters that will be spawned by this MapSpawnValues.</param>
        /// <param name="spawnRect">The area on the map the spawning will take place at.</param>
        MapSpawnValues(DBController dbController, MapSpawnValuesID id, MapIndex mapIndex, CharacterTemplateID characterTemplateID,
                       byte spawnAmount, MapSpawnRect spawnRect)
        {
            _dbController = dbController;
            _id = id;
            _mapIndex = mapIndex;
            _characterTemplateID = characterTemplateID;
            _spawnAmount = spawnAmount;
            _spawnArea = spawnRect;
        }

        /// <summary>
        /// Deletes the MapSpawnValues from the database. After this is called, this MapSpawnValues must be treated
        /// as disposed and not be used at all!
        /// </summary>
        public void Delete()
        {
            if (DBController == null)
            {
                const string errmsg = "Called Delete() on `{0}` when the DBController was already null. Likely already deleted.";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, this);
                Debug.Fail(string.Format(errmsg, this));
                return;
            }

            if (log.IsInfoEnabled)
                log.InfoFormat("Deleting MapSpawnValues `{0}`.", this);

            var id = ID;
            DBController.GetQuery<DeleteMapSpawnQuery>().Execute(id);
            DBController.GetQuery<MapSpawnValuesIDCreator>().FreeID(id);

            _dbController = null;
        }

        /// <summary>
        /// Loads a MapSpawnValues from the database.
        /// </summary>
        /// <param name="dbController">DBController used to communicate with the database.</param>
        /// <param name="id">ID of the MapSpawnValues to load.</param>
        /// <returns>The MapSpawnValues with ID <paramref name="id"/>.</returns>
        public static MapSpawnValues Load(DBController dbController, MapSpawnValuesID id)
        {
            SelectMapSpawnQueryValues values = dbController.GetQuery<SelectMapSpawnQuery>().Execute(id);
            Debug.Assert(id == values.ID);
            return new MapSpawnValues(dbController, values);
        }

        /// <summary>
        /// Loads all of the MapSpawnValues for the given <paramref name="mapIndex"/> from the database.
        /// </summary>
        /// <param name="dbController">DBController used to communicate with the database.</param>
        /// <param name="mapIndex">Index of the map to load the MapSpawnValues for.</param>
        /// <returns>An IEnumerable of all of the MapSpawnValues for the given <paramref name="mapIndex"/>.</returns>
        public static IEnumerable<MapSpawnValues> Load(DBController dbController, MapIndex mapIndex)
        {
            var ret = new List<MapSpawnValues>();
            var queryValues = dbController.GetQuery<SelectMapSpawnsOnMapQuery>().Execute(mapIndex);

            foreach (SelectMapSpawnQueryValues v in queryValues)
            {
                Debug.Assert(v.MapIndex == mapIndex);
                ret.Add(new MapSpawnValues(dbController, v));
            }

            return ret;
        }

        /// <summary>
        /// Sets the spawn area for this MapSpawnValues.
        /// </summary>
        /// <param name="map">Instance of the Map with the MapIndex equal to the MapIndex handled by this MapSpawnValues.
        /// This is to ensure that the <paramref name="newSpawnArea"/> given is in a valid map range.</param>
        /// <param name="newSpawnArea">New MapSpawnRect values.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="newSpawnArea"/> contains one or more
        /// values that are not in range of the <paramref name="map"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="map"/>'s MapIndex does not match this
        /// MapSpawnValues's <see cref="MapIndex"/>.</exception>
        public void SetSpawnArea(MapBase map, MapSpawnRect newSpawnArea)
        {
            if (map.Index != MapIndex)
                throw new ArgumentException("The index of the specified map does not match this MapIndex", "map");

            if (newSpawnArea == SpawnArea)
                return;

            ushort x = newSpawnArea.X.HasValue ? newSpawnArea.X.Value : (ushort)0;
            ushort y = newSpawnArea.Y.HasValue ? newSpawnArea.Y.Value : (ushort)0;

            const string errmsg = "One or more of the `newSpawnArea` parameter values are out of range of the map!";

            if (x < 0)
                throw new ArgumentOutOfRangeException("newSpawnArea", errmsg);

            if (y < 0)
                throw new ArgumentOutOfRangeException("newSpawnArea", errmsg);

            if (newSpawnArea.Width.HasValue && (x + newSpawnArea.Width.Value) > map.Width)
                throw new ArgumentOutOfRangeException("newSpawnArea", errmsg);

            if (newSpawnArea.Height.HasValue && (y + newSpawnArea.Height.Value) > map.Height)
                throw new ArgumentOutOfRangeException("newSpawnArea", errmsg);

            SpawnArea = newSpawnArea;
        }

        /// <summary>
        /// Updates the MapSpawnValues in the database.
        /// </summary>
        void UpdateDB()
        {
            if (DBController == null)
            {
                const string errmsg = "Tried to call UpdateDB() on `{0}` when the DBController was null." +
                    " Likely means Delete() was called.";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, this);
                Debug.Fail(string.Format(errmsg, this));
                return;
            }

            if (log.IsInfoEnabled)
                log.InfoFormat("Updating MapSpawnValues `{0}`.", this);

            DBController.GetQuery<UpdateMapSpawnQuery>().Execute(this);
        }

        public override string ToString()
        {
            return string.Format("MapSpawnValues [ID: {0} Map: {1}]", ID, MapIndex);
        }
    }
}