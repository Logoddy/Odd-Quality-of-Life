using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Odd_Quality_of_Life.Patches
{
    internal class Utility
    {        
        /// Set Tick Counter
        public static int elapsedTicks;
        public static int elapsedSittingTicks;

        /// Lightning Strike Sound Tracking
        private static bool isLightningStrikeActive = false;

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

            /// Non-Destructive Lightning / Soundless Lightning
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PerformLightningUpdate_Patch)}\": prefixing base game method \"StardewValley.Utility.performLightningUpdate\".", LogLevel.Trace);
            harmony.Patch
            (
            original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.performLightningUpdate)),
            prefix: new HarmonyMethod(typeof(Utility), nameof(PerformLightningUpdate_Patch))
            );

            /// Mute Thunder Sounds
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(MuteThunderSound)}\": prefixing base game method \"Game1.playSound()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound), new Type[] { typeof(string), typeof(int?) }),
                prefix: new HarmonyMethod(typeof(Utility), nameof(MuteThunderSound))
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
                    SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.TheHelper.Translation.Get("Utility.AFK.Message"), Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2);
                }
            }
        }

        public static void AFK_Protection(object sender, UpdateTickedEventArgs e)
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

        public static void PassOutPenalty(object sender, DayStartedEventArgs e)
        {
            if (PlayerPassedOut)
            {
                PlayerPassedOut = false;
                Game1.player.Stamina = (int)Math.Ceiling(Game1.player.Stamina / (float)ModEntry.Config.Passout_protection_stamina_penalty);
            }            
        }

        private static bool PassoutProtection(Farmer who)
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

        /// Non-Destructive Lightning
        public static bool PerformLightningUpdate_Patch(int time_of_day)
        {
            try
            {
                Random random = StardewValley.Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, time_of_day);
                if (random.NextDouble() < 0.125 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() / 100.0)
                {
                    Farm.LightningStrikeEvent lightningEvent = new Farm.LightningStrikeEvent();
                    lightningEvent.bigFlash = true;
                    Farm farm = Game1.getFarm();
                    List<Vector2> lightningRods = new List<Vector2>();
                    foreach (KeyValuePair<Vector2, StardewValley.Object> v in farm.objects.Pairs)
                    {
                        if (v.Value.QualifiedItemId == "(BC)9")
                        {
                            lightningRods.Add(v.Key);
                        }
                    }
                    if (lightningRods.Count > 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 v2 = random.ChooseFrom(lightningRods);
                            if (farm.objects[v2].heldObject.Value == null)
                            {
                                farm.objects[v2].heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)787");
                                farm.objects[v2].minutesUntilReady.Value = StardewValley.Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
                                farm.objects[v2].shakeTimer = 1000;
                                lightningEvent.createBolt = true;
                                lightningEvent.boltPosition = v2 * 64f + new Vector2(32f, 0f);
                                if (ModEntry.Config.Prevent_Lightning_Strike_Sound != true)
                                {
                                    isLightningStrikeActive = true;
                                    farm.lightningStrikeEvent.Fire(lightningEvent);
                                    isLightningStrikeActive = false;
                                }
                                return false;
                            }
                        }
                    }
                    if (random.NextDouble() < 0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0)
                    {
                        try
                        {
                            if (StardewValley.Utility.TryGetRandom(farm.terrainFeatures, out var tile, out var feature))
                            {
                                if (feature is FruitTree fruitTree)
                                {
                                    if (ModEntry.Config.Non_Destructive_Lightning_Fruit_Trees != true)
                                    {
                                        fruitTree.struckByLightningCountdown.Value = 4;
                                    }
                                    fruitTree.shake(tile, doEvenIfStillShaking: true);
                                    lightningEvent.createBolt = true;
                                    lightningEvent.boltPosition = tile * 64f + new Vector2(32f, -128f);
                                }
                                else
                                {
                                    Crop crop = (feature as HoeDirt)?.crop;
                                    bool num = crop != null && !crop.dead.Value;
                                    if (feature.performToolAction(null, 50, tile))
                                    {
                                        lightningEvent.createBolt = true;
                                        if (ModEntry.Config.Non_Destructive_Lightning_Objects != true)
                                        {
                                            lightningEvent.destroyedTerrainFeature = true;
                                            farm.terrainFeatures.Remove(tile);
                                        }
                                        lightningEvent.boltPosition = tile * 64f + new Vector2(32f, -128f);
                                    }
                                    if (num && crop.dead.Value)
                                    {
                                        lightningEvent.createBolt = true;
                                        lightningEvent.boltPosition = tile * 64f + new Vector2(32f, 0f);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (ModEntry.Config.Prevent_Lightning_Strike_Sound != true)
                    {
                        isLightningStrikeActive = true;
                        farm.lightningStrikeEvent.Fire(lightningEvent);
                        isLightningStrikeActive = false;
                    }
                }
                else if (random.NextDouble() < 0.1)
                {
                    Farm.LightningStrikeEvent lightningEvent2 = new Farm.LightningStrikeEvent();
                    lightningEvent2.smallFlash = true;
                    Farm farm = Game1.getFarm();
                    if (ModEntry.Config.Prevent_Lightning_Strike_Sound != true)
                    {
                        isLightningStrikeActive = true;
                        farm.lightningStrikeEvent.Fire(lightningEvent2);
                        isLightningStrikeActive = false;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                /// If an error occurs, log it
                ModEntry.TheMonitor.Log($"Failed in {nameof(PerformLightningUpdate_Patch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }

        /// Mute Thunder Sounds
        private static bool MuteThunderSound(string cueName)
        {
            if (ModEntry.Config.Prevent_Thunder_Sound && !isLightningStrikeActive && (cueName == "thunder" || cueName == "thunder_small"))
            {
                return false;
            }
            return true;
        }

        /// Sit to Regen
        public static void SitToRegen(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.Config.Sit_to_Regen)
            {
                try
                {
                    if (!Game1.shouldTimePass() || Game1.paused || Game1.activeClickableMenu != null)
                    {
                        return;
                    }                   
                    foreach (Farmer player in Game1.getAllFarmers())
                    {
                        if (player.IsSitting())
                        {
                            elapsedSittingTicks++;
                            float MaxStamina = player.MaxStamina;
                            if (player.Stamina < MaxStamina && player.Stamina < (MaxStamina * (ModEntry.Config.Sit_to_Regen_Rate_Maximum / 100f)) && elapsedSittingTicks >= ModEntry.Config.Sit_to_Regen_Rate)
                            {                             
                                player.Stamina++;
                                elapsedSittingTicks = 0;
                            }
                        }
                    }
                }
                catch
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(SitToRegen)}:\n", LogLevel.Error);
                }
            }
        }

        /// Auto Gates Tracking
        private struct GateKey : IEquatable<GateKey>
        {
            public string LocationName;
            public Vector2 Position;

            public bool Equals(GateKey other)
            {
                return LocationName == other.LocationName && Position == other.Position;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(LocationName, Position);
            }

            public override bool Equals(object obj)
            {
                return obj is GateKey key && Equals(key);
            }
        }

        private static Dictionary<GateKey, int> trackedGates = new Dictionary<GateKey, int>();

        /// Auto Gates
        public static void AutoGates(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            try
            {
                Farmer player = Game1.player;
                GameLocation location = player.currentLocation;

                if (location == null)
                {
                    return;
                }

                /// Determine if we should check for gates to open based on mount status
                bool isMounted = player.isRidingHorse();
                bool shouldCheckOpen = (isMounted && ModEntry.Config.Auto_gates_on_horse) || (!isMounted && ModEntry.Config.Auto_gates_on_foot);

                if (shouldCheckOpen)
                {
                    /// Build the list of tiles to check based on config
                    List<Vector2> tilesToCheck = new List<Vector2>();

                    if (ModEntry.Config.Auto_gates_facing_only)
                    {
                        /// Only check the tile the player is facing
                        tilesToCheck.Add(GetFacingTile(player));
                        if (isMounted)
                        {
                            tilesToCheck.Add(GetFacingTile(player, 2));
                        }
                    }
                    else
                    {
                        /// Check all four cardinal directions
                        tilesToCheck.Add(player.Tile + new Vector2(0, -1));
                        tilesToCheck.Add(player.Tile + new Vector2(1, 0));
                        tilesToCheck.Add(player.Tile + new Vector2(0, 1));
                        tilesToCheck.Add(player.Tile + new Vector2(-1, 0));
                        if (isMounted)
                        {
                            tilesToCheck.Add(player.Tile + new Vector2(0, -2));
                            tilesToCheck.Add(player.Tile + new Vector2(2, 0));
                            tilesToCheck.Add(player.Tile + new Vector2(0, 2));
                            tilesToCheck.Add(player.Tile + new Vector2(-2, 0));
                        }
                    }

                    /// Check each tile for a gate
                    foreach (Vector2 tile in tilesToCheck)
                    {
                        if (location.objects.TryGetValue(tile, out var obj) && obj is Fence gate && gate.isGate.Value)
                        {
                            if (gate.gatePosition.Value == 0)
                            {
                                /// Gate is closed, open it and any adjacent partner gate
                                OpenGate(gate, tile, location);
                                CheckAndOpenPartnerGate(tile, location);
                            }
                            else if (ModEntry.Config.Auto_gates_auto_close)
                            {
                                /// Gate is already open, reset close timer if we are tracking it
                                var key = new GateKey { LocationName = location.NameOrUniqueName, Position = tile };
                                if (trackedGates.ContainsKey(key))
                                {
                                    trackedGates[key] = 0;
                                }

                                /// Also reset timer on any tracked partner gate
                                ResetPartnerGateTimer(tile, location);
                            }
                        }
                    }
                }

                /// Handle closing tracked gates
                if (ModEntry.Config.Auto_gates_auto_close && trackedGates.Count > 0)
                {
                    var toRemove = new List<GateKey>();

                    foreach (var kvp in trackedGates)
                    {
                        var key = kvp.Key;

                        /// Check if any farmer is near this gate
                        bool playerNearby = false;
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            if (farmer.currentLocation?.NameOrUniqueName == key.LocationName)
                            {
                                float distance = Vector2.Distance(farmer.Tile, key.Position);
                                if (distance <= 2f)
                                {
                                    playerNearby = true;
                                    break;
                                }
                            }
                        }

                        if (playerNearby)
                        {
                            trackedGates[key] = 0;
                        }
                        else
                        {
                            trackedGates[key]++;
                            if (trackedGates[key] >= ModEntry.Config.Auto_gates_close_delay)
                            {
                                /// Close the gate
                                GameLocation gateLoc = Game1.getLocationFromName(key.LocationName);
                                if (gateLoc != null && gateLoc.objects.TryGetValue(key.Position, out var gateObj) && gateObj is Fence gateFence && gateFence.isGate.Value)
                                {
                                    gateFence.gatePosition.Value = 0;

                                    /// Also close any adjacent partner gate
                                    ClosePartnerGate(key.Position, gateLoc, toRemove);
                                }
                                toRemove.Add(key);
                            }
                        }
                    }

                    foreach (var key in toRemove)
                    {
                        trackedGates.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.TheMonitor.Log($"Failed in {nameof(AutoGates)}:\n{ex}", LogLevel.Error);
            }
        }

        /// Open a single gate and track it
        private static void OpenGate(Fence gate, Vector2 tile, GameLocation location)
        {
            gate.gatePosition.Value = 88;
            if (ModEntry.Config.Auto_gates_open_sound)
            {
                Game1.playSound("doorCreak");
            }
            if (ModEntry.Config.Auto_gates_auto_close)
            {
                var key = new GateKey { LocationName = location.NameOrUniqueName, Position = tile };
                trackedGates[key] = 0;
            }
        }

        /// Check adjacent tiles for a partner gate and open it
        private static void CheckAndOpenPartnerGate(Vector2 gateTile, GameLocation location)
        {
            Vector2[] adjacentTiles = new Vector2[]
            {
                gateTile + new Vector2(0, -1),
                gateTile + new Vector2(1, 0),
                gateTile + new Vector2(0, 1),
                gateTile + new Vector2(-1, 0)
            };

            foreach (Vector2 adjacentTile in adjacentTiles)
            {
                if (location.objects.TryGetValue(adjacentTile, out var adjObj) && adjObj is Fence adjGate && adjGate.isGate.Value && adjGate.gatePosition.Value == 0)
                {
                    /// Found a closed partner gate, open it without playing a second sound
                    adjGate.gatePosition.Value = 88;
                    if (ModEntry.Config.Auto_gates_auto_close)
                    {
                        var key = new GateKey { LocationName = location.NameOrUniqueName, Position = adjacentTile };
                        trackedGates[key] = 0;
                    }
                }
            }
        }

        /// Reset the close timer on any tracked partner gate
        private static void ResetPartnerGateTimer(Vector2 gateTile, GameLocation location)
        {
            if (!ModEntry.Config.Auto_gates_auto_close)
            {
                return;
            }

            Vector2[] adjacentTiles = new Vector2[]
            {
                gateTile + new Vector2(0, -1),
                gateTile + new Vector2(1, 0),
                gateTile + new Vector2(0, 1),
                gateTile + new Vector2(-1, 0)
            };

            foreach (Vector2 adjacentTile in adjacentTiles)
            {
                var key = new GateKey { LocationName = location.NameOrUniqueName, Position = adjacentTile };
                if (trackedGates.ContainsKey(key))
                {
                    trackedGates[key] = 0;
                }
            }
        }

        /// Close any tracked partner gate and add it to the removal list
        private static void ClosePartnerGate(Vector2 gateTile, GameLocation location, List<GateKey> toRemove)
        {
            Vector2[] adjacentTiles = new Vector2[]
            {
                gateTile + new Vector2(0, -1),
                gateTile + new Vector2(1, 0),
                gateTile + new Vector2(0, 1),
                gateTile + new Vector2(-1, 0)
            };

            foreach (Vector2 adjacentTile in adjacentTiles)
            {
                var key = new GateKey { LocationName = location.NameOrUniqueName, Position = adjacentTile };
                if (trackedGates.ContainsKey(key))
                {
                    if (location.objects.TryGetValue(adjacentTile, out var adjObj) && adjObj is Fence adjGate && adjGate.isGate.Value)
                    {
                        adjGate.gatePosition.Value = 0;
                    }
                    toRemove.Add(key);
                }
            }

            /// Play close sound once for the whole group
            if (ModEntry.Config.Auto_gates_close_sound)
            {
                Game1.playSound("doorClose");
            }
        }

        /// Get the tile the player is currently facing at a given distance
        private static Vector2 GetFacingTile(Farmer farmer, int distance = 1)
        {
            Vector2 tile = farmer.Tile;
            if (farmer.FacingDirection == 0)
            {
                return tile + new Vector2(0, -distance);
            }
            else if (farmer.FacingDirection == 1)
            {
                return tile + new Vector2(distance, 0);
            }
            else if (farmer.FacingDirection == 2)
            {
                return tile + new Vector2(0, distance);
            }
            else if (farmer.FacingDirection == 3)
            {
                return tile + new Vector2(-distance, 0);
            }
            return tile;
        }

        /// Clear tracked gates when returning to title
        public static void AutoGatesClear(object sender, ReturnedToTitleEventArgs e)
        {
            trackedGates.Clear();
        }
    }
}