using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Odd_Quality_of_Life.Patches
{
    internal class Utility
    {        
        /// Set Tick Counter
        public static int elapsedTicks;

        /// Harmony Patch Handler
        public static void ApplyPatches(Harmony harmony)
        {
            /// AFK Protection
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(AFK_Protection)}\": prefixing base game method \"Game1.UpdateGameClock()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateGameClock)),
                prefix: new HarmonyMethod(typeof(Utility), nameof(AFK_Protection))
            );

            /// Passout Protection
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassoutProtection)}\": prefixing base game method \"Farmer.passOutFromTired()\".", LogLevel.Trace);
            harmony.Patch
            (
            original: AccessTools.Method(typeof(Farmer), "passOutFromTired"),
            prefix: new HarmonyMethod(typeof(Utility), nameof(PassoutProtection))
            );

            /// Non-Destructive NPC's
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(NonDestructiveNPCS)}\": prefixing base game method \"GameLocation.characterDestroyObjectWithinRectangle()\".", LogLevel.Trace);
            harmony.Patch
            (
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.characterDestroyObjectWithinRectangle)),
            prefix: new HarmonyMethod(typeof(Utility), nameof(NonDestructiveNPCS))
            );
        }

        /// AFK Protection Prefix
        public static bool AFK_Protection()
        {
            if (ModEntry.Config.AFK_protection)
            {
                if (!Game1.IsMasterGame || Game1.eventUp || Game1.isFestival() || elapsedTicks < ModEntry.Config.AFK_timer)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public class AFKMenu : IClickableMenu
        {
            public override void draw(SpriteBatch b)
            {
                if (ModEntry.Config.AFK_notification)
                {
                    SpriteText.drawStringWithScrollCenteredAt(b, "You are now AFK; your game has paused", Game1.viewport.Width / 2, Game1.viewport.Height / 2);
                }
            }
        }

        public static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.Config.AFK_protection)
            {
                if (Game1.activeClickableMenu is AFKMenu || Game1.activeClickableMenu is TitleMenu || Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
                {
                    return;
                }
                if (Game1.player.movementDirections.Count > 0 || (Game1.player.CurrentTool is FishingRod && (Game1.player.CurrentTool as FishingRod).inUse()) || Game1.input.GetKeyboardState().GetPressedKeys().Length > 0 || (byte)AccessTools.Field(typeof(MouseState), "_buttons").GetValue(Game1.input.GetMouseState()) > 0)
                {
                    elapsedTicks = 0;
                    return;
                }
                if (elapsedTicks >= ModEntry.Config.AFK_timer)
                {
                    ModEntry.TheMonitor.Log("Going AFK");
                    Game1.activeClickableMenu = new AFKMenu();
                }
                else if (elapsedTicks < ModEntry.Config.AFK_timer)
                    elapsedTicks++;
            }
        }
        public static void AFKPlayerInput(object __sender, object e)
        {
            if (e is CursorMovedEventArgs && ModEntry.Config.AFK_release_mouse)
            {
                return;
            }
            elapsedTicks = 0;
            if (Game1.activeClickableMenu is AFKMenu)
            {
                Game1.activeClickableMenu = null;
            }
        }

        /// Passout Protection
        public static bool PlayerPassedOut { get; set; } = false;

        public void PassOutPenalty(object sender, DayStartedEventArgs e)
        {
            if (PlayerPassedOut)
            {
                PlayerPassedOut = false;
                Game1.player.Stamina = (int)Math.Ceiling(Game1.player.Stamina / (float)ModEntry.Config.Passout_protection_stamina_penalty);
            }            
        }

        private static bool PassoutProtection(StardewValley.Object __instance, Farmer who)
        {
            if (ModEntry.Config.Passout_protection_farm || ModEntry.Config.Passout_protection_everywhere)
            {
                try
                {
                    if (!who.IsLocalPlayer)
                    {
                        return true;
                    }
                    GameLocation loc = Game1.currentLocation;
                    if (ModEntry.Config.Passout_protection_everywhere || (ModEntry.Config.Passout_protection_farm && ((loc is Farm) || (loc is FarmCave) || (loc.isFarmBuildingInterior()) || (loc.Name == "Greenhouse") || (loc.Name == "Shed") || (loc.Name == "Big Shed"))))
                    {
                        ModEntry.TheMonitor.Log("Passed out in a safe location, protecting passout.");
                        PlayerPassedOut = true;
                        SendToBed(who);
                        SetPlayerState(who);
                        return false;
                    }
                    ModEntry.TheMonitor.Log("Passed out elsewhere, not protecting passout.");
                    return true;
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it and run the original
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassoutProtection)}:\n{e}", LogLevel.Error);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private static void SetPlayerState(Farmer who)
        {
            if (who.IsSitting())
            {
                who.StopSitting(animate: false);
            }
            if (who.isRidingHorse())
            {
                who.mount.dismount();
            }
            if (Game1.activeClickableMenu != null)
            {
                Game1.activeClickableMenu.emergencyShutDown();
                Game1.exitActiveMenu();
            }                        
            who.completelyStopAnimatingOrDoingAction();
            if (who.bathingClothes.Value == true)
            {
                who.changeOutOfSwimSuit();
            }
            who.swimming.Value = false;
            who.CanMove = false;                     
        }

        private static void SendToBed(Farmer who)
        {
            Farmer farmer = who;
            Vector2 bedPos = StardewValley.Utility.PointToVector2(StardewValley.Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
            bedPos.X -= 64f;
            LocationRequest locReq = Game1.getLocationRequest(farmer.homeLocation.Value);
            locReq.OnWarp += delegate
            {
                farmer.Position = bedPos;
                farmer.currentLocation.lastTouchActionLocation = bedPos;
                if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
                {
                    Game1.PassOutNewDay();
                }
                Game1.changeMusicTrack("none");
            };
            Game1.warpFarmer(locReq, (int)bedPos.X / 64, (int)bedPos.Y / 64, 2);
        }

        /// Auto Stacking
        public static void InventoryUpdated(object? sender, InventoryChangedEventArgs e)
        {
            foreach (Item addedItem in e.Added)
            {
                if (ModEntry.Config.Auto_stack_fishing_bait)
                {
                    if (addedItem.Category == -21)
                    {
                        StackBait(Game1.player.Items, addedItem);
                    }
                }
                if (ModEntry.Config.Auto_stack_slingshot_ammo)
                {
                    if (ValidSlingShotAmmo(addedItem))
                    {
                        StackAmmo(Game1.player.Items, addedItem);
                    }
                }
            }
        }

        public static void ChestInventoryUpdated(object? sender, ChestInventoryChangedEventArgs e)
        {        
            foreach (Item addedItem in e.Added)
            {
                if (ModEntry.Config.Auto_stack_fishing_bait)
                {
                    if (addedItem.Category == -21)
                    {
                        StackBait(e.Chest.Items, addedItem);
                    }
                }
                if (ModEntry.Config.Auto_stack_slingshot_ammo)
                {
                    if (ValidSlingShotAmmo(addedItem))
                    {
                        StackAmmo(e.Chest.Items, addedItem);
                    }
                }
            }
        }

        private static void StackBait(Inventory inventory, Item bait)
        {
            foreach (Item item in inventory)
            {
                if (item is FishingRod rod)
                {
                    StardewValley.Object currentBait = rod.GetBait();
                    if (currentBait != null && MatchingBait(currentBait, bait) && StackItems(inventory, currentBait, bait))
                    {
                        break;
                    }
                }
            }
        }

        private static void StackAmmo(Inventory inventory, Item ammo)
        {
            foreach (Item item in inventory)
            {
                if (item is Slingshot slingshot)
                {
                    StardewValley.Object currentAmmo = slingshot.attachments[0];
                    if (!(currentAmmo?.ItemId != ammo.ItemId) && StackItems(inventory, currentAmmo, ammo))
                    {
                        break;
                    }
                }
            }
        }

        private static bool StackItems(Inventory inventory, Item oldItem, Item newItem)
        {
            int newStackSize = oldItem.Stack + newItem.Stack;
            int maxSize = oldItem.maximumStackSize();
            if (newStackSize > maxSize)
            {
                oldItem.Stack = maxSize;
                newItem.Stack = newStackSize - maxSize;
                return false;
            }
            oldItem.Stack = newStackSize;
            inventory.RemoveButKeepEmptySlot(newItem);
            return true;
        }

        private static bool ValidSlingShotAmmo(Item item)
        {
            switch (item.QualifiedItemId)
            {
                case "(O)378":
                case "(O)388":
                case "(O)380":
                case "(O)390":
                case "(O)382":
                case "(O)384":
                case "(O)386":
                case "(O)441":
                    return true;
                default:
                    {
                        if (!(item is StardewValley.Object obj) || obj.bigCraftable.Value)
                        {
                            return false;
                        }
                        int category = item.Category;
                        if (category == -79 || category == -75 || category == -5)
                        {
                            return true;
                        }
                        return false;
                    }
            }
        }

        private static bool MatchingBait(Item bait1, Item bait2)
        {
            if (bait1.QualifiedItemId == bait2.QualifiedItemId)
            {
                return bait1.Name == bait2.Name;
            }
            return false;
        }
        
        /// Non-Destructive NPC's
        private static bool NonDestructiveNPCS(bool __result)
        {
            if (ModEntry.Config.Non_Destructive_NPCS)
            {
                try
                {
                    __result = false;
                    return false;
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it and run the original
                    ModEntry.TheMonitor.Log($"Failed in {nameof(NonDestructiveNPCS)}:\n{e}", LogLevel.Error);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
