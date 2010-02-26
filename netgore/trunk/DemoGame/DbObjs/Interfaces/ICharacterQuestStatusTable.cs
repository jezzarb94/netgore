using System;
using System.Linq;
using NetGore.Features.Quests;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Interface for a class that can be used to serialize values to the database table `character_quest_status`.
    /// </summary>
    public interface ICharacterQuestStatusTable
    {
        /// <summary>
        /// Gets the value of the database column `character_id`.
        /// </summary>
        CharacterID CharacterID { get; }

        /// <summary>
        /// Gets the value of the database column `completed_on`.
        /// </summary>
        DateTime? CompletedOn { get; }

        /// <summary>
        /// Gets the value of the database column `quest_id`.
        /// </summary>
        QuestID QuestID { get; }

        /// <summary>
        /// Gets the value of the database column `started_on`.
        /// </summary>
        DateTime StartedOn { get; }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        ICharacterQuestStatusTable DeepCopy();
    }
}