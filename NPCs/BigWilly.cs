// ===============================================
// Schedule1ModdingTool generated NPC blueprint
// Mod: BigWillyMod v1.0.0 by Bars
// Game: TVGS - Schedule I
// ===============================================

using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using System;
using S1API.Entities.NPCs.Northtown;
using S1API.Products;
using S1API.Properties;
using S1API.Quests.Constants;
using S1API.Saveables;
using BigWillyMod.Quests;
using MelonLoader;
using UnityEngine;
using S1API.Graffiti;

namespace BigWillyMod.NPCs
{
    /// <summary>
    /// Auto-generated NPC blueprint for "Big Willy".
    /// Customize ConfigurePrefab and OnCreated to add unique logic.
    /// </summary>
    public sealed class BigWilly : NPC
    {
        public override bool IsPhysical => true;
        
        [Serializable]
        private class PersistedData
        {
            public bool QuestCompleted = false;
        }
        
        [SaveableField("BigWillyData")]
        private PersistedData _data = new PersistedData();

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var goblinHideBuilding = Building.Get<GoblinHideBuilding>();
            var arcade = Building.Get<Arcade>();
            var casino = Building.Get<Casino>();
            
            builder.WithIdentity("big_willy", "BigWilly", "")
            .WithAppearanceDefaults(av =>
            {
                av.Gender = 0f;
                av.Height = 1f;
                av.Weight = 1f;
                av.SkinColor = new Color32(223, 189, 161, 255);
                av.LeftEyeLidColor = new Color(0.875f, 0.741f, 0.631f);
                av.RightEyeLidColor = new Color(0.875f, 0.741f, 0.631f);
                av.EyeBallTint = new Color(1f, 0.655f, 0.655f);
                av.HairColor = new Color(0.122f, 0.075f, 0.043f);
                av.HairPath = "Avatar/Hair/bowlcut/BowlCut";
                av.EyeballMaterialIdentifier = "Default";
                av.PupilDilation = 1f;
                av.EyebrowScale = 1.169f;
                av.EyebrowThickness = 1.111f;
                av.EyebrowRestingHeight = 0.32419f;
                av.EyebrowRestingAngle = -10f;
                av.LeftEye = (0.39435f, 0.26935f);
                av.RightEye = (0.39435f, 0.26935f);
                av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", new Color(0f, 0f, 0f));
                av.WithBodyLayer("Avatar/Layers/Top/Overalls", new Color(0f, 0f, 0.502f));
                av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.863f, 0.078f, 0.235f));
                av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.149f, 0.149f, 0.149f));
                av.WithAccessoryLayer("Avatar/Accessories/Head/PorkpieHat/PorkpieHat", new Color(0.824f, 0.706f, 0.549f));
                av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.824f, 0.706f, 0.549f));
                av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0f, 0f, 0f));
            })
            .WithSpawnPosition(new Vector3(-35.7332f, -4.035f, 52.2295f))
            .EnsureCustomer()
            .WithCustomerDefaults(cd =>
            {
                cd.WithSpending(400f, 900f)
                  .WithOrdersPerWeek(1, 3)
                  .WithPreferredOrderDay(Day.Monday)
                  .WithOrderTime(900)
                  .WithStandards(CustomerStandard.Moderate)
                  .AllowDirectApproach(true)
                  .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                  .WithCallPoliceChance(0.15f)
                  .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1.1f)
                  .WithAffinities(new[]
                  {
                      (DrugType.Marijuana, 0.45f), (DrugType.Cocaine, -0.2f)
                  })
                  .WithPreferredProperties(Property.CalorieDense, Property.ThoughtProvoking, Property.Laxative);
            })
            .WithRelationshipDefaults(r =>
            {
                r.WithDelta(1.0f)
                    .SetUnlocked(false)
                    .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                    .WithConnections<KyleCooley, LudwigMeyer, AustinSteiner>();
            })
            .WithSchedule(plan =>
            {
                plan.EnsureDealSignal()
                    .StayInBuilding(goblinHideBuilding, 700, durationMinutes: 100)
                    // 9:00: Walk to pawnshop exterior
                    .WalkTo(new Vector3(-61.2776f, 1.065f, 55.6136f), 900)
                    // 9:30: Arrive at pawnshop, stand and look at direction (4 hours)
                    .WalkTo(new Vector3(-64.553f, 1.065f, 48.3866f), 1000, forward: Quaternion.Euler(0, 220, 0) * Vector3.forward)
                    // 13:30: Walk to arcade
                    .WalkTo(new Vector3(-44.2771f, -2.935f, 136.0889f), 1330)
                    // 14:00: Stay in arcade building (2 hours)
                    .StayInBuilding(arcade, 1400, durationMinutes: 100)
                    // 16:30: Gamble at slot machine until 23:00 (bet $10 per spin)
                    .UseSlotMachineUntilTime(1630, 2300, new Vector3(23.4776f, 1.8546f, 95.6571f), betAmount: 10, stopIfBroke: true)
                    // 23:00: Walk to sewer vent entrance
                    .WalkTo(new Vector3(93.5044f, -5.535f, 7.0331f), 2300)
                    // 23:00+: Stay in vent overnight (until next day 9:00)
                    .StayInBuilding(goblinHideBuilding, 2330, durationMinutes: 250);
            })
            .WithInventoryDefaults(inv =>
            {
                // Startup items that will always be in inventory when spawned
                inv.WithStartupItems("donut", "horsesemen", "megabean")
                    // Random cash between $500 and $5000
                    .WithRandomCash(min: 500, max: 5000)
                    // Preserve inventory across sleep cycles
                    .WithClearInventoryEachNight(false);
            });
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            
            // Sync quest completion state when loading
            // If quest exists and is completed, mark it in our saved state
            var quest = QuestRegistry.GetBigWillyGraffitiQuest();
            if (quest != null && quest.QuestState == QuestState.Completed)
            {
                _data.QuestCompleted = true;
            }
        }

        protected override void OnCreated()
        {
            base.OnCreated();
            Appearance.Build();
            Schedule.Enable();
            Region = Region.Northtown;
            
            SetupDialogue();
        }

        private void SetupDialogue()
        {
            try
            {
                // Check if quest was already completed (using saved field, as completed quests may be removed)
                // Also check current quest state as a fallback
                var quest = QuestRegistry.GetBigWillyGraffitiQuest();
                bool questCompleted = _data.QuestCompleted || (quest != null && quest.QuestState == QuestState.Completed);
                
                // Only set up quest dialogue if quest is not completed
                if (!questCompleted)
                {
                    Dialogue.BuildAndRegisterContainer("BigWillyQuestDialogue", c =>
                    {
                        // Entry node - checks quest state and routes accordingly
                        c.AddNode("ENTRY", GetEntryDialogueText(), ch =>
                        {
                            var currentQuest = QuestRegistry.GetBigWillyGraffitiQuest();
                            
                            // Check saved completion state first (quest may have been removed after completion)
                            if (_data.QuestCompleted)
                            {
                                // Quest completed - stop using quest dialogue container
                                ch.Add("OK", "Thanks for the help!", "EXIT");
                            }
                            else if (currentQuest == null || currentQuest.QuestState == QuestState.Inactive)
                            {
                                // Quest not started - offer quest or alternative reward based on available spots
                                int availableSpots = GraffitiManager.UntaggedSpraySurfaces.Count;
                                
                                if (availableSpots == 0)
                                {
                                    // No spots available - offer hat directly
                                    ch.Add("ACCEPT_HAT", "Sure, I'll take the hat!", "GIVE_HAT_DIRECTLY")
                                      .Add("DECLINE", "Not right now", "DECLINE_RESPONSE");
                                }
                                else
                                {
                                    // Spots available - offer quest
                                    ch.Add("ACCEPT_HELP", $"Yeah, I'll spread the word", "QUEST_ACCEPTED")
                                      .Add("DECLINE", "Not right now", "DECLINE_RESPONSE");
                                }
                            }
                            else if (currentQuest.QuestState == QuestState.Completed)
                            {
                                // Quest completed - mark in saved state and stop using quest dialogue container
                                _data.QuestCompleted = true;
                                RequestGameSave();
                                ch.Add("OK", "Thanks for the help!", "EXIT");
                            }
                            else if (currentQuest.QuestState == QuestState.Active)
                            {
                                if (currentQuest.IsReadyToTurnIn)
                                {
                                    // Ready to turn in
                                    ch.Add("TURN_IN", $"I've tagged {currentQuest.RequiredTagCount} {(currentQuest.RequiredTagCount == 1 ? "spot" : "spots")}!", "TURN_IN_REWARD");
                                }
                                else
                                {
                                    // In progress
                                    ch.Add("CHECK_PROGRESS", "How am I doing?", "PROGRESS_UPDATE")
                                      .Add("LEAVE", "I'll keep working on it", "LEAVE_RESPONSE");
                                }
                            }
                            else
                            {
                                // Quest failed or other state
                                ch.Add("OK", "Okay", "EXIT");
                            }
                        });
                        
                        // Quest accepted
                        c.AddNode("QUEST_ACCEPTED", GetQuestAcceptedText(), ch =>
                        {
                            ch.Add("GOT_IT", "Got it!", "EXIT");
                        });
                        
                        // Give hat directly (when no graffiti spots available)
                        c.AddNode("GIVE_HAT_DIRECTLY", "Stay silly, brother!", ch =>
                        {
                            ch.Add("THANKS", "Thanks!", "EXIT");
                        });
                        
                        // Decline response
                        c.AddNode("DECLINE_RESPONSE", "Alright, let me know if you change your mind!");
                        
                        // Progress update
                        c.AddNode("PROGRESS_UPDATE", GetProgressUpdateText());
                        
                        // Leave response
                        c.AddNode("LEAVE_RESPONSE", "Thanks for helping spread the word!");
                        
                        // Turn in reward
                        c.AddNode("TURN_IN_REWARD", "Great work brother! Spreading the word like a pro. Here, take this cap.", ch =>
                        {
                            ch.Add("THANKS", "Thanks!", "EXIT");
                        });
                        
                        // Exit node
                        c.AddNode("EXIT", "Stay silly, brother!");
                    });
                    
                    // Set up choice callbacks
                    Dialogue.OnChoiceSelected("ACCEPT_HELP", () =>
                    {
                        try
                        {
                            int availableSpots = GraffitiManager.UntaggedSpraySurfaces.Count;
                            
                            // Create quest with adjusted requirement based on available spots
                            var currentQuest = QuestRegistry.CreateBigWillyGraffitiQuest();
                            if (currentQuest != null)
                            {
                                // Set the required count based on available spots (min 1, max 5)
                                int requiredCount = Math.Min(Math.Max(availableSpots, 1), 5);
                                currentQuest.SetRequiredTagCount(requiredCount);
                                
                                if (currentQuest.QuestState != QuestState.Active)
                                {
                                    currentQuest.Begin();
                                }
                            }
                            Dialogue.JumpTo("BigWillyQuestDialogue", "QUEST_ACCEPTED");
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error($"[BigWilly] Failed to start quest: {ex.Message}");
                            Dialogue.JumpTo("BigWillyQuestDialogue", "EXIT");
                        }
                    });
                    
                    Dialogue.OnChoiceSelected("ACCEPT_HAT", () =>
                    {
                        try
                        {
                            // Give hat directly when no graffiti spots are available
                            Items.StaySillyCapCreator.Initialize();
                            
                            var capItem = S1API.Items.ItemManager.GetItemDefinition("stay_silly_cap");
                            if (capItem == null)
                            {
                                MelonLogger.Error("[BigWilly] Failed to find 'stay_silly_cap' item definition!");
                            }
                            else
                            {
                                S1API.Console.ConsoleHelper.AddItemToInventory("stay_silly_cap", 1);
                                
                                // Mark as completed so we don't offer this again
                                _data.QuestCompleted = true;
                                RequestGameSave();
                            }
                            
                            Dialogue.JumpTo("BigWillyQuestDialogue", "GIVE_HAT_DIRECTLY");
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error($"[BigWilly] Failed to give hat directly: {ex.Message}");
                            MelonLogger.Error(ex.StackTrace);
                            Dialogue.JumpTo("BigWillyQuestDialogue", "EXIT");
                        }
                    });
                    
                    Dialogue.OnChoiceSelected("TURN_IN", () =>
                    {
                        try
                        {
                            var currentQuest = QuestRegistry.GetBigWillyGraffitiQuest();
                            if (currentQuest != null && currentQuest.IsReadyToTurnIn)
                            {
                                // Complete the return entry first (this marks the entry as complete)
                                if (currentQuest.QuestEntries.Count >= 2)
                                {
                                    currentQuest.QuestEntries[1].Complete();
                                }
                                
                                // Give reward and complete quest AFTER showing reward dialogue
                                // Don't complete quest yet - let the reward dialogue show first
                                currentQuest.GiveReward();
                                
                                // Mark quest as completed in NPC's saved state (but don't complete quest yet)
                                // The quest will be completed after the player sees the reward dialogue
                                _data.QuestCompleted = true;
                                RequestGameSave();
                            }
                            // Jump to reward dialogue - quest completion happens after player acknowledges reward
                            Dialogue.JumpTo("BigWillyQuestDialogue", "TURN_IN_REWARD");
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error($"[BigWilly] Failed to complete quest: {ex.Message}");
                            Dialogue.JumpTo("BigWillyQuestDialogue", "EXIT");
                        }
                    });
                    
                    Dialogue.OnChoiceSelected("THANKS", () =>
                    {
                        try
                        {
                            // Complete the quest now that player has seen the reward dialogue
                            var currentQuest = QuestRegistry.GetBigWillyGraffitiQuest();
                            if (currentQuest != null && currentQuest.QuestState == QuestState.Active)
                                currentQuest.Complete();
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error($"[BigWilly] Failed to complete quest in THANKS handler: {ex.Message}");
                        }
                        
                        // Stop using quest dialogue container after completion
                        Dialogue.StopOverride();
                    });
                    
                    Dialogue.OnChoiceSelected("CHECK_PROGRESS", () =>
                    {
                        Dialogue.JumpTo("BigWillyQuestDialogue", "PROGRESS_UPDATE");
                    });
                    
                    Dialogue.OnChoiceSelected("LEAVE", () =>
                    {
                        Dialogue.JumpTo("BigWillyQuestDialogue", "LEAVE_RESPONSE");
                    });
                    
                    Dialogue.OnChoiceSelected("DECLINE", () =>
                    {
                        Dialogue.JumpTo("BigWillyQuestDialogue", "DECLINE_RESPONSE");
                    });
                    
                    Dialogue.OnChoiceSelected("GOT_IT", () =>
                    {
                        Dialogue.StopOverride();
                    });
                    
                    Dialogue.OnChoiceSelected("OK", () =>
                    {
                        // Only stop override if quest is truly completed AND we're in the completion dialogue flow
                        // Don't stop override if quest is active and ready to turn in - let the turn-in flow complete
                        var currentQuest = QuestRegistry.GetBigWillyGraffitiQuest();
                        bool questCompleted = _data.QuestCompleted || (currentQuest != null && currentQuest.QuestState == QuestState.Completed);
                        
                        // Only stop override if quest is completed AND not ready to turn in (prevents stopping during turn-in flow)
                        if (questCompleted && (currentQuest == null || !currentQuest.IsReadyToTurnIn))
                        {
                            // Mark as completed if not already marked (handles completion outside dialogue)
                            if (!_data.QuestCompleted)
                            {
                                _data.QuestCompleted = true;
                                RequestGameSave();
                            }
                            Dialogue.StopOverride();
                        }
                        else
                        {
                            Dialogue.JumpTo("BigWillyQuestDialogue", "EXIT");
                        }
                    });
                    
                    // Use this dialogue container when player interacts
                    Dialogue.UseContainerOnInteract("BigWillyQuestDialogue");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[BigWilly] Failed to setup dialogue: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }

        private string GetEntryDialogueText()
        {
            var quest = QuestRegistry.GetBigWillyGraffitiQuest();
            
            if (quest == null || quest.QuestState == QuestState.Inactive)
            {
                // Check if graffiti spots are available
                int availableSpots = GraffitiManager.UntaggedSpraySurfaces.Count;
                
                if (availableSpots == 0)
                {
                    return "Hey brother, have you heard of the Stay Silly merchandise? Here, take this Stay Silly hat!";
                }
                else
                {
                    return "Hey brother, I need your help spreading the word of the big willy business, think you can help me?";
                }
            }
            else if (quest.QuestState == QuestState.Active)
            {
                if (quest.IsReadyToTurnIn)
                {
                    return "Hey brother! I see you've been busy spreading the word. Great work!";
                }
                else
                {
                    return "Hey brother! How's the tagging going?";
                }
            }
            else if (quest.QuestState == QuestState.Completed)
            {
                return "Looking silly, brother! Thanks for the help.";
            }
            else
            {
                return "Hey brother, what's up?";
            }
        }
        
        private string GetQuestAcceptedText()
        {
            var quest = QuestRegistry.GetBigWillyGraffitiQuest();
            if (quest != null)
            {
                int count = quest.RequiredTagCount;
                return $"Great! Tag {count} {(count == 1 ? "spot" : "spots")} with graffiti for Big Willy. I'll know when you're done!";
            }
            return "Great! Tag some spots with graffiti for Big Willy. I'll know when you're done!";
        }
        
        private string GetProgressUpdateText()
        {
            var quest = QuestRegistry.GetBigWillyGraffitiQuest();
            if (quest != null)
            {
                int tagged = quest.TaggedCount;
                int remaining = quest.RequiredTagCount - tagged;
                
                if (remaining == 1)
                {
                    return $"Keep it up brother, you've tagged {tagged} {(tagged == 1 ? "spot" : "spots")}. Just one more to go!";
                }
                else
                {
                    return $"Keep it up brother, you've tagged {tagged} {(tagged == 1 ? "spot" : "spots")}. Just {remaining} more to go!";
                }
            }
            return "Keep it up brother! Just a few more to go!";
        }
    }
}
