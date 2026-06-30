using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using Netcode;

namespace Odd_Quality_of_Life.Patches
{
    internal class FenceDecay
    {
        /// Harmony Patch handler
        public static void ApplyPatches(Harmony harmony)
        {
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(FenceDecay_Patch)}\": prefixing base game method \"Fence.minutesElapsed()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Fence), nameof(Fence.minutesElapsed)),
                prefix: new HarmonyMethod(typeof(FenceDecay), nameof(FenceDecay_Patch))
            );            
        }

        /// Fence Decay Patch
        public static bool FenceDecay_Patch(Fence __instance, int minutes, ref bool __result)
        {
            if (ModEntry.Config.Fence_decay)
            {
                try
                {
                    if (!Game1.IsMasterGame)
                    {
                        __result = false;
                        return false;
                    }
                    __instance.PerformRepairIfNecessary();
                    if (!Game1.IsBuildingConstructed("Gold Clock") || Game1.netWorldState.Value.goldenClocksTurnedOff.Value)
                    {
                        __instance.health.Value -= FenceDecay_Calculate_Rate(minutes, ModEntry.Config.Fence_decay_speed);
                        if (__instance.health.Value <= -1f && (Game1.timeOfDay <= 610 || Game1.timeOfDay > 1800))
                        {
                            __result = false;
                            return true;
                        }
                    }
                    __result = false;
                    return false;
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it and run the original code
                    ModEntry.TheMonitor.Log($"Failed in {nameof(FenceDecay_Patch)}:\n{e}", LogLevel.Error);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private static float FenceDecay_Calculate_Rate(int minutes, int decay_speed)
        {
            if (decay_speed > 0)
            {
                if (ModEntry.Config.Enable_debugging_fence_decay)
                {
                    ModEntry.TheMonitor.Log($"The {nameof(FenceDecay_Patch)} method ran supplying a result of " + minutes / 1440f * (decay_speed / 100f) + " via the if case. Variables provided were \nMinutes: " + minutes + "\nDecay Speed: " + decay_speed);
                }
                return minutes / 1440f * (decay_speed / 100f);
            }
            else
            {
                if (ModEntry.Config.Enable_debugging_fence_decay)
                {
                    ModEntry.TheMonitor.Log($"The {nameof(FenceDecay_Patch)} method ran supplying a result of 0 via the else case");
                }
                return 0f;
            }
        }
    }
}

