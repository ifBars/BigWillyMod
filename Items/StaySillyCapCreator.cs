using MelonLoader;
using HarmonyLib;
using S1API.Internal.Utils;
using S1API.Items;
using S1API.Rendering;
using S1API.Shops;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

#if (IL2CPPMELON)
using S1AvatarFramework = Il2CppScheduleOne.AvatarFramework;
#elif (MONOMELON || MONOBEPINEX || IL2CPPBEPINEX)
using S1AvatarFramework = ScheduleOne.AvatarFramework;
#endif

namespace BigWillyMod.Items
{
    /// <summary>
    /// Creates the Stay Silly Cap clothing item by cloning the base game cap.
    /// </summary>
    public static class StaySillyCapCreator
    {
        private const string ITEM_ID = "stay_silly_cap";
        private const string ITEM_NAME = "Stay Silly Cap";
        private const string ITEM_DESCRIPTION = "A custom cap that helps you stay silly at all times. Never take life too seriously!";
        
        private const string SOURCE_CAP_PATH = "avatar/accessories/head/cap/Cap";
        private const string CUSTOM_CAP_RESOURCE_PATH = "BigWillyMod/Accessories/StaySillyCap";
        
        private static bool _initialized = false;

        /// <summary>
        /// Initializes the Stay Silly Cap item.
        /// Should be called during GameLifecycle.OnPreLoad.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                MelonLogger.Warning("[StaySillyCap] Already initialized, skipping...");
                return;
            }

            try
            {
                // Step 1: Create and register custom accessory prefab
                if (!CreateCustomAccessory())
                {
                    MelonLogger.Error("[StaySillyCap] Failed to create custom accessory");
                    return;
                }

                // Step 2: Create the clothing item definition
                if (!CreateClothingItem())
                {
                    MelonLogger.Error("[StaySillyCap] Failed to create clothing item");
                    return;
                }

                _initialized = true;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[StaySillyCap] Initialization failed: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// Adds the Stay Silly Cap to compatible shops.
        /// Should be called after the Main scene is fully loaded.
        /// </summary>
        public static void AddToShops()
        {
            if (!_initialized)
            {
                MelonLogger.Warning("[StaySillyCap] Cannot add to shops - item not initialized");
                return;
            }

            try
            {
                var item = ItemManager.GetItemDefinition(ITEM_ID);
                if (item == null)
                {
                    MelonLogger.Error($"[StaySillyCap] Item '{ITEM_ID}' not found in registry");
                    return;
                }

                int shopsAdded = ShopManager.AddToCompatibleShops(item);
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[StaySillyCap] Failed to add to shops: {ex.Message}");
            }
        }

        private static bool CreateCustomAccessory()
        {
            try
            {
                // Load custom texture from embedded resources
                var assembly = Assembly.GetExecutingAssembly();
                var customTexture = TextureUtils.LoadTextureFromResource(
                    assembly,
                    "BigWillyMod.Resources.StaySillyCap.stay_silly_cap_texture.png");

                if (customTexture == null)
                {
                    MelonLogger.Error("[StaySillyCap] Failed to load custom texture from resources");
                    return false;
                }

                // Create texture replacement dictionary
                // Common Unity shader texture property names: _MainTex (standard), _BaseMap (URP), _Albedo
                // S1API will automatically apply these textures at runtime when the accessory is instantiated
                var textureReplacements = new Dictionary<string, Texture2D>
                {
                    { "_MainTex", customTexture },  // Standard shader
                    { "_BaseMap", customTexture },   // URP shader
                    { "_Albedo", customTexture }    // Some custom shaders
                };

                // Clone and customize the accessory with custom texture
                bool success = AccessoryFactory.CreateAndRegisterAccessory(
                    sourceResourcePath: SOURCE_CAP_PATH,
                    targetResourcePath: CUSTOM_CAP_RESOURCE_PATH,
                    newName: "StaySillyCap",
                    textureReplacements: textureReplacements,
                    colorTint: null
                );

                return success;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[StaySillyCap] Failed to create accessory: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
                return false;
            }
        }

        private static bool CreateClothingItem()
        {
            try
            {
                // Clone the base cap and customize
                var builder = ClothingItemCreator.CloneFrom("cap");
                if (builder == null)
                {
                    MelonLogger.Error("[StaySillyCap] Failed to clone base cap - 'cap' item not found");
                    return false;
                }

                // Load icon from embedded resources (optional - will use base cap icon if not found)
                var assembly = Assembly.GetExecutingAssembly();
                var icon = ImageUtils.LoadImageFromResource(
                    assembly,
                    "BigWillyMod.Resources.StaySillyCap.icon.png");
                
                // Verify the custom accessory is registered
                if (!RuntimeResourceRegistry.IsRegistered(CUSTOM_CAP_RESOURCE_PATH))
                {
                    MelonLogger.Warning($"[StaySillyCap] Custom accessory not registered at '{CUSTOM_CAP_RESOURCE_PATH}'. Model may not load correctly.");
                }

                var staySillyCap = builder
                    .WithBasicInfo(
                        id: ITEM_ID,
                        name: ITEM_NAME,
                        description: ITEM_DESCRIPTION)
                    .WithClothingAsset(CUSTOM_CAP_RESOURCE_PATH)
                    .WithColorable(false) // Custom textures, not colorable
                    .WithDefaultColor(ClothingColor.White)
                    .WithPricing(basePurchasePrice: 75f, resellMultiplier: 0.5f)
                    .WithKeywords("cap", "hat", "silly", "custom", "bigwilly")
                    .WithLabelColor(new Color(1f, 0.8f, 0.2f)) // Golden yellow tint
                    .Build();

                // Only set icon if we have a custom one (otherwise keep the cloned icon from base cap)
                if (icon != null)
                {
                    staySillyCap.Icon = icon;
                }

                if (staySillyCap == null)
                {
                    MelonLogger.Error("[StaySillyCap] Failed to build clothing item");
                    return false;
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[StaySillyCap] Failed to create clothing item: {ex.Message}");
                MelonLogger.Error(ex.StackTrace);
                return false;
            }
        }

    }
}

