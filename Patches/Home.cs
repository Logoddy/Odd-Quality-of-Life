using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;

namespace Odd_Quality_of_Life.Patches
{
    internal class Home
    {
        /// Light the fireplace
        public static void LightFireplace(object sender, TimeChangedEventArgs e)
        {
            if (!ModEntry.Config.Auto_fireplace)
            {
                return;
            }

            int threshold = GetFireplaceThreshold();

            if (Game1.timeOfDay >= threshold)
            {
                foreach (Furniture furniture in Game1.player.currentLocation.furniture)
                {
                    if (furniture.furniture_type.Value == 14 && !furniture.IsOn)
                    {
                        furniture.IsOn = true;
                        furniture.initializeLightSource(furniture.TileLocation);
                        furniture.setFireplace(false, true);
                    }
                }
            }
        }

        /// Determine the fireplace lighting threshold based on season and weather
        private static int GetFireplaceThreshold()
        {
            if (Game1.IsSpring)
            {
                if (Game1.isRaining)
                {
                    return ModEntry.Config.Auto_fireplace_spring_rain;
                }
                else
                {
                    return ModEntry.Config.Auto_fireplace_spring;
                }
            }
            else if (Game1.IsSummer)
            {
                if (Game1.isRaining || Game1.isLightning || Game1.isGreenRain)
                {
                    return ModEntry.Config.Auto_fireplace_summer_rain;
                }
                else
                {
                    return ModEntry.Config.Auto_fireplace_summer;
                }
            }
            else if (Game1.IsFall)
            {
                if (Game1.isRaining || Game1.isLightning)
                {
                    return ModEntry.Config.Auto_fireplace_fall_rain;
                }
                else
                {
                    return ModEntry.Config.Auto_fireplace_fall;
                }
            }
            else if (Game1.IsWinter)
            {
                if (Game1.isSnowing)
                {
                    return ModEntry.Config.Auto_fireplace_winter_snow;
                }
                else
                {
                    return ModEntry.Config.Auto_fireplace_winter;
                }
            }

            /// Fallback if no season matches; 2600 is beyond latest game time so fireplaces won't light
            return 2600;
        }

        /// Morning Shutoff
        public static void StartDay(object sender, DayStartedEventArgs e)
        {
            if (ModEntry.Config.Auto_fireplace_douse)
            {
                foreach (Furniture furniture in Game1.player.currentLocation.furniture)
                {
                    if (furniture.furniture_type.Value == 14 && furniture.IsOn)
                    {
                        furniture.IsOn = false;
                        furniture.setFireplace(false, false);
                    }
                }
            }
        }
    }
}
