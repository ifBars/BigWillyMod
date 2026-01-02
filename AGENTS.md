# Repository Guidelines

## Project Structure & Module Organization
- `Core.cs` is the MelonMod entry point and wires lifecycle hooks for items, quests, and scene events.
- Gameplay content lives in `Items/`, `NPCs/`, and `Quests/`; keep new features in their respective folders with one class per file.
- Shared constants and helpers belong in `Utils/`; prefer expanding `Constants` rather than scattering magic strings.
- Embedded assets (icons, textures) sit under `Resources/**`; they are packaged automatically by MSBuild.
- Build outputs go to `bin/<Configuration>/netstandard2.1/`; avoid editing `bin/` or `obj/` by hand.

## Build, Test, and Development Commands
- For contributors, copy `local.build.props.example` to `local.build.props` and set your S1API and game paths there. The file is gitignored.
- Alternatively, configure game paths inline: `dotnet build BigWillyMod.csproj -c CrossCompat /p:GamePath=\"D:\\\\SteamLibrary\\\\...\" /p:ManagedPath=\"$(GamePath)\\Schedule I_Data\\Managed\" /p:MelonLoaderPath=\"$(GamePath)\\MelonLoader\\net35\" /p:S1ApiPath=\"C:\\\\Users\\\\ghost\\\\Desktop\\\\Coding\\\\ScheduleOne\\\\S1API\\\\S1API\\\\bin\\\\MonoMelon\\\\netstandard2.1\"`.
- Fast rebuild: `dotnet build BigWillyMod.csproj -c CrossCompat` (uses defaults from the project file or local.build.props).
- Clean artifacts: `dotnet clean BigWillyMod.csproj`.
- To try the mod, copy `bin/CrossCompat/netstandard2.1/BigWillyMod.dll` into the game’s `Mods/` folder and launch Schedule I with MelonLoader.

## Coding Style & Naming Conventions
- C# 8+/netstandard2.1 with nullable enabled; 4-space indentation; braces on new lines.
- Use `PascalCase` for types/methods/properties, `camelCase` for locals, and `CAPS_SNAKE` for constants (see `Utils/Constants.cs`).
- Prefer clear guard clauses, avoid silent failures, and log via `MelonLogger` with context tags.
- Keep assets referenced by fully qualified resource names (`BigWillyMod.Resources.*`); avoid hard-coded file system paths in runtime code.

## Testing Guidelines
- No automated tests yet; validate manually in-game.
- Recommended checks: `give stay_silly_cap 1` to spawn the item, equip it to confirm textures, and enter the Main scene to ensure shop integration.
- When adding quests/NPC changes, exercise the relevant scenes and watch the MelonLoader console for warnings or unhandled exceptions.

## Commit & Pull Request Guidelines
- Use concise, imperative commit messages (e.g., `Add graffiti quest tracker`, `Fix Stay Silly cap icon load`); current history uses short summaries.
- For PRs, include: goal/behavior change, build command used, and manual test steps or screenshots (especially for visual assets).
- Update README or in-file comments when changing build steps, asset paths, or lifecycle hooks to keep contributors aligned.
