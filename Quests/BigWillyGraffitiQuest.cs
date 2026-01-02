using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using S1API.Console;
using S1API.Entities;
using S1API.Graffiti;
using S1API.Internal.Abstraction;
using S1API.Items;
using S1API.Quests;
using S1API.Quests.Constants;
using S1API.Saveables;
using BigWillyMod.NPCs;
using UnityEngine;

namespace BigWillyMod.Quests
{
    public class BigWillyGraffitiQuest : Quest
    {
        private const string QUEST_ID = "big_willy_graffiti_quest";
        
        [SaveableField("taggedSurfaces")]
        private HashSet<string> _taggedSurfaceIds = new HashSet<string>();
        
        [SaveableField("rewardGranted")]
        private bool _rewardGranted = false;
        
        [SaveableField("requiredTagCount")]
        private int _requiredTagCount = 5;
        
        private QuestEntry _tagEntry;
        private QuestEntry _returnEntry;
        
        /// <summary>
        /// Sets the required number of tags for this quest.
        /// Must be called before the quest is started.
        /// </summary>
        public void SetRequiredTagCount(int count)
        {
            if (count <= 0)
            {
                MelonLogger.Warning("[BigWillyGraffitiQuest] Required tag count must be greater than 0, using 1");
                _requiredTagCount = 1;
            }
            else
            {
                _requiredTagCount = count;
            }
        }
        
        /// <summary>
        /// Gets the required number of tags for this quest.
        /// </summary>
        public int RequiredTagCount => _requiredTagCount;

        protected override string Title => "Spread the Word";
        
        protected override string Description => 
            $"Big Willy needs your help spreading the word about his business! " +
            $"Tag {_requiredTagCount} {(_requiredTagCount == 1 ? "spot" : "different spots")} around town with graffiti to help promote Big Willy's enterprise. " +
            $"Once you've completed all {_requiredTagCount} {(_requiredTagCount == 1 ? "tag" : "tags")}, return to Big Willy to claim your reward.";
        
        protected override bool AutoBegin => false;

        /// <summary>
        /// Called when the quest is created (Unity Start method).
        /// For LOADED quests: OnLoaded() runs first, then OnCreated(). QuestEntries will already be populated.
        /// For NEW quests: Only OnCreated() runs. QuestEntries will be empty.
        /// Check QuestEntries.Count to avoid creating duplicate entries.
        /// </summary>
        protected override void OnCreated()
        {
            base.OnCreated();

            // Only create entries if they haven't been created yet (avoids duplicates for loaded quests)
            if (QuestEntries.Count == 0)
            {
                // Entry 1: Tag spots for Big Willy
                _tagEntry = AddEntry($"Tag {_requiredTagCount} {(_requiredTagCount == 1 ? "spot" : "spots")} for Big Willy ({_taggedSurfaceIds.Count}/{_requiredTagCount})");
                _tagEntry.Begin();
                
                // Update POI to nearest untagged surface (wait a frame for GraffitiManager to initialize)
                MelonCoroutines.Start(UpdatePOIToNearestUntaggedSurfaceDelayed());

                // Entry 2: Return to Big Willy (initially hidden)
                // Create entry without NPC first, then wait for NPC to spawn and attach it
                _returnEntry = AddEntry("Return to Big Willy");
                _returnEntry.SetState(QuestState.Inactive);
                
                // Wait for Big Willy NPC to spawn before attaching it to the entry
                MelonCoroutines.Start(WaitForBigWillyAndAttachToEntry());
            }
            else
            {
                // Restore quest entries from existing list (loaded quests)
                if (QuestEntries.Count >= 1)
                {
                    _tagEntry = QuestEntries[0];
                }
                if (QuestEntries.Count >= 2)
                {
                    _returnEntry = QuestEntries[1];
                }
                
                // Update counter text to reflect current progress
                UpdateTagEntryText();
                
                // Update POI to nearest untagged surface if tag entry is still active
                if (_tagEntry != null && _tagEntry.State == QuestState.Active)
                {
                    MelonCoroutines.Start(UpdatePOIToNearestUntaggedSurfaceDelayed());
                }
                
                // Try to attach Big Willy NPC to return entry if it wasn't attached before
                MelonCoroutines.Start(WaitForBigWillyAndAttachToEntry());
                
                // After save system restores states, check if we need to update entry states based on progress
                // This handles cases where quest was loaded with progress already made
                if (_taggedSurfaceIds.Count >= _requiredTagCount)
                {
                    // Mark tag entry as complete if all tags are done
                    if (_tagEntry != null && _tagEntry.State != QuestState.Completed)
                    {
                        _tagEntry.Complete();
                    }
                    
                    // Show return entry if quest is ready to turn in
                    if (_returnEntry != null && _returnEntry.State == QuestState.Inactive)
                    {
                        _returnEntry.SetState(QuestState.Active);
                        _returnEntry.Begin();
                    }
                }
            }

            // Subscribe to quest completion event (only once, in OnCreated)
            OnComplete += HandleQuestComplete;
        }
        
