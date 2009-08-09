using System;
using System.Linq;
using NetGore.Db;
using DemoGame.DbObjs;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Provides a strongly-typed structure for the database table `character_template_equipped`.
/// </summary>
public class CharacterTemplateEquippedTable : ICharacterTemplateEquippedTable
{
/// <summary>
/// Array of the database column names.
/// </summary>
 static  readonly System.String[] _dbColumns = new string[] {"chance", "character_template_id", "item_template_id" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
/// </summary>
public static System.Collections.Generic.IEnumerable<System.String> DbColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumns;
}
}
/// <summary>
/// Array of the database column names for columns that are primary keys.
/// </summary>
 static  readonly System.String[] _dbColumnsKeys = new string[] { };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns that are primary keys.
/// </summary>
public static System.Collections.Generic.IEnumerable<System.String> DbKeyColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumnsKeys;
}
}
/// <summary>
/// Array of the database column names for columns that are not primary keys.
/// </summary>
 static  readonly System.String[] _dbColumnsNonKey = new string[] {"chance", "character_template_id", "item_template_id" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns that are not primary keys.
/// </summary>
public static System.Collections.Generic.IEnumerable<System.String> DbNonKeyColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumnsNonKey;
}
}
/// <summary>
/// The name of the database table that this class represents.
/// </summary>
public const System.String TableName = "character_template_equipped";
/// <summary>
/// The number of columns in the database table that this class represents.
/// </summary>
public const System.Int32 ColumnCount = 3;
/// <summary>
/// The field that maps onto the database column `chance`.
/// </summary>
System.UInt16 _chance;
/// <summary>
/// The field that maps onto the database column `character_template_id`.
/// </summary>
System.UInt16 _characterTemplateID;
/// <summary>
/// The field that maps onto the database column `item_template_id`.
/// </summary>
System.UInt16 _itemTemplateID;
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `chance`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public DemoGame.Server.ItemChance Chance
{
get
{
return (DemoGame.Server.ItemChance)_chance;
}
set
{
this._chance = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `character_template_id`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public DemoGame.Server.CharacterTemplateID CharacterTemplateID
{
get
{
return (DemoGame.Server.CharacterTemplateID)_characterTemplateID;
}
set
{
this._characterTemplateID = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `item_template_id`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public DemoGame.Server.ItemTemplateID ItemTemplateID
{
get
{
return (DemoGame.Server.ItemTemplateID)_itemTemplateID;
}
set
{
this._itemTemplateID = (System.UInt16)value;
}
}

/// <summary>
/// CharacterTemplateEquippedTable constructor.
/// </summary>
public CharacterTemplateEquippedTable()
{
}
/// <summary>
/// CharacterTemplateEquippedTable constructor.
/// </summary>
/// <param name="chance">The initial value for the corresponding property.</param>
/// <param name="characterTemplateID">The initial value for the corresponding property.</param>
/// <param name="itemTemplateID">The initial value for the corresponding property.</param>
public CharacterTemplateEquippedTable(DemoGame.Server.ItemChance @chance, DemoGame.Server.CharacterTemplateID @characterTemplateID, DemoGame.Server.ItemTemplateID @itemTemplateID)
{
Chance = (DemoGame.Server.ItemChance)@chance;
CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)@characterTemplateID;
ItemTemplateID = (DemoGame.Server.ItemTemplateID)@itemTemplateID;
}
/// <summary>
/// CharacterTemplateEquippedTable constructor.
/// </summary>
/// <param name="dataReader">The IDataReader to read the values from. See method ReadValues() for details.</param>
public CharacterTemplateEquippedTable(System.Data.IDataReader dataReader)
{
ReadValues(dataReader);
}
public CharacterTemplateEquippedTable(ICharacterTemplateEquippedTable source)
{
CopyValuesFrom(source);
}
/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. The database column's name is used to as the key, so the value
/// will not be found if any aliases are used or not all columns were selected.
/// </summary>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public void ReadValues(System.Data.IDataReader dataReader)
{
System.Int32 i;

i = dataReader.GetOrdinal("chance");
Chance = (DemoGame.Server.ItemChance)(DemoGame.Server.ItemChance)dataReader.GetUInt16(i);

i = dataReader.GetOrdinal("character_template_id");
CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)(DemoGame.Server.CharacterTemplateID)dataReader.GetUInt16(i);

i = dataReader.GetOrdinal("item_template_id");
ItemTemplateID = (DemoGame.Server.ItemTemplateID)(DemoGame.Server.ItemTemplateID)dataReader.GetUInt16(i);
}

/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
/// this method will not create them if they are missing.
/// </summary>
/// <param name="dic">The Dictionary to copy the values into.</param>
public void CopyValues(System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
CopyValues(this, dic);
}
/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
/// this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="dic">The Dictionary to copy the values into.</param>
public static void CopyValues(ICharacterTemplateEquippedTable source, System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
dic["@chance"] = (DemoGame.Server.ItemChance)source.Chance;
dic["@character_template_id"] = (DemoGame.Server.CharacterTemplateID)source.CharacterTemplateID;
dic["@item_template_id"] = (DemoGame.Server.ItemTemplateID)source.ItemTemplateID;
}

/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public void CopyValues(NetGore.Db.DbParameterValues paramValues)
{
CopyValues(this, paramValues);
}
/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void CopyValues(ICharacterTemplateEquippedTable source, NetGore.Db.DbParameterValues paramValues)
{
paramValues["@chance"] = (DemoGame.Server.ItemChance)source.Chance;
paramValues["@character_template_id"] = (DemoGame.Server.CharacterTemplateID)source.CharacterTemplateID;
paramValues["@item_template_id"] = (DemoGame.Server.ItemTemplateID)source.ItemTemplateID;
}

public void CopyValuesFrom(ICharacterTemplateEquippedTable source)
{
Chance = (DemoGame.Server.ItemChance)source.Chance;
CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)source.CharacterTemplateID;
ItemTemplateID = (DemoGame.Server.ItemTemplateID)source.ItemTemplateID;
}

public System.Object GetValue(System.String columnName)
{
switch (columnName)
{
case "chance":
return Chance;

case "character_template_id":
return CharacterTemplateID;

case "item_template_id":
return ItemTemplateID;

default:
throw new ArgumentException("Field not found.","columnName");
}
}

public void SetValue(System.String columnName, System.Object value)
{
switch (columnName)
{
case "chance":
Chance = (DemoGame.Server.ItemChance)value;
break;

case "character_template_id":
CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)value;
break;

case "item_template_id":
ItemTemplateID = (DemoGame.Server.ItemTemplateID)value;
break;

default:
throw new ArgumentException("Field not found.","columnName");
}
}

