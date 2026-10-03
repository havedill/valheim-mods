using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftingStationRange
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    class CraftingStationRangePlugin : BaseUnityPlugin
    {
        const string GUID = "net.mtnewton.craftingstationrange";

        const string NAME = "CraftingStationRange";

        const string VERSION = "1.0.4";

        // Large finite range instead of float.MaxValue — MaxValue breaks distance checks.
        const float UnlimitedRange = 100000f;

        // Negative = leave the station's vanilla range alone.
        const float VanillaRange = -1f;

        private static ConfigFile config;

        private static ManualLogSource logger;

        private static readonly Dictionary<string, ConfigEntry<float>> stationRanges =
            new Dictionary<string, ConfigEntry<float>>();

        void Awake()
        {
            logger = Logger;
            config = Config;

            // Original stations default to unlimited (0), matching 1.0.3 behavior.
            BindStationRange("$piece_workbench", 0f);
            BindStationRange("$piece_forge", 0f);
            BindStationRange("$piece_stonecutter", 0f);
            BindStationRange("$piece_artisanstation", 0f);

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();

            Logger.LogInfo(NAME + " loaded.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CraftingStation), "Start")]
        public static void ApplyStationRange(CraftingStation __instance)
        {
            float stationRange = GetStationRange(__instance.m_name);

            // Unknown / newly added stations default to -1 (vanilla). Do not touch them.
            // This is what broke Forge of Potential ($piece_upgradestation): it was
            // auto-bound to 0, then had its area marker destroyed and range set to MaxValue.
            if (stationRange < 0f)
            {
                logger.LogInfo(__instance.m_name + " left at vanilla range.");
                return;
            }

            if (stationRange == 0f)
            {
                __instance.m_rangeBuild = UnlimitedRange;
                UpdateAreaMarker(__instance, UnlimitedRange);
                logger.LogInfo(__instance.m_name + " set to max range.");
                return;
            }

            __instance.m_rangeBuild = stationRange;
            UpdateAreaMarker(__instance, stationRange);
            logger.LogInfo(__instance.m_name + " set to " + stationRange + " range");
        }

        static void UpdateAreaMarker(CraftingStation station, float range)
        {
            // Never Destroy(m_areaMarker). Newer stations (Forge of Potential) need it,
            // and nulling it leads to "can't use the station" failures.
            if (station.m_areaMarker == null)
            {
                return;
            }

            CircleProjector projector = station.m_areaMarker.GetComponent<CircleProjector>();
            if (projector == null)
            {
                return;
            }

            projector.m_radius = range;
            // Cap segment count so unlimited range doesn't spawn hundreds of thousands of segments.
            float visualRadius = Math.Min(range, 200f);
            projector.m_nrOfSegments = (int)Math.Ceiling(Math.Max(5f, 4f * visualRadius));
        }

        public static float GetStationRange(string stationName)
        {
            if (!stationRanges.TryGetValue(stationName, out ConfigEntry<float> entry))
            {
                // Auto-discovered stations (cauldron, FoP, etc.): default to vanilla, not unlimited.
                entry = BindStationRange(stationName, VanillaRange);
            }

            return entry.Value;
        }

        public static ConfigEntry<float> BindStationRange(string stationName, float defaultRange)
        {
            ConfigEntry<float> entry = config.Bind(
                "CraftingStationRange",
                stationName,
                defaultRange,
                "Station build range. -1 = vanilla (do not modify), 0 = unlimited, >0 = custom range."
            );

            stationRanges[stationName] = entry;
            return entry;
        }
    }
}