        private void HandleQuestComplete()
        {
            // Ensure reward is given if not already granted
            if (!_rewardGranted)
            {
                GiveReward();
            }
            
            // Request game save to persist completion state
            RequestGameSave();
        }

        /// <summary>
        /// Called after quest data has been loaded from save files.
        /// For loaded quests, this runs BEFORE OnCreated(), so we create entries here.
        /// The game's save system will restore the entry states after we create them.
        /// IMPORTANT: Do NOT call .Begin() or .SetState() here - let the save system restore states.
        /// Note: Do not subscribe to triggers here - that is handled in OnCreated() to avoid duplicate subscriptions.
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            // Create quest entries if they don't exist yet
            // This ensures loaded quests have their entries created before OnCreated() runs
            if (QuestEntries.Count == 0)
            {
                // Entry 1: Tag spots for Big Willy
                _tagEntry = AddEntry($"Tag {_requiredTagCount} {(_requiredTagCount == 1 ? "spot" : "spots")} for Big Willy ({_taggedSurfaceIds.Count}/{_requiredTagCount})");
                // State will be restored from save data by S1API

                // Entry 2: Return to Big Willy
                // Create entry without NPC first, then wait for NPC to spawn and attach it
                _returnEntry = AddEntry("Return to Big Willy");
                // State will be restored from save data by S1API
                
                // Wait for Big Willy NPC to spawn before attaching it to the entry
                MelonCoroutines.Start(WaitForBigWillyAndAttachToEntry());
            }
            else
            {
                // Restore quest entries from saved state
                if (QuestEntries.Count >= 1)
                {
                    _tagEntry = QuestEntries[0];
                }
                if (QuestEntries.Count >= 2)
                {
                    _returnEntry = QuestEntries[1];
                }
                
                // Try to attach Big Willy NPC to return entry if it wasn't attached before
                MelonCoroutines.Start(WaitForBigWillyAndAttachToEntry());
            }
        }

        public void RegisterTag(string surfaceGuid)
        {
            if (string.IsNullOrEmpty(surfaceGuid))
            {
                MelonLogger.Warning("[BigWillyGraffitiQuest] Attempted to register tag with null/empty GUID");
                return;
            }
            
            if (_taggedSurfaceIds.Contains(surfaceGuid))
                return;
            
            _taggedSurfaceIds.Add(surfaceGuid);
            UpdateTagEntryText();
            RequestGameSave();
            SendProgressTextMessage();
            
            // Update POI to nearest untagged surface after tagging
            if (_tagEntry != null && _tagEntry.State == QuestState.Active)
            {
                UpdatePOIToNearestUntaggedSurface();
            }
            
            if (_taggedSurfaceIds.Count >= _requiredTagCount)
            {
                if (_tagEntry != null && _tagEntry.State != QuestState.Completed)
                {
                    _tagEntry.Complete();
                }
                else if (_tagEntry == null)
                {
                    MelonLogger.Warning("[BigWillyGraffitiQuest] _tagEntry is null, cannot complete");
                }

                _returnEntry.SetState(QuestState.Active);
                _returnEntry.Begin();

                RequestGameSave();
            }
        }

        private void UpdateTagEntryText()
        {
            if (_tagEntry != null)
            {
                _tagEntry.Title = $"Tag {_requiredTagCount} {(_requiredTagCount == 1 ? "spot" : "spots")} for Big Willy ({_taggedSurfaceIds.Count}/{_requiredTagCount})";
            }
        }

