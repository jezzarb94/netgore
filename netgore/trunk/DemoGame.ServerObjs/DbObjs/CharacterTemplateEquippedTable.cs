using System;
using System.Linq;
using NetGore.Db;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Interface for a class that can be used to serialize values to the database table `character_template_equipped`.
/// </summary>
public interface ICharacterTemplateEquippedTable
{
/// <summary>
/// Gets the value for the database column `chance`.
/// </summary>
System.UInt16 Chance
{
get;
}
/// <summary>
/// Gets the value for the database column `character_id`.
/// </summary>
DemoGame.Server.CharacterID CharacterId
{
get;
}
/// <summary>
/// Gets the value for the database column `item_id`.
/// </summary>
DemoGame.Server.ItemID ItemId
{
get;
}
}

/// <summary>
/// Provides a strongly-typed structure for the database table `character_template_equipped`.
/// </summary>
public class CharacterTemplateEquippedTable : ICharacterTemplateEquippedTable
{
/// <summary>
/// Array of the database column names.
/// </summary>
 static  readonly System.String[] _dbColumns = new string[] {"chance", "character_id", "item_id" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
/// </summary>
public System.Collections.Generic.IEnumerable<System.String> DbColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumns;
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
/// The field that maps onto the database column `character_id`.
/// </summary>
System.UInt16 _characterId;
/// <summary>
/// The field that maps onto the database column `item_id`.
/// </summary>
System.UInt16 _itemId;
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `chance`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public System.UInt16 Chance
{
get
{
return (System.UInt16)_chance;
}
set
{
this._chance = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `character_id`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public DemoGame.Server.CharacterID CharacterId
{
get
{
return (DemoGame.Server.CharacterID)_characterId;
}
set
{
this._characterId = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `item_id`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public DemoGame.Server.ItemID ItemId
{
get
{
return (DemoGame.Server.ItemID)_itemId;
}
set
{
this._itemId = (System.UInt16)value;
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
/// <param name="characterId">The initial value for the corresponding property.</param>
/// <param name="itemId">The initial value for the corresponding property.</param>
public CharacterTemplateEquippedTable(System.UInt16 @chance, DemoGame.Server.CharacterID @characterId, DemoGame.Server.ItemID @itemId)
{
Chance = (System.UInt16)@chance;
CharacterId = (DemoGame.Server.CharacterID)@characterId;
ItemId = (DemoGame.Server.ItemID)@itemId;
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
Chance = (System.UInt16)(System.UInt16)dataReader.GetUInt16(dataReader.GetOrdinal("chance"));
CharacterId = (DemoGame.Server.CharacterID)(DemoGame.Server.CharacterID)dataReader.GetUInt16(dataReader.GetOrdinal("character_id"));
ItemId = (DemoGame.Server.ItemID)(DemoGame.Server.ItemID)dataReader.GetUInt16(dataReader.GetOrdinal("item_id"));
}

/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="dic">The Dictionary to copy the values into.</param>
public void CopyValues(System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
CopyValues(this, dic);
}
/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="dic">The Dictionary to copy the values into.</param>
public static void CopyValues(ICharacterTemplateEquippedTable source, System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
dic["@chance"] = (System.UInt16)source.Chance;
dic["@character_id"] = (DemoGame.Server.CharacterID)source.CharacterId;
dic["@item_id"] = (DemoGame.Server.ItemID)source.ItemId;
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
paramValues["@chance"] = (System.UInt16)source.Chance;
paramValues["@character_id"] = (DemoGame.Server.CharacterID)source.CharacterId;
paramValues["@item_id"] = (DemoGame.Server.ItemID)source.ItemId;
}

public void CopyValuesFrom(ICharacterTemplateEquippedTable source)
{
Chance = (System.UInt16)source.Chance;
CharacterId = (DemoGame.Server.CharacterID)source.CharacterId;
ItemId = (DemoGame.Server.ItemID)source.ItemId;
}

public System.Object GetValue(System.String columnName)
{
switch (columnName)
{
case "chance":
return Chance;
case "character_id":
return CharacterId;
case "item_id":
return ItemId;
default:
throw new ArgumentException("Field not found.","columnName");
}
}

public void SetValue(System.String columnName, System.Object value)
{
switch (columnName)
{
case "chance":
Chance = (System.UInt16)value;
break;
case "character_id":
CharacterId = (DemoGame.Server.CharacterID)value;
break;
case "item_id":
ItemId = (DemoGame.Server.ItemID)value;
break;
default:
throw new ArgumentException("Field not found.","columnName");
}
}

}

}
