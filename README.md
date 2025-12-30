# BigWillyMod

A mod for Schedule One that adds Big Willy NPC and custom items.

## Features

### NPCs
- **Big Willy** - A unique NPC with custom schedule and appearance

### Items
- **Stay Silly Cap** - A custom wearable cap that clones the base game cap with custom textures

## Installation

1. Build the mod using `dotnet build BigWillyMod/BigWillyMod.csproj -c Release`
2. Copy the built DLL from `bin/Release/` to your game's `Mods/` folder
3. Launch the game

## Custom Assets

The Stay Silly Cap supports custom textures. Place your assets in:
- `BigWillyMod/Resources/StaySillyCap/icon.png` - Inventory icon (256x256 or 512x512 recommended)
- `BigWillyMod/Resources/StaySillyCap/cap_texture.png` - Custom cap texture (optional)

These will be automatically embedded during build.

## Testing

### Console Commands
- `give stay_silly_cap 1` - Spawn the Stay Silly Cap in your inventory
- Equip it from your inventory to see it on your character

### Shop Integration
The Stay Silly Cap is automatically added to compatible clothing shops after the Main scene loads.

## Technical Details

### Dependencies
- S1API (for clothing system, item creation, shop integration)
- MelonLoader

### Implementation Notes
- Uses S1API's clothing system (`ClothingItemCreator.CloneFrom`)
- Runtime resource registry for custom accessory prefabs
- Material/texture customization via `AccessoryFactory`
- Automatic shop integration via `ShopManager.AddToCompatibleShops`

## Building

```bash
dotnet build BigWillyMod/BigWillyMod.csproj -c Release
```

The output DLL will be in `bin/Release/netstandard2.1/BigWillyMod.dll`