public static ColumnMetadata GetColumnData(System.String fieldName)
{
switch (fieldName)
{
case "chance":
return new ColumnMetadata("chance", "", "smallint(5) unsigned", null, typeof(System.UInt16), false, false, false);

case "character_template_id":
return new ColumnMetadata("character_template_id", "", "smallint(5) unsigned", null, typeof(System.UInt16), false, false, true);

case "item_template_id":
return new ColumnMetadata("item_template_id", "", "smallint(5) unsigned", null, typeof(System.UInt16), false, false, true);

default:
throw new ArgumentException("Field not found.","fieldName");
}
}

/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. Unlike ReadValues(), this method not only doesn't require
/// all values to be in the IDataReader, but also does not require the values in
/// the IDataReader to be a defined field for the table this class represents.
/// Because of this, you need to be careful when using this method because values
/// can easily be skipped without any indication.
/// </summary>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public void TryReadValues(System.Data.IDataReader dataReader)
{
for (int i = 0; i < dataReader.FieldCount; i++)
{
switch (dataReader.GetName(i))
{
case "chance":
Chance = (DemoGame.Server.ItemChance)(DemoGame.Server.ItemChance)dataReader.GetUInt16(i);
break;


case "character_template_id":
CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)(DemoGame.Server.CharacterTemplateID)dataReader.GetUInt16(i);
break;


case "item_template_id":
ItemTemplateID = (DemoGame.Server.ItemTemplateID)(DemoGame.Server.ItemTemplateID)dataReader.GetUInt16(i);
break;


}

}
}

/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The key must already exist in the DbParameterValues
/// for the value to be copied over. If any of the keys in the DbParameterValues do not
/// match one of the column names, or if there is no field for a key, then it will be
/// ignored. Because of this, it is important to be careful when using this method
/// since columns or keys can be skipped without any indication.
/// </summary>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public void TryCopyValues(NetGore.Db.DbParameterValues paramValues)
{
TryCopyValues(this, paramValues);
}
/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The key must already exist in the DbParameterValues
/// for the value to be copied over. If any of the keys in the DbParameterValues do not
/// match one of the column names, or if there is no field for a key, then it will be
/// ignored. Because of this, it is important to be careful when using this method
/// since columns or keys can be skipped without any indication.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void TryCopyValues(ICharacterTemplateEquippedTable source, NetGore.Db.DbParameterValues paramValues)
{
for (int i = 0; i < paramValues.Count; i++)
{
switch (paramValues.GetParameterName(i))
{
case "@chance":
paramValues[i] = source.Chance;
break;


case "@character_template_id":
paramValues[i] = source.CharacterTemplateID;
break;


case "@item_template_id":
paramValues[i] = source.ItemTemplateID;
break;


}

}
}

}

}
