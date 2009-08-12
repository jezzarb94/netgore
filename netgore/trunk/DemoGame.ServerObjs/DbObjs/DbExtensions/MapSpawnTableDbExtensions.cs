using System;
using System.Linq;
using NetGore.Db;
using DemoGame.DbObjs;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Contains extension methods for class MapSpawnTable that assist in performing
/// reads and writes to and from a database.
/// </summary>
public static  class MapSpawnTableDbExtensions
{
/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void CopyValues(this IMapSpawnTable source, NetGore.Db.DbParameterValues paramValues)
{
paramValues["@amount"] = (System.Byte)source.Amount;
paramValues["@character_template_id"] = (DemoGame.Server.CharacterTemplateID)source.CharacterTemplateID;
paramValues["@height"] = (System.Nullable<System.UInt16>)source.Height;
paramValues["@id"] = (DemoGame.Server.MapSpawnValuesID)source.ID;
paramValues["@map_id"] = (NetGore.MapIndex)source.MapID;
paramValues["@width"] = (System.Nullable<System.UInt16>)source.Width;
paramValues["@x"] = (System.Nullable<System.UInt16>)source.X;
paramValues["@y"] = (System.Nullable<System.UInt16>)source.Y;
}

/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. The database column's name is used to as the key, so the value
/// will not be found if any aliases are used or not all columns were selected.
/// </summary>
/// <param name="source">The object to add the extension method to.</param>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public static void ReadValues(this MapSpawnTable source, System.Data.IDataReader dataReader)
{
System.Int32 i;

i = dataReader.GetOrdinal("amount");
source.Amount = (System.Byte)(System.Byte)dataReader.GetByte(i);

i = dataReader.GetOrdinal("character_template_id");
source.CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)(DemoGame.Server.CharacterTemplateID)dataReader.GetUInt16(i);

i = dataReader.GetOrdinal("height");
source.Height = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));

i = dataReader.GetOrdinal("id");
source.ID = (DemoGame.Server.MapSpawnValuesID)(DemoGame.Server.MapSpawnValuesID)dataReader.GetInt32(i);

i = dataReader.GetOrdinal("map_id");
source.MapID = (NetGore.MapIndex)(NetGore.MapIndex)dataReader.GetUInt16(i);

i = dataReader.GetOrdinal("width");
source.Width = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));

i = dataReader.GetOrdinal("x");
source.X = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));

i = dataReader.GetOrdinal("y");
source.Y = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));
}

/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. Unlike ReadValues(), this method not only doesn't require
/// all values to be in the IDataReader, but also does not require the values in
/// the IDataReader to be a defined field for the table this class represents.
/// Because of this, you need to be careful when using this method because values
/// can easily be skipped without any indication.
/// </summary>
/// <param name="source">The object to add the extension method to.</param>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public static void TryReadValues(this MapSpawnTable source, System.Data.IDataReader dataReader)
{
for (int i = 0; i < dataReader.FieldCount; i++)
{
switch (dataReader.GetName(i))
{
case "amount":
source.Amount = (System.Byte)(System.Byte)dataReader.GetByte(i);
break;


case "character_template_id":
source.CharacterTemplateID = (DemoGame.Server.CharacterTemplateID)(DemoGame.Server.CharacterTemplateID)dataReader.GetUInt16(i);
break;


case "height":
source.Height = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));
break;


case "id":
source.ID = (DemoGame.Server.MapSpawnValuesID)(DemoGame.Server.MapSpawnValuesID)dataReader.GetInt32(i);
break;


case "map_id":
source.MapID = (NetGore.MapIndex)(NetGore.MapIndex)dataReader.GetUInt16(i);
break;


case "width":
source.Width = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));
break;


case "x":
source.X = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));
break;


case "y":
source.Y = (System.Nullable<System.UInt16>)(System.Nullable<System.UInt16>)(dataReader.IsDBNull(i) ? (System.Nullable<System.UInt16>)null : dataReader.GetUInt16(i));
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
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void TryCopyValues(this IMapSpawnTable source, NetGore.Db.DbParameterValues paramValues)
{
for (int i = 0; i < paramValues.Count; i++)
{
switch (paramValues.GetParameterName(i))
{
case "@amount":
paramValues[i] = source.Amount;
break;


case "@character_template_id":
paramValues[i] = source.CharacterTemplateID;
break;


case "@height":
paramValues[i] = source.Height;
break;


case "@id":
paramValues[i] = source.ID;
break;


case "@map_id":
paramValues[i] = source.MapID;
break;


case "@width":
paramValues[i] = source.Width;
break;


case "@x":
paramValues[i] = source.X;
break;


case "@y":
paramValues[i] = source.Y;
break;


}

}
}

}

}
