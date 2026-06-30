using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace Odd_Quality_of_Life.Patches
{
    internal class AnimalDoors
    {
        /// Daily State Tracking
        private static bool doorsOpenedToday = false;
        private static bool doorsClosedToday = false;
        private static bool waitingToClose = false;

        /// SVE Building Type IDs
        private const string SVE_PREMIUM_BARN = "FlashShifter.StardewValleyExpandedCP_PremiumBarn";
        private const string SVE_PREMIUM_COOP = "FlashShifter.StardewValleyExpandedCP_PremiumCoop";

        /// Vanilla Building Level Mappings
        private static readonly Dictionary<string, int> CoopLevels = new Dictionary<string, int>
        {
            { "Coop", 1 },
            { "Big Coop", 2 },
            { "Deluxe Coop", 3 }
        };

        private static readonly Dictionary<string, int> BarnLevels = new Dictionary<string, int>
        {
            { "Barn", 1 },
            { "Big Barn", 2 },
            { "Deluxe Barn", 3 }
        };

        /// Time Changed - Open and close doors at configured times
        public static void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.IsMasterGame)
            {
                return;
            }

            if (!ModEntry.Config.Animal_doors_auto_open && !ModEntry.Config.Animal_doors_auto_close)
            {
                return;
            }

            try
            {
                /// Get open/close times based on season and weather
                int openTime;
                int closeTime;

                if (!GetDoorTimes(out openTime, out closeTime))
                {
                    /// Weather is blocked, do not open doors today
                    return;
                }

                /// Open doors when the time has been reached
                if (ModEntry.Config.Animal_doors_auto_open && !doorsOpenedToday && e.NewTime >= openTime && e.NewTime < closeTime)
                {
                    OpenEligibleDoors();
                    doorsOpenedToday = true;
                }

                /// Begin closing process when the time has been reached
                if (ModEntry.Config.Animal_doors_auto_close && !doorsClosedToday && e.NewTime >= closeTime)
                {
                    waitingToClose = true;
                }

                /// If waiting to close, try to close buildings whose animals have all returned
                if (waitingToClose && !doorsClosedToday)
                {
                    TryCloseReadyBuildings();
                }
            }
            catch (Exception ex)
            {
                ModEntry.TheMonitor.Log($"Failed in {nameof(TimeChanged)}:\n{ex}", LogLevel.Error);
            }
        }

        /// Day Ending Safety Net - Force warp animals and close any remaining open doors
        public static void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Game1.IsMasterGame)
            {
                return;
            }

            if (!ModEntry.Config.Animal_doors_auto_open && !ModEntry.Config.Animal_doors_auto_close)
            {
                return;
            }

            try
            {
                /// Force close everything that is still open
                if (!doorsClosedToday)
                {
                    ForceCloseAllDoors();
                    doorsClosedToday = true;
                }

                waitingToClose = false;
            }
            catch (Exception ex)
            {
                ModEntry.TheMonitor.Log($"Failed in DayEnding (AnimalDoors):\n{ex}", LogLevel.Error);
            }
        }

        /// Day Started - Reset daily state
        public static void DayStarted(object sender, DayStartedEventArgs e)
        {
            doorsOpenedToday = false;
            doorsClosedToday = false;
            waitingToClose = false;
        }

        /// Returned to Title - Clear all state
        public static void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            doorsOpenedToday = false;
            doorsClosedToday = false;
            waitingToClose = false;
        }

        /// Determine open/close times based on current season and weather
        private static bool GetDoorTimes(out int openTime, out int closeTime)
        {
            /// Start with seasonal defaults
            if (Game1.IsSpring)
            {
                openTime = ModEntry.Config.Animal_doors_spring_open;
                closeTime = ModEntry.Config.Animal_doors_spring_close;
            }
            else if (Game1.IsSummer)
            {
                openTime = ModEntry.Config.Animal_doors_summer_open;
                closeTime = ModEntry.Config.Animal_doors_summer_close;
            }
            else if (Game1.IsFall)
            {
                openTime = ModEntry.Config.Animal_doors_fall_open;
                closeTime = ModEntry.Config.Animal_doors_fall_close;
            }
            else if (Game1.IsWinter)
            {
                openTime = ModEntry.Config.Animal_doors_winter_open;
                closeTime = ModEntry.Config.Animal_doors_winter_close;
            }
            else
            {
                /// Fallback
                openTime = 730;
                closeTime = 1800;
            }

            /// Check weather overrides
            string weatherId = GetCurrentWeatherId();
            string mode = "Default";
            int weatherOpen = openTime;
            int weatherClose = closeTime;

            if (weatherId == "Rain")
            {
                mode = ModEntry.Config.Animal_doors_rain_mode;
                weatherOpen = ModEntry.Config.Animal_doors_rain_open;
                weatherClose = ModEntry.Config.Animal_doors_rain_close;
            }
            else if (weatherId == "Storm")
            {
                mode = ModEntry.Config.Animal_doors_storm_mode;
                weatherOpen = ModEntry.Config.Animal_doors_storm_open;
                weatherClose = ModEntry.Config.Animal_doors_storm_close;
            }
            else if (weatherId == "Snow")
            {
                mode = ModEntry.Config.Animal_doors_snow_mode;
                weatherOpen = ModEntry.Config.Animal_doors_snow_open;
                weatherClose = ModEntry.Config.Animal_doors_snow_close;
            }
            else if (weatherId == "Wind")
            {
                mode = ModEntry.Config.Animal_doors_wind_mode;
                weatherOpen = ModEntry.Config.Animal_doors_wind_open;
                weatherClose = ModEntry.Config.Animal_doors_wind_close;
            }
            else if (weatherId == "GreenRain")
            {
                mode = ModEntry.Config.Animal_doors_green_rain_mode;
                weatherOpen = ModEntry.Config.Animal_doors_green_rain_open;
                weatherClose = ModEntry.Config.Animal_doors_green_rain_close;
            }
            else if (weatherId != "Sun")
            {
                /// Unknown weather (modded) - check blocked list
                if (!string.IsNullOrWhiteSpace(ModEntry.Config.Animal_doors_blocked_weather_ids))
                {
                    string[] blockedIds = ModEntry.Config.Animal_doors_blocked_weather_ids.Split(',');
                    foreach (string id in blockedIds)
                    {
                        if (id.Trim().Equals(weatherId, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
            }

            /// Apply weather mode
            if (mode == "Block")
            {
                return false;
            }
            else if (mode == "Custom")
            {
                openTime = weatherOpen;
                closeTime = weatherClose;
            }
            /// "Default" keeps the seasonal times as-is

            return true;
        }

        /// Get the current weather ID from the farm location
        private static string GetCurrentWeatherId()
        {
            try
            {
                var weather = Game1.getFarm().GetWeather();
                if (weather != null && !string.IsNullOrEmpty(weather.Weather))
                {
                    return weather.Weather;
                }
            }
            catch
            {
                /// Fallback to boolean checks if weather API fails
            }

            if (Game1.isLightning)
            {
                return "Storm";
            }
            else if (Game1.isRaining)
            {
                return "Rain";
            }
            else if (Game1.isSnowing)
            {
                return "Snow";
            }
            else if (Game1.isGreenRain)
            {
                return "GreenRain";
            }
            else if (Game1.isDebrisWeather)
            {
                return "Wind";
            }

            return "Sun";
        }

        /// Get all buildings across all game locations
        private static IEnumerable<Building> GetAllBuildings()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Building building in location.buildings)
                {
                    yield return building;
                }
            }
        }

        /// Check if a building qualifies for auto doors based on config
        private static bool IsBuildingEligible(Building building)
        {
            if (building == null || building.isUnderConstruction())
            {
                return false;
            }

            /// Verify the building has an animal door
            var data = building.GetData();
            if (data == null || data.AnimalDoor == null)
            {
                return false;
            }

            string buildingType = building.buildingType.Value;

            /// Check vanilla coops
            if (CoopLevels.TryGetValue(buildingType, out int coopLevel))
            {
                return coopLevel >= ModEntry.Config.Animal_doors_coop_level;
            }

            /// Check vanilla barns
            if (BarnLevels.TryGetValue(buildingType, out int barnLevel))
            {
                return barnLevel >= ModEntry.Config.Animal_doors_barn_level;
            }

            /// Check SVE Premium Barn
            if (buildingType == SVE_PREMIUM_BARN)
            {
                return ModEntry.Config.Animal_doors_sve_premium_barn;
            }

            /// Check SVE Premium Coop
            if (buildingType == SVE_PREMIUM_COOP)
            {
                return ModEntry.Config.Animal_doors_sve_premium_coop;
            }

            /// All other modded buildings with animal doors
            return ModEntry.Config.Animal_doors_other_buildings;
        }

        /// Open doors on all eligible buildings
        private static void OpenEligibleDoors()
        {
            try
            {
                bool soundPlayed = false;

                foreach (Building building in GetAllBuildings())
                {
                    if (IsBuildingEligible(building) && !building.animalDoorOpen.Value)
                    {
                        building.animalDoorOpen.Value = true;

                        /// Play sound once if player is on the farm
                        if (ModEntry.Config.Animal_doors_sound && !soundPlayed && Game1.player.currentLocation == building.GetParentLocation())
                        {
                            Game1.playSound("doorCreak");
                            soundPlayed = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.TheMonitor.Log($"Failed in {nameof(OpenEligibleDoors)}:\n{e}", LogLevel.Error);
            }
        }

        /// Try to close buildings whose animals have all returned home naturally
        private static void TryCloseReadyBuildings()
        {
            try
            {
                bool allClosed = true;
                bool soundPlayed = false;

                foreach (Building building in GetAllBuildings())
                {
                    if (IsBuildingEligible(building) && building.animalDoorOpen.Value)
                    {
                        /// Check if all animals are home
                        if (AreAllAnimalsHome(building))
                        {
                            /// All animals returned, close this building
                            building.animalDoorOpen.Value = false;

                            if (ModEntry.Config.Animal_doors_sound && !soundPlayed && Game1.player.currentLocation == building.GetParentLocation())
                            {
                                Game1.playSound("doorCreakReverse");
                                soundPlayed = true;
                            }
                        }
                        else
                        {
                            /// At least one building still has animals outside
                            allClosed = false;
                        }
                    }
                }

                /// If every eligible building is now closed, we are done for the day
                if (allClosed)
                {
                    doorsClosedToday = true;
                    waitingToClose = false;
                }
            }
            catch (Exception e)
            {
                ModEntry.TheMonitor.Log($"Failed in {nameof(TryCloseReadyBuildings)}:\n{e}", LogLevel.Error);
            }
        }

        /// Force close all doors and warp animals home (DayEnding safety net only)
        private static void ForceCloseAllDoors()
        {
            try
            {
                bool soundPlayed = false;

                foreach (Building building in GetAllBuildings())
                {
                    if (IsBuildingEligible(building) && building.animalDoorOpen.Value)
                    {
                        /// Force warp all outdoor animals back into the building
                        WarpAnimalsHome(building);

                        /// Close the door
                        building.animalDoorOpen.Value = false;

                        /// Play sound once if player is on the farm
                        if (ModEntry.Config.Animal_doors_sound && !soundPlayed && Game1.player.currentLocation == building.GetParentLocation())
                        {
                            Game1.playSound("doorCreakReverse");
                            soundPlayed = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.TheMonitor.Log($"Failed in {nameof(ForceCloseAllDoors)}:\n{e}", LogLevel.Error);
            }
        }

        /// Check if all animals belonging to a building are currently inside
        private static bool AreAllAnimalsHome(Building building)
        {
            if (building.indoors.Value is not AnimalHouse animalHouse)
            {
                return true;
            }

            return animalHouse.animalsThatLiveHere.Count == animalHouse.animals.Count();
        }

        /// Warp all animals belonging to a building back inside
        private static void WarpAnimalsHome(Building building)
        {
            if (building.indoors.Value is not AnimalHouse animalHouse)
            {
                return;
            }

            /// Get the outdoor location this building sits in
            GameLocation parentLocation = building.GetParentLocation();
            if (parentLocation == null)
            {
                return;
            }

            foreach (long animalId in animalHouse.animalsThatLiveHere)
            {
                FarmAnimal animal = StardewValley.Utility.getAnimal(animalId);

                /// Only warp animals that are currently outside in the building's parent location
                if (animal != null && parentLocation.animals.ContainsKey(animal.myID.Value))
                {
                    animal.warpHome();
                }
            }
        }
    }
}
