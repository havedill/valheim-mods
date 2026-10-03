# CraftingStationRange Plugin for Valheim
Unlimited range for crafting stations. Or configure a custom range.

As a reference, the game default for the stations is 20.

- `-1` = leave vanilla range alone (do not modify)
- `0` = unlimited
- `> 0` = custom range

Workbench / forge / stonecutter / artisan default to `0` (unlimited).
Any other station discovered at runtime (cauldron, Forge of Potential, etc.) defaults to `-1` (vanilla).

## Installation
1. Download and install [BepInEx Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Download this mod and move the `CraftingStationRange.dll` into `<GameLocation>\BepInEx\plugins`
3. Launching the game will generate a config file at `<GameLocation>\BepInEx\config`

## Configuration
`<GameLocation>/BepInEx/config/net.mtnewton.craftingstationrange.cfg`
```
[CraftingStationRange]

## Station build range. -1 = vanilla (do not modify), 0 = unlimited, >0 = custom range.
# Setting type: Single
# Default value: 0
$piece_workbench = 0

$piece_forge = 0
$piece_stonecutter = 0
$piece_artisanstation = 0

## Forge of Potential — leave at -1 unless you intentionally want a custom range.
$piece_upgradestation = -1
```

## Known Issues
- When set to unlimited range, a crafting station must still be loaded by the game. Walking far away from a station will cause it to unload.

## Changelog
- v1.0.4
    - Fix Forge of Potential / other new stations being broken by auto-binding them to unlimited
    - Stop destroying `m_areaMarker` (that broke station use)
    - Use a large finite range instead of `float.MaxValue`
    - Null-safe area marker updates
    - New stations default to `-1` (vanilla) instead of `0` (unlimited)
- v1.0.3
    - update readme - config path
- v1.0.2
    - fix readme dll name and typos
- v1.0.1
    - readme changes - installation steps, known issues, and changelog
- v1.0.0
    - initial mod creation
    - each crafting station can be configured to have a specific range (default is unlimited)

## Source Code
https://github.com/mtnewton/valheim-mods/tree/master/CraftingStationRange
