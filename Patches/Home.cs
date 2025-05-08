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
            if (ModEntry.Config.Auto_fireplace)
            {
                /// Spring
                if (Game1.IsSpring)
                {
                    /// Rain
                    if (Game1.isRaining)
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_spring_rain)
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
                    /// Sunny
                    else
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_spring)
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
                }
                /// Summer
                if (Game1.IsSummer)
                {   
                    /// Rain
                    if (Game1.isRaining || Game1.isLightning || Game1.isGreenRain)
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_summer_rain)
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
                    /// Sunny
                    else
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_summer)
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
                }
                /// Fall
                if (Game1.IsFall)
                {   
                    /// Rain
                    if (Game1.isRaining || Game1.isLightning)
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_fall_rain)
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
                    /// Sunny
                    else
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_fall)
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
                }
                /// Winter
                if (Game1.IsWinter)
                {   
                    /// Snow
                    if (Game1.isSnowing)
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_winter_snow)
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
                    /// Sunny
                    else
                    {
                        if (Game1.timeOfDay >= ModEntry.Config.Auto_fireplace_winter)
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
                }
            }
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