        public void GiveReward()
        {
            if (_rewardGranted)
            {
                MelonLogger.Warning("[BigWillyGraffitiQuest] Reward already granted, skipping.");
                return;
            }
            
            try
            {
                Items.StaySillyCapCreator.Initialize();
                
                var capItem = ItemManager.GetItemDefinition("stay_silly_cap");
                if (capItem == null)
                {
                    MelonLogger.Error("[BigWillyGraffitiQuest] Failed to find 'stay_silly_cap' item definition!");
                    return;
                }
                
                ConsoleHelper.AddItemToInventory("stay_silly_cap", 1);
                _rewardGranted = true;
                
                RequestGameSave();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[BigWillyGraffitiQuest] Failed to give reward: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }

        private void SendProgressTextMessage()
        {
            try
            {
                var bigWilly = NPC.Get<BigWilly>();
                if (bigWilly == null)
                {
                    MelonLogger.Warning("[BigWillyGraffitiQuest] Big Willy NPC not found, cannot send text message");
                    return;
                }
                
                string message;
                if (_taggedSurfaceIds.Count >= _requiredTagCount)
                {
                    message = $"Brother! You've tagged all {_requiredTagCount} {(_requiredTagCount == 1 ? "spot" : "spots")}! Come see me and I'll give you something special!";
                }
                else
                {
                    int remaining = _requiredTagCount - _taggedSurfaceIds.Count;
                    message = $"Nice work brother! You've tagged {_taggedSurfaceIds.Count} spot{(_taggedSurfaceIds.Count == 1 ? "" : "s")}. Just {remaining} more to go!";
                }
                
                bigWilly.SendTextMessage(message);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[BigWillyGraffitiQuest] Failed to send progress text message: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// Coroutine that waits for Big Willy NPC to spawn before attaching it to the return entry.
        /// This prevents NPC lookup failures when quests are loaded before NPCs are initialized.
        /// </summary>
        private IEnumerator WaitForBigWillyAndAttachToEntry()
        {
            float timeout = 10f;
            float waited = 0f;
            float checkInterval = 1.0f;
            
            while (waited < timeout)
            {
                var bigWilly = NPC.Get<BigWilly>();
                if (bigWilly != null)
                {
                    if (_returnEntry != null)
                        _returnEntry.SetPOIToNPC(bigWilly);
                    
                    yield break;
                }
                
                yield return new WaitForSeconds(checkInterval);
                waited += checkInterval;
            }

            MelonLogger.Warning("[BigWillyGraffitiQuest] Timeout waiting for Big Willy NPC to spawn. Quest entry created without NPC attachment.");
        }

        /// <summary>
        /// Coroutine that waits a frame before updating POI to ensure GraffitiManager is initialized.
        /// </summary>
        private IEnumerator UpdatePOIToNearestUntaggedSurfaceDelayed()
        {
            // Wait a frame to ensure GraffitiManager and spray surfaces are initialized
            yield return null;
            UpdatePOIToNearestUntaggedSurface();
        }

        /// <summary>
        /// Updates the tag entry's POI to point to the nearest untagged spray surface.
        /// </summary>
        private void UpdatePOIToNearestUntaggedSurface()
        {
            if (_tagEntry == null)
                return;

            try
            {
                // Get player position
                var player = Player.Local;
                if (player == null)
                {
                    MelonLogger.Warning("[BigWillyGraffitiQuest] Player.Local is null, cannot update POI");
                    return;
                }

                // Find nearest untagged surface
                var nearestSurface = GraffitiManager.FindNearestUntaggedSurface(player.Position);
                if (nearestSurface != null)
                {
                    // Filter out surfaces that have already been tagged
                    string surfaceGuid = nearestSurface.GUID.ToString();
                    if (!_taggedSurfaceIds.Contains(surfaceGuid))
                    {
                        _tagEntry.SetPOIToSpraySurface(nearestSurface);
                    }
                    else
                    {
                        // This surface was already tagged, try to find another one
                        // Get all untagged surfaces and filter out tagged ones
                        var untaggedSurfaces = GraffitiManager.GetUntaggedSpraySurfaces()
                            .Where(s => !_taggedSurfaceIds.Contains(s.GUID.ToString()))
                            .ToList();

                        if (untaggedSurfaces.Count > 0)
                        {
                            // Find nearest from filtered list
                            SpraySurface? nextNearest = null;
                            float nearestDistance = float.MaxValue;

                            foreach (var surface in untaggedSurfaces)
                            {
                                float distance = Vector3.Distance(player.Position, surface.Position);
                                if (distance < nearestDistance)
                                {
                                    nearestDistance = distance;
                                    nextNearest = surface;
                                }
                            }

                            if (nextNearest != null)
                                _tagEntry.SetPOIToSpraySurface(nextNearest);
                        }
                        else
                        {
                            MelonLogger.Warning("[BigWillyGraffitiQuest] No untagged surfaces found to point to");
                        }
                    }
                }
                else
                {
                    MelonLogger.Warning("[BigWillyGraffitiQuest] No untagged surfaces found in the game");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[BigWillyGraffitiQuest] Failed to update POI to nearest untagged surface: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }

        public int TaggedCount => _taggedSurfaceIds.Count;
        
        public bool IsReadyToTurnIn => _taggedSurfaceIds.Count >= _requiredTagCount;
        
        /// <summary>
        /// Gets the current quest state. Public accessor for the protected QuestState property.
        /// </summary>
        public new QuestState QuestState => base.QuestState;
    }
}

