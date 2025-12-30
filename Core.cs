using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using S1API.Entities;
using S1API.GameTime;
using S1API.Lifecycle;
using BigWillyMod.Items;
using BigWillyMod.Quests;
using BigWillyMod.Utils;

[assembly: MelonInfo(typeof(BigWillyMod.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace BigWillyMod
{
    public class Core : MelonMod
    {
        public static Core? Instance { get; private set; }
        
        private bool _itemsInitialized = false;
        private bool _shopsInitialized = false;

        public override void OnLateInitializeMelon()
        {
            Instance = this;
            
            // Initialize Harmony patches for graffiti tracking
            GraffitiQuestTracker.Initialize();
            
            // Register lifecycle event for item creation
            GameLifecycle.OnPreLoad += OnPreLoad;
            
            // Register quest after player spawns
            Player.LocalPlayerSpawned += OnLocalPlayerSpawned;
            
            LoggerInstance.Msg($"{Constants.MOD_NAME} v{Constants.MOD_VERSION} initialized");
        }

        private void OnLocalPlayerSpawned(Player player)
        {

        }

        public override void OnApplicationQuit()
        {
            // Cleanup Harmony patches
            GraffitiQuestTracker.Cleanup();
            
            // Unsubscribe from events
            Player.LocalPlayerSpawned -= OnLocalPlayerSpawned;
            
            Instance = null;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                // Add items to shops after a delay to ensure shops are ready
                if (!_shopsInitialized)
                {
                    // MelonCoroutines.Start(AddToShopsDelayed());
                }
            }
            else if (sceneName == "Menu")
            {
                // Reset initialization flags when returning to menu
                _itemsInitialized = false;
                _shopsInitialized = false;
            }
        }

        private void OnPreLoad()
        {
            if (_itemsInitialized) return;
            
            // Initialize Stay Silly Cap
            StaySillyCapCreator.Initialize();
            
            _itemsInitialized = true;
            LoggerInstance.Msg("Adding Big Willy to your game...");
        }
    }
}
