using System;
using MelonLoader;
using S1API.Graffiti;
using S1API.Quests.Constants;
using BigWillyMod.Utils;

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
                DebugLog.Msg("[GraffitiQuestTracker] Already subscribed, skipping initialization");
                return;
            }

            DebugLog.Msg("[GraffitiQuestTracker] Subscribing to GraffitiCompleted event");
            GraffitiEvents.GraffitiCompleted += OnGraffitiCompleted;
            _subscribed = true;
            DebugLog.Msg("[GraffitiQuestTracker] Successfully subscribed to GraffitiCompleted event");
        }

        public static void Cleanup()
        {
            if (_subscribed)
            {
                DebugLog.Msg("[GraffitiQuestTracker] Unsubscribing from GraffitiCompleted event");
                GraffitiEvents.GraffitiCompleted -= OnGraffitiCompleted;
                _subscribed = false;
            }
        }

        private static void OnGraffitiCompleted(SpraySurface spraySurface)
        {
            try
            {
                DebugLog.Msg($"[GraffitiQuestTracker] OnGraffitiCompleted called!");
                
                if (spraySurface == null)
                {
                    DebugLog.Warning("[GraffitiQuestTracker] spraySurface is null");
                    return;
                }

                string surfaceGuid = spraySurface.GUID.ToString();
                DebugLog.Msg($"[GraffitiQuestTracker] Surface GUID: {surfaceGuid}");
                
                var quest = QuestRegistry.GetBigWillyGraffitiQuest();
                if (quest == null)
                {
                    DebugLog.Msg("[GraffitiQuestTracker] BigWillyGraffitiQuest not found in registry");
                    return;
                }
                
                DebugLog.Msg($"[GraffitiQuestTracker] Quest found, state: {quest.QuestState}");
                
                if (quest.QuestEntries == null || quest.QuestEntries.Count == 0)
                {
                    DebugLog.Warning("[GraffitiQuestTracker] Quest has no entries");
                    return;
                }

                DebugLog.Msg($"[GraffitiQuestTracker] Quest has {quest.QuestEntries.Count} entries");
                
                var firstEntry = quest.QuestEntries[0];
                DebugLog.Msg($"[GraffitiQuestTracker] First entry state: {firstEntry.State}");
                
                if (firstEntry.State != QuestState.Active)
                {
                    DebugLog.Msg($"[GraffitiQuestTracker] First entry not active, skipping tag registration");
                    return;
                }
                
                DebugLog.Msg($"[GraffitiQuestTracker] Calling quest.RegisterTag({surfaceGuid})");
                quest.RegisterTag(surfaceGuid);
                DebugLog.Msg($"[GraffitiQuestTracker] Tag registration complete");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GraffitiQuestTracker] Error in OnGraffitiCompleted: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }
    }
}

