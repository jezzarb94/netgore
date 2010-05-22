/********************************************************************
                   DO NOT MANUALLY EDIT THIS FILE!

This file was automatically generated using the DbClassCreator
program. The only time you should ever alter this file is if you are
using an automated code formatter. The DbClassCreator will overwrite
this file every time it is run, so all manual changes will be lost.
If there is something in this file that you wish to change, you should
be able to do it through the DbClassCreator arguments.

Make sure that you re-run the DbClassCreator every time you alter your
game's database.

For more information on the DbClassCreator, please see:
    http://www.netgore.com/wiki/dbclasscreator.html

This file was generated on (UTC): 5/21/2010 1:39:24 AM
********************************************************************/

using System;
using System.Linq;
using NetGore;
using NetGore.IO;
using System.Collections.Generic;
using System.Collections;
using NetGore.Db;
using DemoGame.DbObjs;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Provides a strongly-typed structure for the database table `world_stats_guild_user_change`.
/// </summary>
public class WorldStatsGuildUserChangeTable : IWorldStatsGuildUserChangeTable, NetGore.IO.IPersistable
{
/// <summary>
/// Array of the database column names.
/// </summary>
 static  readonly System.String[] _dbColumns = new string[] {"guild_id", "user_id", "when" };
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
 static  readonly System.String[] _dbColumnsNonKey = new string[] {"guild_id", "user_id", "when" };
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
public const System.String TableName = "world_stats_guild_user_change";
/// <summary>
/// The number of columns in the database table that this class represents.
/// </summary>
public const System.Int32 ColumnCount = 3;
/// <summary>
/// The field that maps onto the database column `guild_id`.
/// </summary>
System.Nullable<System.UInt16> _guildID;
/// <summary>
/// The field that maps onto the database column `user_id`.
/// </summary>
System.Int32 _userId;
/// <summary>
/// The field that maps onto the database column `when`.
/// </summary>
System.DateTime _when;
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `guild_id`.
/// The underlying database type is `smallint(5) unsigned`. The database column contains the comment: 
/// "The ID of the guild, or null if the user left a guild.".
/// </summary>
[System.ComponentModel.Description("The ID of the guild, or null if the user left a guild.")]
[NetGore.SyncValueAttribute()]
public System.Nullable<NetGore.Features.Guilds.GuildID> GuildID
{
get
{
return (System.Nullable<NetGore.Features.Guilds.GuildID>)_guildID;
}
set
{
this._guildID = (System.Nullable<System.UInt16>)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `user_id`.
/// The underlying database type is `int(11)`. The database column contains the comment: 
/// "The ID of the user who changed the guild they are part of.".
/// </summary>
[System.ComponentModel.Description("The ID of the user who changed the guild they are part of.")]
[NetGore.SyncValueAttribute()]
public DemoGame.CharacterID UserId
{
get
{
return (DemoGame.CharacterID)_userId;
}
set
{
this._userId = (System.Int32)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `when`.
/// The underlying database type is `timestamp` with the default value of `CURRENT_TIMESTAMP`. The database column contains the comment: 
/// "When this event took place.".
/// </summary>
[System.ComponentModel.Description("When this event took place.")]
[NetGore.SyncValueAttribute()]
public System.DateTime When
{
get
{
return (System.DateTime)_when;
}
set
{
this._when = (System.DateTime)value;
}
}

/// <summary>
/// Creates a deep copy of this table. All the values will be the same
/// but they will be contained in a different object instance.
/// </summary>
/// <returns>
/// A deep copy of this table.
/// </returns>
public IWorldStatsGuildUserChangeTable DeepCopy()
{
return new WorldStatsGuildUserChangeTable(this);
}
/// <summary>
/// WorldStatsGuildUserChangeTable constructor.
/// </summary>
public WorldStatsGuildUserChangeTable()
{
}
/// <summary>
/// WorldStatsGuildUserChangeTable constructor.
/// </summary>
/// <param name="guildID">The initial value for the corresponding property.</param>
/// <param name="userId">The initial value for the corresponding property.</param>
/// <param name="when">The initial value for the corresponding property.</param>
public WorldStatsGuildUserChangeTable(System.Nullable<NetGore.Features.Guilds.GuildID> @guildID, DemoGame.CharacterID @userId, System.DateTime @when)
{
this.GuildID = (System.Nullable<NetGore.Features.Guilds.GuildID>)@guildID;
this.UserId = (DemoGame.CharacterID)@userId;
this.When = (System.DateTime)@when;
}
/// <summary>
/// WorldStatsGuildUserChangeTable constructor.
/// </summary>
/// <param name="source">IWorldStatsGuildUserChangeTable to copy the initial values from.</param>
public WorldStatsGuildUserChangeTable(IWorldStatsGuildUserChangeTable source)
{
CopyValuesFrom(source);
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
public static void CopyValues(IWorldStatsGuildUserChangeTable source, System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
dic["@guild_id"] = (System.Nullable<NetGore.Features.Guilds.GuildID>)source.GuildID;
dic["@user_id"] = (DemoGame.CharacterID)source.UserId;
dic["@when"] = (System.DateTime)source.When;
}

/// <summary>
/// Copies the values from the given <paramref name="source"/> into this WorldStatsGuildUserChangeTable.
/// </summary>
/// <param name="source">The IWorldStatsGuildUserChangeTable to copy the values from.</param>
public void CopyValuesFrom(IWorldStatsGuildUserChangeTable source)
{
this.GuildID = (System.Nullable<NetGore.Features.Guilds.GuildID>)source.GuildID;
this.UserId = (DemoGame.CharacterID)source.UserId;
this.When = (System.DateTime)source.When;
}

/// <summary>
/// Gets the value of a column by the database column's name.
/// </summary>
/// <param name="columnName">The database name of the column to get the value for.</param>
/// <returns>
/// The value of the column with the name <paramref name="columnName"/>.
/// </returns>
public System.Object GetValue(System.String columnName)
{
switch (columnName)
{
case "guild_id":
return GuildID;

case "user_id":
return UserId;

case "when":
return When;

default:
throw new ArgumentException("Field not found.","columnName");
}
}

/// <summary>
/// Sets the <paramref name="value"/> of a column by the database column's name.
/// </summary>
/// <param name="columnName">The database name of the column to get the <paramref name="value"/> for.</param>
/// <param name="value">Value to assign to the column.</param>
public void SetValue(System.String columnName, System.Object value)
{
switch (columnName)
{
case "guild_id":
this.GuildID = (System.Nullable<NetGore.Features.Guilds.GuildID>)value;
break;

case "user_id":
this.UserId = (DemoGame.CharacterID)value;
break;

case "when":
this.When = (System.DateTime)value;
break;

default:
throw new ArgumentException("Field not found.","columnName");
}
}

/// <summary>
/// Gets the data for the database column that this table represents.
/// </summary>
/// <param name="columnName">The database name of the column to get the data for.</param>
/// <returns>
/// The data for the database column with the name <paramref name="columnName"/>.
/// </returns>
public static ColumnMetadata GetColumnData(System.String columnName)
{
switch (columnName)
{
case "guild_id":
return new ColumnMetadata("guild_id", "The ID of the guild, or null if the user left a guild.", "smallint(5) unsigned", null, typeof(System.Nullable<System.UInt16>), true, false, true);

case "user_id":
return new ColumnMetadata("user_id", "The ID of the user who changed the guild they are part of.", "int(11)", null, typeof(System.Int32), false, false, true);

case "when":
return new ColumnMetadata("when", "When this event took place.", "timestamp", "CURRENT_TIMESTAMP", typeof(System.DateTime), false, false, false);

default:
throw new ArgumentException("Field not found.","columnName");
}
}

/// <summary>
/// Reads the state of the object from an <see cref="IValueReader"/>.
/// </summary>
/// <param name="reader">The <see cref="IValueReader"/> to read the values from.</param>
public void ReadState(NetGore.IO.IValueReader reader)
{
NetGore.IO.PersistableHelper.Read(this, reader);
}

/// <summary>
/// Writes the state of the object to an <see cref="IValueWriter"/>.
/// </summary>
/// <param name="writer">The <see cref="IValueWriter"/> to write the values to.</param>
public void WriteState(NetGore.IO.IValueWriter writer)
{
NetGore.IO.PersistableHelper.Write(this, writer);
}

}

}