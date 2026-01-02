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
                return;

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
                    return;

                string surfaceGuid = spraySurface.GUID.ToString();
                var quest = QuestRegistry.GetBigWillyGraffitiQuest();
                if (quest == null || quest.QuestEntries == null || quest.QuestEntries.Count == 0)
                    return;

                var firstEntry = quest.QuestEntries[0];
                if (firstEntry.State != QuestState.Active)
                    return;
                
                quest.RegisterTag(surfaceGuid);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GraffitiQuestTracker] Error in OnGraffitiCompleted: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }
    }
}

