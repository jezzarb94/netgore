using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DemoGame.DbObjs;
using NetGore;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `item_template`.
    /// </summary>
    public class ItemTemplateTable : IItemTemplateTable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 31;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "item_template";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[]
                                              {
                                                  "agi", "armor", "bra", "defence", "description", "dex", "evade", "graphic",
                                                  "height", "hp", "id", "imm", "int", "maxhit", "maxhp", "maxmp", "minhit", "mp",
                                                  "name", "perc", "reqacc", "reqagi", "reqarmor", "reqbra", "reqdex", "reqevade",
                                                  "reqimm", "reqint", "type", "value", "width"
                                              };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[]
                                                    {
                                                        "agi", "armor", "bra", "defence", "description", "dex", "evade", "graphic",
                                                        "height", "hp", "imm", "int", "maxhit", "maxhp", "maxmp", "minhit", "mp",
                                                        "name", "perc", "reqacc", "reqagi", "reqarmor", "reqbra", "reqdex",
                                                        "reqevade", "reqimm", "reqint", "type", "value", "width"
                                                    };

        /// <summary>
        /// The fields that are used in the column collection `ReqStat`.
        /// </summary>
        static readonly String[] _reqStatColumns = new string[]
                                                   {
                                                       "reqacc", "reqagi", "reqarmor", "reqbra", "reqdex", "reqevade", "reqimm",
                                                       "reqint"
                                                   };

        /// <summary>
        /// The fields that are used in the column collection `Stat`.
        /// </summary>
        static readonly String[] _statColumns = new string[]
                                                {
                                                    "agi", "armor", "bra", "defence", "dex", "evade", "imm", "int", "maxhit",
                                                    "maxhp", "maxmp", "minhit", "perc"
                                                };

        /// <summary>
        /// Dictionary containing the values for the column collection `ReqStat`.
        /// </summary>
        readonly ReqStatConstDictionary _reqStat = new ReqStatConstDictionary();

        /// <summary>
        /// Dictionary containing the values for the column collection `Stat`.
        /// </summary>
        readonly StatConstDictionary _stat = new StatConstDictionary();

        /// <summary>
        /// The field that maps onto the database column `description`.
        /// </summary>
        String _description;

        /// <summary>
        /// The field that maps onto the database column `graphic`.
        /// </summary>
        UInt16 _graphic;

        /// <summary>
        /// The field that maps onto the database column `height`.
        /// </summary>
        Byte _height;

        /// <summary>
        /// The field that maps onto the database column `hp`.
        /// </summary>
        Int16 _hP;

        /// <summary>
        /// The field that maps onto the database column `id`.
        /// </summary>
        UInt16 _iD;

        /// <summary>
        /// The field that maps onto the database column `mp`.
        /// </summary>
        Int16 _mP;

        /// <summary>
        /// The field that maps onto the database column `name`.
        /// </summary>
        String _name;

        /// <summary>
        /// The field that maps onto the database column `type`.
        /// </summary>
        Byte _type;

        /// <summary>
        /// The field that maps onto the database column `value`.
        /// </summary>
        Int32 _value;

        /// <summary>
        /// The field that maps onto the database column `width`.
        /// </summary>
        Byte _width;

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
        /// </summary>
        public static IEnumerable<String> DbColumns
        {
            get { return _dbColumns; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns that are primary keys.
        /// </summary>
        public static IEnumerable<String> DbKeyColumns
        {
            get { return _dbColumnsKeys; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns that are not primary keys.
        /// </summary>
        public static IEnumerable<String> DbNonKeyColumns
        {
            get { return _dbColumnsNonKey; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the name of the database
        /// columns used in the column collection `ReqStat`.
        /// </summary>
        public static IEnumerable<String> ReqStatColumns
        {
            get { return _reqStatColumns; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the name of the database
        /// columns used in the column collection `Stat`.
        /// </summary>
        public static IEnumerable<String> StatColumns
        {
            get { return _statColumns; }
        }

        /// <summary>
        /// ItemTemplateTable constructor.
        /// </summary>
        public ItemTemplateTable()
        {
        }

        /// <summary>
        /// ItemTemplateTable constructor.
        /// </summary>
        /// <param name="agi">The initial value for the corresponding property.</param>
        /// <param name="armor">The initial value for the corresponding property.</param>
        /// <param name="bra">The initial value for the corresponding property.</param>
        /// <param name="defence">The initial value for the corresponding property.</param>
        /// <param name="description">The initial value for the corresponding property.</param>
        /// <param name="dex">The initial value for the corresponding property.</param>
        /// <param name="evade">The initial value for the corresponding property.</param>
        /// <param name="graphic">The initial value for the corresponding property.</param>
        /// <param name="height">The initial value for the corresponding property.</param>
        /// <param name="hP">The initial value for the corresponding property.</param>
        /// <param name="iD">The initial value for the corresponding property.</param>
        /// <param name="imm">The initial value for the corresponding property.</param>
        /// <param name="int">The initial value for the corresponding property.</param>
        /// <param name="maxHit">The initial value for the corresponding property.</param>
        /// <param name="maxHP">The initial value for the corresponding property.</param>
        /// <param name="maxMP">The initial value for the corresponding property.</param>
        /// <param name="minHit">The initial value for the corresponding property.</param>
        /// <param name="mP">The initial value for the corresponding property.</param>
        /// <param name="name">The initial value for the corresponding property.</param>
        /// <param name="perc">The initial value for the corresponding property.</param>
        /// <param name="reqacc">The initial value for the corresponding property.</param>
        /// <param name="reqagi">The initial value for the corresponding property.</param>
        /// <param name="reqarmor">The initial value for the corresponding property.</param>
        /// <param name="reqbra">The initial value for the corresponding property.</param>
        /// <param name="reqdex">The initial value for the corresponding property.</param>
        /// <param name="reqevade">The initial value for the corresponding property.</param>
        /// <param name="reqimm">The initial value for the corresponding property.</param>
        /// <param name="reqint">The initial value for the corresponding property.</param>
        /// <param name="type">The initial value for the corresponding property.</param>
        /// <param name="value">The initial value for the corresponding property.</param>
        /// <param name="width">The initial value for the corresponding property.</param>
        public ItemTemplateTable(Int16 @agi, Int16 @armor, Int16 @bra, Int16 @defence, String @description, Int16 @dex,
                                 Int16 @evade, GrhIndex @graphic, Byte @height, SPValueType @hP, ItemTemplateID @iD, Int16 @imm,
                                 Int16 @int, Int16 @maxHit, Int16 @maxHP, Int16 @maxMP, Int16 @minHit, SPValueType @mP,
                                 String @name, Int16 @perc, Byte @reqacc, Byte @reqagi, Byte @reqarmor, Byte @reqbra, Byte @reqdex,
                                 Byte @reqevade, Byte @reqimm, Byte @reqint, Byte @type, Int32 @value, Byte @width)
        {
            SetStat(StatType.Agi, @agi);
            SetStat(StatType.Armor, @armor);
            SetStat(StatType.Bra, @bra);
            SetStat(StatType.Defence, @defence);
            Description = @description;
            SetStat(StatType.Dex, @dex);
            SetStat(StatType.Evade, @evade);
            Graphic = @graphic;
            Height = @height;
            HP = @hP;
            ID = @iD;
            SetStat(StatType.Imm, @imm);
            SetStat(StatType.Int, @int);
            SetStat(StatType.MaxHit, @maxHit);
            SetStat(StatType.MaxHP, @maxHP);
            SetStat(StatType.MaxMP, @maxMP);
            SetStat(StatType.MinHit, @minHit);
            MP = @mP;
            Name = @name;
            SetStat(StatType.Perc, @perc);
            SetReqStat(StatType.Acc, @reqacc);
            SetReqStat(StatType.Agi, @reqagi);
            SetReqStat(StatType.Armor, @reqarmor);
            SetReqStat(StatType.Bra, @reqbra);
            SetReqStat(StatType.Dex, @reqdex);
            SetReqStat(StatType.Evade, @reqevade);
            SetReqStat(StatType.Imm, @reqimm);
            SetReqStat(StatType.Int, @reqint);
            Type = @type;
            Value = @value;
            Width = @width;
        }

        /// <summary>
        /// ItemTemplateTable constructor.
        /// </summary>
        /// <param name="source">IItemTemplateTable to copy the initial values from.</param>
        public ItemTemplateTable(IItemTemplateTable source)
        {
            CopyValuesFrom(source);
        }

        /// <summary>
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public static void CopyValues(IItemTemplateTable source, IDictionary<String, Object> dic)
        {
            dic["@agi"] = (Int16)source.GetStat(StatType.Agi);
            dic["@armor"] = (Int16)source.GetStat(StatType.Armor);
            dic["@bra"] = (Int16)source.GetStat(StatType.Bra);
            dic["@defence"] = (Int16)source.GetStat(StatType.Defence);
            dic["@description"] = source.Description;
            dic["@dex"] = (Int16)source.GetStat(StatType.Dex);
            dic["@evade"] = (Int16)source.GetStat(StatType.Evade);
            dic["@graphic"] = source.Graphic;
            dic["@height"] = source.Height;
            dic["@hp"] = source.HP;
            dic["@id"] = source.ID;
            dic["@imm"] = (Int16)source.GetStat(StatType.Imm);
            dic["@int"] = (Int16)source.GetStat(StatType.Int);
            dic["@maxhit"] = (Int16)source.GetStat(StatType.MaxHit);
            dic["@maxhp"] = (Int16)source.GetStat(StatType.MaxHP);
            dic["@maxmp"] = (Int16)source.GetStat(StatType.MaxMP);
            dic["@minhit"] = (Int16)source.GetStat(StatType.MinHit);
            dic["@mp"] = source.MP;
            dic["@name"] = source.Name;
            dic["@perc"] = (Int16)source.GetStat(StatType.Perc);
            dic["@reqacc"] = (Byte)source.GetReqStat(StatType.Acc);
            dic["@reqagi"] = (Byte)source.GetReqStat(StatType.Agi);
            dic["@reqarmor"] = (Byte)source.GetReqStat(StatType.Armor);
            dic["@reqbra"] = (Byte)source.GetReqStat(StatType.Bra);
            dic["@reqdex"] = (Byte)source.GetReqStat(StatType.Dex);
            dic["@reqevade"] = (Byte)source.GetReqStat(StatType.Evade);
            dic["@reqimm"] = (Byte)source.GetReqStat(StatType.Imm);
            dic["@reqint"] = (Byte)source.GetReqStat(StatType.Int);
            dic["@type"] = source.Type;
            dic["@value"] = source.Value;
            dic["@width"] = source.Width;
        }

        /// <summary>
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public void CopyValues(IDictionary<String, Object> dic)
        {
            CopyValues(this, dic);
        }

        /// <summary>
        /// Copies the values from the given <paramref name="source"/> into this ItemTemplateTable.
        /// </summary>
        /// <param name="source">The IItemTemplateTable to copy the values from.</param>
        public void CopyValuesFrom(IItemTemplateTable source)
        {
            SetStat(StatType.Agi, source.GetStat(StatType.Agi));
            SetStat(StatType.Armor, source.GetStat(StatType.Armor));
            SetStat(StatType.Bra, source.GetStat(StatType.Bra));
            SetStat(StatType.Defence, source.GetStat(StatType.Defence));
            Description = source.Description;
            SetStat(StatType.Dex, source.GetStat(StatType.Dex));
            SetStat(StatType.Evade, source.GetStat(StatType.Evade));
            Graphic = source.Graphic;
            Height = source.Height;
            HP = source.HP;
            ID = source.ID;
            SetStat(StatType.Imm, source.GetStat(StatType.Imm));
            SetStat(StatType.Int, source.GetStat(StatType.Int));
            SetStat(StatType.MaxHit, source.GetStat(StatType.MaxHit));
            SetStat(StatType.MaxHP, source.GetStat(StatType.MaxHP));
            SetStat(StatType.MaxMP, source.GetStat(StatType.MaxMP));
            SetStat(StatType.MinHit, source.GetStat(StatType.MinHit));
            MP = source.MP;
            Name = source.Name;
            SetStat(StatType.Perc, source.GetStat(StatType.Perc));
            SetReqStat(StatType.Acc, source.GetReqStat(StatType.Acc));
            SetReqStat(StatType.Agi, source.GetReqStat(StatType.Agi));
            SetReqStat(StatType.Armor, source.GetReqStat(StatType.Armor));
            SetReqStat(StatType.Bra, source.GetReqStat(StatType.Bra));
            SetReqStat(StatType.Dex, source.GetReqStat(StatType.Dex));
            SetReqStat(StatType.Evade, source.GetReqStat(StatType.Evade));
            SetReqStat(StatType.Imm, source.GetReqStat(StatType.Imm));
            SetReqStat(StatType.Int, source.GetReqStat(StatType.Int));
            Type = source.Type;
            Value = source.Value;
            Width = source.Width;
        }

        /// <summary>
        /// Gets the data for the database column that this table represents.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the data for.</param>
        /// <returns>
        /// The data for the database column with the name <paramref name="columnName"/>.
        /// </returns>
        public static ColumnMetadata GetColumnData(String columnName)
        {
            switch (columnName)
            {
                case "agi":
                    return new ColumnMetadata("agi", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "armor":
                    return new ColumnMetadata("armor", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "bra":
                    return new ColumnMetadata("bra", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "defence":
                    return new ColumnMetadata("defence", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "description":
                    return new ColumnMetadata("description", "", "varchar(255)", null, typeof(String), false, false, false);

                case "dex":
                    return new ColumnMetadata("dex", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "evade":
                    return new ColumnMetadata("evade", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "graphic":
                    return new ColumnMetadata("graphic", "", "smallint(5) unsigned", null, typeof(UInt16), false, false, false);

                case "height":
                    return new ColumnMetadata("height", "", "tinyint(3) unsigned", null, typeof(Byte), false, false, false);

                case "hp":
                    return new ColumnMetadata("hp", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "id":
                    return new ColumnMetadata("id", "", "smallint(5) unsigned", null, typeof(UInt16), false, true, false);

                case "imm":
                    return new ColumnMetadata("imm", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "int":
                    return new ColumnMetadata("int", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "maxhit":
                    return new ColumnMetadata("maxhit", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "maxhp":
                    return new ColumnMetadata("maxhp", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "maxmp":
                    return new ColumnMetadata("maxmp", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "minhit":
                    return new ColumnMetadata("minhit", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "mp":
                    return new ColumnMetadata("mp", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "name":
                    return new ColumnMetadata("name", "", "varchar(255)", null, typeof(String), false, false, false);

                case "perc":
                    return new ColumnMetadata("perc", "", "smallint(6)", "0", typeof(Int16), false, false, false);

                case "reqacc":
                    return new ColumnMetadata("reqacc", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqagi":
                    return new ColumnMetadata("reqagi", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqarmor":
                    return new ColumnMetadata("reqarmor", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqbra":
                    return new ColumnMetadata("reqbra", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqdex":
                    return new ColumnMetadata("reqdex", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqevade":
                    return new ColumnMetadata("reqevade", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqimm":
                    return new ColumnMetadata("reqimm", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "reqint":
                    return new ColumnMetadata("reqint", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "type":
                    return new ColumnMetadata("type", "", "tinyint(3) unsigned", "0", typeof(Byte), false, false, false);

                case "value":
                    return new ColumnMetadata("value", "", "int(11)", "0", typeof(Int32), false, false, false);

                case "width":
                    return new ColumnMetadata("width", "", "tinyint(3) unsigned", null, typeof(Byte), false, false, false);

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        /// <summary>
        /// Gets the value of a column by the database column's name.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the value for.</param>
        /// <returns>
        /// The value of the column with the name <paramref name="columnName"/>.
        /// </returns>
        public Object GetValue(String columnName)
        {
            switch (columnName)
            {
                case "agi":
                    return GetStat(StatType.Agi);

                case "armor":
                    return GetStat(StatType.Armor);

                case "bra":
                    return GetStat(StatType.Bra);

                case "defence":
                    return GetStat(StatType.Defence);

                case "description":
                    return Description;

                case "dex":
                    return GetStat(StatType.Dex);

                case "evade":
                    return GetStat(StatType.Evade);

                case "graphic":
                    return Graphic;

                case "height":
                    return Height;

                case "hp":
                    return HP;

                case "id":
                    return ID;

                case "imm":
                    return GetStat(StatType.Imm);

                case "int":
                    return GetStat(StatType.Int);

                case "maxhit":
                    return GetStat(StatType.MaxHit);

                case "maxhp":
                    return GetStat(StatType.MaxHP);

                case "maxmp":
                    return GetStat(StatType.MaxMP);

                case "minhit":
                    return GetStat(StatType.MinHit);

                case "mp":
                    return MP;

                case "name":
                    return Name;

                case "perc":
                    return GetStat(StatType.Perc);

                case "reqacc":
                    return GetReqStat(StatType.Acc);

                case "reqagi":
                    return GetReqStat(StatType.Agi);

                case "reqarmor":
                    return GetReqStat(StatType.Armor);

                case "reqbra":
                    return GetReqStat(StatType.Bra);

                case "reqdex":
                    return GetReqStat(StatType.Dex);

                case "reqevade":
                    return GetReqStat(StatType.Evade);

                case "reqimm":
                    return GetReqStat(StatType.Imm);

                case "reqint":
                    return GetReqStat(StatType.Int);

                case "type":
                    return Type;

                case "value":
                    return Value;

                case "width":
                    return Width;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        /// <summary>
        /// Gets the <paramref name="value"/> of a database column for the corresponding <paramref name="key"/> for the column collection `ReqStat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <param name="value">The value to assign to the column for the corresponding <paramref name="key"/>.</param>
        public void SetReqStat(StatType key, Int32 value)
        {
            _reqStat[key] = (Byte)value;
        }

        /// <summary>
        /// Gets the <paramref name="value"/> of a database column for the corresponding <paramref name="key"/> for the column collection `Stat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <param name="value">The value to assign to the column for the corresponding <paramref name="key"/>.</param>
        public void SetStat(StatType key, Int32 value)
        {
            _stat[key] = (Int16)value;
        }

        /// <summary>
        /// Sets the <paramref name="value"/> of a column by the database column's name.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the <paramref name="value"/> for.</param>
        /// <param name="value">Value to assign to the column.</param>
        public void SetValue(String columnName, Object value)
        {
            switch (columnName)
            {
                case "agi":
                    SetStat(StatType.Agi, (Int32)value);
                    break;

                case "armor":
                    SetStat(StatType.Armor, (Int32)value);
                    break;

                case "bra":
                    SetStat(StatType.Bra, (Int32)value);
                    break;

                case "defence":
                    SetStat(StatType.Defence, (Int32)value);
                    break;

                case "description":
                    Description = (String)value;
                    break;

                case "dex":
                    SetStat(StatType.Dex, (Int32)value);
                    break;

                case "evade":
                    SetStat(StatType.Evade, (Int32)value);
                    break;

                case "graphic":
                    Graphic = (GrhIndex)value;
                    break;

                case "height":
                    Height = (Byte)value;
                    break;

                case "hp":
                    HP = (SPValueType)value;
                    break;

                case "id":
                    ID = (ItemTemplateID)value;
                    break;

                case "imm":
                    SetStat(StatType.Imm, (Int32)value);
                    break;

                case "int":
                    SetStat(StatType.Int, (Int32)value);
                    break;

                case "maxhit":
                    SetStat(StatType.MaxHit, (Int32)value);
                    break;

                case "maxhp":
                    SetStat(StatType.MaxHP, (Int32)value);
                    break;

                case "maxmp":
                    SetStat(StatType.MaxMP, (Int32)value);
                    break;

                case "minhit":
                    SetStat(StatType.MinHit, (Int32)value);
                    break;

                case "mp":
                    MP = (SPValueType)value;
                    break;

                case "name":
                    Name = (String)value;
                    break;

                case "perc":
                    SetStat(StatType.Perc, (Int32)value);
                    break;

                case "reqacc":
                    SetReqStat(StatType.Acc, (Int32)value);
                    break;

                case "reqagi":
                    SetReqStat(StatType.Agi, (Int32)value);
                    break;

                case "reqarmor":
                    SetReqStat(StatType.Armor, (Int32)value);
                    break;

                case "reqbra":
                    SetReqStat(StatType.Bra, (Int32)value);
                    break;

                case "reqdex":
                    SetReqStat(StatType.Dex, (Int32)value);
                    break;

                case "reqevade":
                    SetReqStat(StatType.Evade, (Int32)value);
                    break;

                case "reqimm":
                    SetReqStat(StatType.Imm, (Int32)value);
                    break;

                case "reqint":
                    SetReqStat(StatType.Int, (Int32)value);
                    break;

                case "type":
                    Type = (Byte)value;
                    break;

                case "value":
                    Value = (Int32)value;
                    break;

                case "width":
                    Width = (Byte)value;
                    break;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        #region IItemTemplateTable Members

        /// <summary>
        /// Gets an IEnumerable of KeyValuePairs containing the values in the `Stat` collection. The
        /// key is the collection's key and the value is the value for that corresponding key.
        /// </summary>
        public IEnumerable<KeyValuePair<StatType, Int32>> Stats
        {
            get { return _stat; }
        }

        /// <summary>
        /// Gets an IEnumerable of KeyValuePairs containing the values in the `ReqStat` collection. The
        /// key is the collection's key and the value is the value for that corresponding key.
        /// </summary>
        public IEnumerable<KeyValuePair<StatType, Int32>> ReqStats
        {
            get { return _reqStat; }
        }

        /// <summary>
        /// Gets the value of a database column for the corresponding <paramref name="key"/> for the column collection `Stat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <returns>
        /// The value of the database column for the corresponding <paramref name="key"/>.
        /// </returns>
        public Int32 GetStat(StatType key)
        {
            return (Int16)_stat[key];
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `description`.
        /// The underlying database type is `varchar(255)`.
        /// </summary>
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `graphic`.
        /// The underlying database type is `smallint(5) unsigned`.
        /// </summary>
        public GrhIndex Graphic
        {
            get { return (GrhIndex)_graphic; }
            set { _graphic = (UInt16)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `height`.
        /// The underlying database type is `tinyint(3) unsigned`.
        /// </summary>
        public Byte Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `hp`.
        /// The underlying database type is `smallint(6)` with the default value of `0`.
        /// </summary>
        public SPValueType HP
        {
            get { return _hP; }
            set { _hP = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `id`.
        /// The underlying database type is `smallint(5) unsigned`.
        /// </summary>
        public ItemTemplateID ID
        {
            get { return (ItemTemplateID)_iD; }
            set { _iD = (UInt16)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `mp`.
        /// The underlying database type is `smallint(6)` with the default value of `0`.
        /// </summary>
        public SPValueType MP
        {
            get { return _mP; }
            set { _mP = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `name`.
        /// The underlying database type is `varchar(255)`.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the value of a database column for the corresponding <paramref name="key"/> for the column collection `ReqStat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <returns>
        /// The value of the database column for the corresponding <paramref name="key"/>.
        /// </returns>
        public Int32 GetReqStat(StatType key)
        {
            return (Byte)_reqStat[key];
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `type`.
        /// The underlying database type is `tinyint(3) unsigned` with the default value of `0`.
        /// </summary>
        public Byte Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `value`.
        /// The underlying database type is `int(11)` with the default value of `0`.
        /// </summary>
        public Int32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `width`.
        /// The underlying database type is `tinyint(3) unsigned`.
        /// </summary>
        public Byte Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public IItemTemplateTable DeepCopy()
        {
            return new ItemTemplateTable(this);
        }

        #endregion

        /// <summary>
        /// A Dictionary-like lookup table for the Enum values of the type collection `ReqStat` for the
        /// table that this class represents. Majority of the code for this class was automatically generated and
        /// only other automatically generated code should be using this class.
        /// </summary>
        class ReqStatConstDictionary : IEnumerable<KeyValuePair<StatType, Int32>>
        {
            /// <summary>
            /// Array that maps the integer value of key type to the index of the _values array.
            /// </summary>
            static readonly Int32[] _lookupTable;

            /// <summary>
            /// Array containing the actual values. The index of this array is found through the value returned
            /// from the _lookupTable.
            /// </summary>
            readonly Int32[] _values;

            /// <summary>
            /// Gets or sets an item's value using the <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The key for the value to get or set.</param>
            /// <returns>The item's value for the corresponding <paramref name="key"/>.</returns>
            public Int32 this[StatType key]
            {
                get { return _values[_lookupTable[(Int32)key]]; }
                set { _values[_lookupTable[(Int32)key]] = value; }
            }

            /// <summary>
            /// ReqStatConstDictionary static constructor.
            /// </summary>
            static ReqStatConstDictionary()
            {
                var asArray = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();
                _lookupTable = new Int32[asArray.Length];

                for (Int32 i = 0; i < _lookupTable.Length; i++)
                {
                    _lookupTable[i] = (Int32)asArray[i];
                }
            }

            /// <summary>
            /// ReqStatConstDictionary constructor.
            /// </summary>
            public ReqStatConstDictionary()
            {
                _values = new Int32[_lookupTable.Length];
            }

            #region IEnumerable<KeyValuePair<StatType,int>> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<KeyValuePair<StatType, Int32>> GetEnumerator()
            {
                for (int i = 0; i < _values.Length; i++)
                {
                    yield return new KeyValuePair<StatType, Int32>((StatType)i, _values[i]);
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        /// <summary>
        /// A Dictionary-like lookup table for the Enum values of the type collection `Stat` for the
        /// table that this class represents. Majority of the code for this class was automatically generated and
        /// only other automatically generated code should be using this class.
        /// </summary>
        class StatConstDictionary : IEnumerable<KeyValuePair<StatType, Int32>>
        {
            /// <summary>
            /// Array that maps the integer value of key type to the index of the _values array.
            /// </summary>
            static readonly Int32[] _lookupTable;

            /// <summary>
            /// Array containing the actual values. The index of this array is found through the value returned
            /// from the _lookupTable.
            /// </summary>
            readonly Int32[] _values;

            /// <summary>
            /// Gets or sets an item's value using the <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The key for the value to get or set.</param>
            /// <returns>The item's value for the corresponding <paramref name="key"/>.</returns>
            public Int32 this[StatType key]
            {
                get { return _values[_lookupTable[(Int32)key]]; }
                set { _values[_lookupTable[(Int32)key]] = value; }
            }

            /// <summary>
            /// StatConstDictionary static constructor.
            /// </summary>
            static StatConstDictionary()
            {
                var asArray = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();
                _lookupTable = new Int32[asArray.Length];

                for (Int32 i = 0; i < _lookupTable.Length; i++)
                {
                    _lookupTable[i] = (Int32)asArray[i];
                }
            }

            /// <summary>
            /// StatConstDictionary constructor.
            /// </summary>
            public StatConstDictionary()
            {
                _values = new Int32[_lookupTable.Length];
            }

            #region IEnumerable<KeyValuePair<StatType,int>> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<KeyValuePair<StatType, Int32>> GetEnumerator()
            {
                for (int i = 0; i < _values.Length; i++)
                {
                    yield return new KeyValuePair<StatType, Int32>((StatType)i, _values[i]);
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }
    }
}