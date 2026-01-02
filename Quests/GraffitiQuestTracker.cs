using MelonLoader;
using S1API.Graffiti;
using S1API.Quests.Constants;

namespace BigWillyMod.Quests
{
    /// <summary>
    /// Tracks graffiti completion for the Big Willy quest.
    /// </summary>
    public static class GraffitiQuestTracker
    {
        private static bool _subscribed = false;

        public static void Initialize()
        {
            if (_subscribed)
            {
                MelonLogger.Warning("[GraffitiQuestTracker] Already initialized, skipping...");
                return;
            }

            GraffitiEvents.GraffitiCompleted += OnGraffitiCompleted;
            _subscribed = true;
        }

        public static void Cleanup()
        {
            if (_subscribed)
            {
                GraffitiEvents.GraffitiCompleted -= OnGraffitiCompleted;
                _subscribed = false;
            }
        }

        private static void OnGraffitiCompleted(SpraySurface spraySurface)
        {
            try
            {
                if (spraySurface == null)
                {
                    MelonLogger.Warning("[GraffitiQuestTracker] SpraySurface is null");
                    return;
                }

                string surfaceGuid = spraySurface.GUID.ToString();
                var quest = QuestRegistry.GetBigWillyGraffitiQuest();
                if (quest == null)
                {
                    MelonLogger.Warning("[GraffitiQuestTracker] Quest not found - quest may not exist yet or not be created");
                    return;
                }

                if (quest.QuestEntries == null || quest.QuestEntries.Count == 0)
                {
                    MelonLogger.Warning("[GraffitiQuestTracker] Quest has no entries");
                    return;
                }

                var firstEntry = quest.QuestEntries[0];
                var entryState = firstEntry.State;
                if (entryState != QuestState.Active)
                    return;
                
                quest.RegisterTag(surfaceGuid);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GraffitiQuestTracker] Error in OnGraffitiCompleted: {ex.Message}");
                MelonLogger.Error($"[GraffitiQuestTracker] StackTrace: {ex.StackTrace}");
            }
        }
    }
}

