using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace Odd_Quality_of_Life.Patches
{
    internal class Friendship
    {
        public static void EndDay(object sender, DayEndingEventArgs e)
        {
            if (ModEntry.Config.Spouse_decay || ModEntry.Config.Dating_decay || ModEntry.Config.Other_decay)
            {
                var farmers = Game1.getAllFarmers();
                var npcs = StardewValley.Utility.getAllCharacters();
                var farmerArray = farmers as Farmer[] ?? farmers.ToArray();

                /// Villager Friendships
                foreach (NPC character in npcs)
                {
                    if (!character.IsVillager && !character.IsMonster)
                    {
                        continue;
                    }

                    foreach (Farmer farmer in farmerArray)
                    {
                        if (!farmer.friendshipData.ContainsKey(character.Name))
                        {
                            continue;
                        }

                        var friendship = farmer.friendshipData[character.Name];

                        if (farmer.spouse == character.Name && !ModEntry.Config.Spouse_decay)
                        {
                            continue;
                        }
                        else if (friendship.Status == FriendshipStatus.Dating && !ModEntry.Config.Dating_decay
                                && farmer.spouse == null)
                        {
                            continue;
                        }
                        else if (!ModEntry.Config.Other_decay)
                        {
                            continue;
                        }

                        /// Set the villager talked to flag without giving free friendship points
                        friendship.TalkedToToday = true;
                    }
                }
            }

            /// Animal Friendships
            if (ModEntry.Config.Animal_decay)
            {
                var animals = Game1.getFarm().getAllFarmAnimals().Distinct();
                foreach (var animal in animals)
                {
                    /// Set the animal pet flag without giving free friendship points
                    animal.wasPet.Set(true);
                }
            }
        }

        /// Debugger
        public static void Debugger(object sender, DayStartedEventArgs e)
        {
            /// Friendship Decay (Debugger)
            if (ModEntry.Config.Enable_debugging_friendship)
            {
                var farmers = Game1.getAllFarmers();
                var npcs = StardewValley.Utility.getAllCharacters();
                var farmerArray = farmers as Farmer[] ?? farmers.ToArray();
                ModEntry.TheMonitor.Log("Beggining Debug log for all currently known villager friendships");

                /// Villager Friendships (Debug)
                foreach (NPC character in npcs)
                {
                    if (!character.IsVillager && !character.IsMonster)
                    {
                        continue;
                    }

                    foreach (Farmer farmer in farmerArray)
                    {
                        if (!farmer.friendshipData.ContainsKey((character.Name)))
                        {
                            continue;
                        }

                        var friendship = farmer.friendshipData[character.Name];
                        ModEntry.TheMonitor.Log(character.Name + "current friendship score: " + friendship.Points);
                    }
                }
            }

            /// Animal Friendships (Debug)
            if (ModEntry.Config.Animal_decay)
            {
                var animals = Game1.getFarm().getAllFarmAnimals().Distinct();
                ModEntry.TheMonitor.Log("Beggining Debug log for all current farm animal friendships");

                foreach (var animal in animals)
                {
                    ModEntry.TheMonitor.Log(animal.Name + "current firendship score: " + animal.friendshipTowardFarmer.Get());
                }
            }
        }
    }
}
