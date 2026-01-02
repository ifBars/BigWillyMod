using S1API.Quests;
using MelonLoader;
using System.Collections.Generic;
using System.Reflection;

namespace BigWillyMod.Quests
{
    public static class QuestRegistry
    {
        private static BigWillyGraffitiQuest? _cachedQuest;

        public static BigWillyGraffitiQuest? GetBigWillyGraffitiQuest()
        {
            if (_cachedQuest != null)
            {
                if (QuestManagerQuests.Contains(_cachedQuest))
                {
                    return _cachedQuest;
                }
                _cachedQuest = null;
            }

            // Try to find existing quest by name first
            var questByName = QuestManager.GetQuestByName("Spread the Word");
            if (questByName is BigWillyGraffitiQuest foundByName)
            {
                _cachedQuest = foundByName;
                return _cachedQuest;
            }

            // Fallback: linear search across all quests
            for (int i = 0; i < QuestManagerQuests.Count; i++)
            {
                if (QuestManagerQuests[i] is BigWillyGraffitiQuest quest)
                {
                    _cachedQuest = quest;
                    return quest;
                }
            }
            
            return null;
        }

        public static BigWillyGraffitiQuest CreateBigWillyGraffitiQuest()
        {
            var existing = GetBigWillyGraffitiQuest();
            if (existing != null)
            {
                return existing;
            }
            
            var quest = QuestManager.CreateQuest<BigWillyGraffitiQuest>();
            if (quest is BigWillyGraffitiQuest bigWillyQuest)
            {
                _cachedQuest = bigWillyQuest;
                return bigWillyQuest;
            }
            
            MelonLogger.Error("[QuestRegistry] Failed to create BigWillyGraffitiQuest - wrong type returned");
            return null;
        }

        public static void ClearCache()
        {
            _cachedQuest = null;
        }

        private static List<Quest> QuestManagerQuests => (List<Quest>)typeof(QuestManager)
            .GetField("Quests", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null);
    }
}

