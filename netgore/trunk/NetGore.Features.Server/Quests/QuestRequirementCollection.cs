using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NetGore.Features.Quests
{
    public class QuestRequirementCollection<TCharacter> : IQuestRequirementCollection<TCharacter> where TCharacter : DynamicEntity
    {
        static readonly IEnumerable<IQuestRequirement<TCharacter>> _emptyQuestRequirements = new IQuestRequirement<TCharacter>[0];

        readonly IEnumerable<IQuestRequirement<TCharacter>> _questRequirements;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestRequirementCollection{TCharacter}"/> class.
        /// </summary>
        /// <param name="questRequirements">The quest requirements.</param>
        public QuestRequirementCollection(IEnumerable<IQuestRequirement<TCharacter>> questRequirements)
        {
            if (questRequirements == null || questRequirements.Count() == 0)
                _questRequirements = _emptyQuestRequirements;
            else
                _questRequirements = questRequirements.ToCompact();
        }

        /// <summary>
        /// Checks if the <paramref name="character"/> meets this test requirement.
        /// </summary>
        /// <param name="character">The character to check if they meet the requirements.</param>
        /// <returns>True if the <paramref name="character"/> meets the requirements defined by this
        /// <see cref="IQuestRequirement{TCharacter}"/>; otherwise false.</returns>
        public bool HasRequirements(TCharacter character)
        {
            return this.All(x => x.HasRequirements(character));
        }

        /// <summary>
        /// Takes the quest requirements from the <paramref name="character"/>, if applicable. Not required,
        /// and only applies for when turning in a quest and not starting a quest.
        /// </summary>
        /// <param name="character">The <paramref name="character"/> to take the requirements from.</param>
        public void TakeRequirements(TCharacter character)
        {
            foreach (var req in this)
                req.TakeRequirements(character);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IQuestRequirement<TCharacter>> GetEnumerator()
        {
            return _questRequirements.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}