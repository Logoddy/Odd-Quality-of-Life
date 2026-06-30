using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;

namespace Odd_Quality_of_Life.Patches
{
    internal class Fishing
    {
        /// Unlimited Legendary Fish
        public static void Unlimited_Legendary_Fish(object sender, AssetRequestedEventArgs e)
        {
            if (ModEntry.Config.Slightly_Limited_legendary_fish != 1 && e.Name.IsEquivalentTo("Data/Locations"))
            {
                e.Edit
                (
                    asset =>
                    {
                        foreach (LocationData location in asset.AsDictionary<string, LocationData>().Data.Values)
                        {
                            if (location.Fish is null)
                            {
                                continue;
                            }

                            foreach (SpawnFishData fish in location.Fish)
                            {
                                if (fish.CatchLimit == 1 && ItemContextTagManager.HasBaseTag(fish.ItemId, "fish_legendary"))
                                {
                                    fish.CatchLimit = ModEntry.Config.Slightly_Limited_legendary_fish;
                                }
                            }
                        }
                    },
                    /// Handles new legendary fish added by mods
                    AssetEditPriority.Late 
                );
            }
        }

        /// Max Cast Assist
        public static void Max_Cast_Assist(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.Config.Fishing_max_cast_assist)
            {
                if (Game1.player.CurrentTool is FishingRod { isTimingCast: true } rod)
                {
                    rod.castingPower = 1.01f;
                }
            }
        }

        /// Fish Hook Assist
        public static void Fish_Hook_Assist(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.Config.Fishing_hook_assist)
            {
                if (Game1.player.CurrentTool is FishingRod { isFishing: true, isNibbling: true } rod)
                {
                    if (!rod.isReeling && !rod.hit && !rod.pullingOutOfWater)
                    {
                        rod.timePerBobberBob = 1f;
                        rod.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                        rod.DoFunction(Game1.player.currentLocation, (int)rod.bobber.X, (int)rod.bobber.Y, 1, Game1.player);
                        Rumble.rumble(0.95f, 200f);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
