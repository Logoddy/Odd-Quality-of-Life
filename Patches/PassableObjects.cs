using System;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Objects;
using StardewValley.Extensions;
using Netcode;
using Microsoft.Xna.Framework.Graphics;
using System.Data;
using System.Xml.Linq;

namespace Odd_Quality_of_Life.Patches
{
    internal class PassableObjects
    {
        /// Create Drawing
        private static bool isDrawing = false;

        /// Harmony Patch handler
        public static void ApplyPatches(Harmony harmony)
        {
            /// Passable Sprinklers
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableSprinklers_Patch)}\": postfixing base game method \"StardewValley.Object.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPassable)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableSprinklers_Patch))
            );

            /// Passable Scarecrows
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableScarecrows_Patch)}\": postfixing base game method \"StardewValley.Object.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPassable)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableScarecrows_Patch))
            );

            /// Passable Crops
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableCrops_Patch)}\": postfixing base game method \"HoeDirt.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(HoeDirt), "isPassable", new Type[1] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableCrops_Patch))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(HoeDirt), "doCollisionAction"),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableCrops_Collisions_Patch)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableCrops_Collisions_Patch))
            );

            /// Passable Bushes
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableBushes_Patch)}\": postfixing base game method \"HoeDirt.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Bush), "isPassable", new Type[1] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableBushes_Patch))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Bush), nameof(Bush.draw), new Type[1] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Draw_Bush))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Bush), nameof(Bush.getBoundingBox)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(Postfix_Bush_BoundingBox))
            );

            /// Passable Saplings
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableSaplings_Patch)}\": postfixing base game methods \"StardewValley.Terrain.Tree.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Tree), nameof(Tree.isPassable), new Type[1] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableSaplings_Patch))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Tree), nameof(FruitTree.draw), new Type[1] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Draw_Tree))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Tree), nameof(FruitTree.getBoundingBox)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(Postfix_Tree_BoundingBox))
            );

            /// Passable Fruit Saplings
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableSaplings_Patch)}\": postfixing base game methods \"StardewValley.Terrain.FruitTree.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.isPassable), new Type[1] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableFruitSaplings_Patch))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.draw), new Type[1] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Draw_Fruit_Tree))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.getBoundingBox)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(Postfix_Fruit_Tree_BoundingBox))
            );

            /// Passable Weeds
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableWeeds_Patch)}\": postfixing base game method \"StardewValley.Object.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPassable)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableWeeds_Patch))
            );

            /// Passable Forage
            ModEntry.TheMonitor.Log($"Applying Harmony patch \"{nameof(PassableForage_Patch)}\": postfixing base game method \"StardewValley.Object.isPassable()\".", LogLevel.Trace);
            harmony.Patch
            (
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPassable)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PassableForage_Patch))
            );

            /// Shaker Patches
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Character), "MovePosition"),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Character_MovePosition)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(Postfix_Character_MovePosition))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(Farmer), "MovePosition"),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Farmer_MovePosition)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(Postfix_Character_MovePosition))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(StardewValley.Object), "draw", new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Draw_Object)),
                postfix: new HarmonyMethod(typeof(PassableObjects), nameof(PostFix_Draw_Object))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Draw_SpriteBatch_First))
            );
            harmony.Patch
            (
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(PassableObjects), nameof(Prefix_Draw_SpriteBatch_Second))
            );
        }

        /// Rustling Sound Handler
        public static void PlayRustleSound(Vector2 tile, GameLocation location)
        {
            if ((ModEntry.Config.Shake_when_passing) && StardewValley.Utility.isOnScreen(new Point((int)tile.X, (int)tile.Y), 2, location))
            {
                Grass.PlayGrassSound();
            }
        }

        /// Shake Handlers
        private const float Shake_rate = (float)Math.PI / 100f;
        private const float Shake_max = (float)Math.PI / 12f;
        private const float Shake_max_stiff = (float)Math.PI / 16f;

        private struct Shake_Data
        {
            public ObjType Object_type;
            public bool Passable;
            public float MaxShake, ShakeRotation;
            public bool ShakeLeft;
            public Character Character;
        }

        private enum ObjType
        {
            None = 0,
            Scarecrow = 1,
            Sprinkler = 2,
            Forage = 3,
            Weed = 4
        }

        private static ObjType GetObjType(StardewValley.Object o)
        {
            if ((ModEntry.Config.Passable_sprinklers != false) && (o.IsSprinkler() != false))
            {
                return ObjType.Sprinkler;
            }
            if ((ModEntry.Config.Passable_scarecrows != false) && (o.IsScarecrow() != false))
            {
                return ObjType.Scarecrow;
            }
            if ((ModEntry.Config.Passable_forage != false) && (o.isForage() != false))
            {
                return ObjType.Forage;
            }
            if ((ModEntry.Config.Passable_weeds != false) && (o.IsWeeds() != false))
            {
                return ObjType.Weed;
            }
            return ObjType.None;
        }

        private static Shake_Data Last_shake_data;

        private static string KeyDataShake = "Lt_oddy.Odd_Quality_of_Life/shake";

        private static void Prefix_Farmer_MovePosition(Farmer __instance)
        {
            Last_shake_data.Character = __instance;
        }
        private static void Prefix_Character_MovePosition(Character __instance)
        {
            Last_shake_data.Character = __instance;
        }
        private static void Postfix_Character_MovePosition()
        {
            Last_shake_data.Character = null;
        }

        private static void Prefix_Draw_Object(StardewValley.Object __instance)
        {
            var ot = GetObjType(__instance);
            if (ot != ObjType.None
                && __instance!.modData.TryGetValue(KeyDataShake, out var data)
            )
            {
                var s = (data ?? "").Split(';');
                if (s.Length == 3
                    && float.TryParse(s[0], out var maxShake)
                    && float.TryParse(s[1], out var shakeRotation)
                    && bool.TryParse(s[2], out var shakeLeft))
                {
                    // calc new shake data
                    if (maxShake > 0f)
                    {
                        if (shakeLeft)
                        {
                            shakeRotation -= Shake_rate;
                            if (Math.Abs(shakeRotation) >= maxShake)
                            {
                                shakeLeft = false;
                            }
                        }
                        else
                        {
                            shakeRotation += Shake_rate;
                            if (shakeRotation >= maxShake)
                            {
                                shakeLeft = true;
                                shakeRotation -= Shake_rate;
                            }
                        }
                        maxShake = Math.Max(0f, maxShake - (float)Math.PI / 300f);
                    }
                    else
                    {
                        shakeRotation /= 2f;
                        if (shakeRotation <= 0.01f)
                        {
                            shakeRotation = 0f;
                        }
                    }
                    // update tracking values
                    __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                    Last_shake_data.Object_type = ot;
                    Last_shake_data.MaxShake = maxShake;
                    Last_shake_data.ShakeRotation = shakeRotation;
                    Last_shake_data.ShakeLeft = shakeLeft;
                }
            }
        }

        private static void PostFix_Draw_Object() => Last_shake_data.Object_type = ObjType.None;

        private static void MoveRotation(ref Rectangle destinationRectangle, ref Vector2 origin, Vector2 move)
        {
            destinationRectangle = new Rectangle(destinationRectangle.Location.X + (int)move.X * 4, destinationRectangle.Location.Y + (int)move.Y * 4, destinationRectangle.Width, destinationRectangle.Height);
            origin += move;
        }

        private static void MoveRotation(ref Vector2 position, ref Vector2 origin, Vector2 move)
        {
            position += move * 4;
            origin += move;
        }

        private static void Prefix_Draw_SpriteBatch_First(ref Rectangle destinationRectangle, ref float rotation, ref Vector2 origin)
        {
            if (Last_shake_data.Object_type != ObjType.None)
            {
                if ((ModEntry.Config.Shake_when_passing == true))
                {
                    rotation = Last_shake_data.ShakeRotation;
                }
                switch (Last_shake_data.Object_type)
                {
                    case ObjType.Scarecrow:
                        // move rotation to bottom of post
                        MoveRotation(ref destinationRectangle, ref origin, new Vector2(8f, 30f));
                        break;
                }
            }
        }

        private static void Prefix_Draw_SpriteBatch_Second(ref Vector2 position, ref float rotation, ref Vector2 origin, ref float layerDepth)
        {
            if (Last_shake_data.Object_type != ObjType.None)
            {
                if ((ModEntry.Config.Shake_when_passing == true))
                {
                    rotation = Last_shake_data.ShakeRotation;
                }
                switch (Last_shake_data.Object_type)
                {
                    case ObjType.Weed:
                        layerDepth += (Last_shake_data.Passable ? 24 : -40) / 10000f;
                        MoveRotation(ref position, ref origin, new Vector2(0f, 8f));
                        break;
                    case ObjType.Sprinkler:
                        layerDepth += (Last_shake_data.Passable ? 45 : -19) / 10000f;
                        break;
                    case ObjType.Forage:
                        layerDepth += (Last_shake_data.Passable ? 32 : -32) / 10000f;
                        MoveRotation(ref position, ref origin, new Vector2(0f, 8f));
                        break;
                }
            }
        }

        /// Passable Sprinklers Patch
        private static void PassableSprinklers_Patch(StardewValley.Object __instance, ref bool __result)
        {
            if (ModEntry.Config.Passable_sprinklers)
            {
                try
                {
                    if (__instance.IsSprinkler())
                    {
                        var character = Last_shake_data.Character = Game1.player;
                        var character_bounding_box = character.GetBoundingBox();
                        var object_bounding_box = __instance.GetBoundingBoxAt((int)__instance.TileLocation.X, (int)__instance.TileLocation.Y);
                        var character_bounding_box_enlarged = character_bounding_box.Clone();
                        character_bounding_box_enlarged.Inflate(16, 16);
                        var intersect = character_bounding_box_enlarged.Intersects(object_bounding_box);
                        Last_shake_data.Passable = intersect;
                        __result = character_bounding_box_enlarged.Intersects(object_bounding_box);

                        if (character_bounding_box.Intersects(object_bounding_box))
                        {
                            if (ModEntry.Config.Slowdown_when_passing && character == Game1.player)
                            {
                                if (Game1.player.stats.Get("Book_Grass") == 0)
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                                }
                                else
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                                }
                            }
                            if (Last_shake_data.Character is not null && (!__instance!.modData.TryGetValue(KeyDataShake, out var data) || !float.TryParse((data = "").Split(';')[0], out var maxShake) || maxShake <= 0f))
                            {
                                maxShake = Shake_max_stiff;
                                var shakeLeft = character_bounding_box.Center.X > __instance.TileLocation.X * 64f + 32f;
                                var shakeRotation = 0f;
                                __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                                PlayRustleSound(__instance.TileLocation, __instance.Location);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableSprinklers_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Scarecrows Patch
        private static void PassableScarecrows_Patch(StardewValley.Object __instance, ref bool __result)
        {
            if (ModEntry.Config.Passable_scarecrows)
            {
                try
                {
                    if (__instance.IsScarecrow())
                    {
                        var character = Last_shake_data.Character = Game1.player;
                        var character_bounding_box = character.GetBoundingBox();
                        var object_bounding_box = __instance.GetBoundingBoxAt((int)__instance.TileLocation.X, (int)__instance.TileLocation.Y);
                        var character_bounding_box_enlarged = character_bounding_box.Clone();
                        character_bounding_box_enlarged.Inflate(16, 16);
                        var intersect = character_bounding_box_enlarged.Intersects(object_bounding_box);
                        Last_shake_data.Passable = intersect;
                        __result = character_bounding_box_enlarged.Intersects(object_bounding_box);

                        if (character_bounding_box.Intersects(object_bounding_box))
                        {
                            if (ModEntry.Config.Slowdown_when_passing && character == Game1.player)
                            {
                                if (Game1.player.stats.Get("Book_Grass") == 0)
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                                }
                                else
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                                }
                            }
                            if (Last_shake_data.Character is not null && (!__instance!.modData.TryGetValue(KeyDataShake, out var data) || !float.TryParse((data = "").Split(';')[0], out var maxShake) || maxShake <= 0f))
                            {
                                maxShake = Shake_max_stiff;
                                var shakeLeft = character_bounding_box.Center.X > __instance.TileLocation.X * 64f + 32f;
                                var shakeRotation = 0f;
                                __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                                PlayRustleSound(__instance.TileLocation, __instance.Location);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableScarecrows_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Crops Patch
        private static void PassableCrops_Patch(HoeDirt __instance, ref bool __result, Character c)
        {
            if (ModEntry.Config.Passable_crops)
            {
                try
                {
                    var farmer = c as Farmer;
                    if (farmer != null)
                    {
                        if (farmer is not null)
                        {
                            __result |= __instance.crop is not null;
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableCrops_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Crops Collisions
        private static void PassableCrops_Collisions_Patch(HoeDirt __instance)
        {
            if (ModEntry.Config.Passable_crops)
            {
                try
                {
                    /// Only handle raised seed crops, which should still shake when newly planted
                    if (__instance.crop != null && __instance.crop.raisedSeeds.Value != false)
                    {
                        switch (__instance.crop.currentPhase.Value)
                        {
                            /// Set newly planted crops to an invalid phase to allow shaking
                            case 0:
                                __instance.crop.currentPhase.Value = -1;
                                break;
                            /// If already in an invalid phase; set to 0
                            case -1:
                                __instance.crop.currentPhase.Value = 0;
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableCrops_Collisions_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Bushes Patch
        private static void PassableBushes_Patch(Bush __instance, ref bool __result, ref float ___maxShake, ref bool ___shakeLeft, Character c)
        {
            if (ModEntry.Config.Passable_bushes)
            {
                try
                {
                    if (!AnyPassable(__instance))
                    {
                        return;
                    }
                    var farmer = Game1.player;
                    if (farmer is not null)
                    {
                        __result = true;
                        if (farmer is not null && ModEntry.Config.Slowdown_when_passing)
                        {
                            if (farmer.stats.Get("Book_Grass") == 0)
                            {
                                farmer.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                            }
                            else
                            {
                                farmer.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                            }
                        }
                        if ((ModEntry.Config.Shake_when_passing == true) && c is not null && ___maxShake == 0f)
                        {
                            ___shakeLeft = c.Tile.X > __instance!.Tile.X || (c.Tile.X == __instance.Tile.X && Game1.random.NextBool());
                            ___maxShake = (float)Math.PI / 40f;
                            __instance.shakeTimer = 1000f;
                            __instance.NeedsUpdate = true;
                            PlayRustleSound(__instance.Tile, __instance.Location);
                        }
                    }
                }                
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableBushes_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Bush Object Check
        private static bool AnyPassable(Bush bush)
        {
            if (ModEntry.Config != null && (bush?.size.Value ?? 0) == 3)
            {
                return !(bush?.inPot.Value ?? false);
            }
            return false;
        }

        /// Passable Bush Shaking
        private static void Prefix_Draw_Bush()
        {
            isDrawing = true;
        }
        private static void Postfix_Bush_BoundingBox(ref Rectangle __result)
        {
            if (isDrawing)
            {
                isDrawing = false;
                var skew = -46;
                __result = new Rectangle(__result.X, __result.Y + skew, __result.Width, __result.Height);
            }
        }

        /// Passable Saplings Patch
        private static void PassableSaplings_Patch(Tree __instance, ref bool __result, ref float ___maxShake, ref NetBool ___shakeLeft, Character c)
        {
            if (ModEntry.Config.Passable_saplings)
            {
                try
                {
                    if (__instance.growthStage.Value < 5)
                    {
                        var farmer = c as Farmer;
                        if (farmer != null)
                        {
                            __result = true;
                            if (farmer is not null && ModEntry.Config.Slowdown_when_passing)
                            {
                                if (farmer.stats.Get("Book_Grass") == 0)
                                {
                                    farmer.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                                }
                                else
                                {
                                    farmer.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                                }
                            }
                            if ((ModEntry.Config.Shake_when_passing == true) && c is not null && ___maxShake == 0f && __instance.growthStage.Value > 0)
                            {
                                ___shakeLeft.Value = c.StandingPixel.X > (__instance.Tile.X + 0.5f) * 64f || (c.Tile.X == __instance.Tile.X && Game1.random.NextBool());
                                ___maxShake = (float)(Math.PI / 64.0);
                                PlayRustleSound(__instance.Tile, __instance.Location);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableSaplings_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Tree Shaking
        private static void Prefix_Draw_Tree()
        {
            isDrawing = true;
        }
        private static void Postfix_Tree_BoundingBox(FruitTree __instance, ref Rectangle __result)
        {
            if (isDrawing)
            {
                isDrawing = false;
                var skew = __instance.growthStage.Value switch
                {
                    0 => -46,
                    1 => -46,
                    2 => -34,
                    3 => -30,
                    4 => -30,
                    _ => 0
                };
                __result = new Rectangle(__result.X, __result.Y + skew, __result.Width, __result.Height);
            }
        }

        /// Passable Fruit Tree Saplings Patch
        private static void PassableFruitSaplings_Patch(FruitTree __instance, ref bool __result, ref float ___maxShake, ref NetBool ___shakeLeft, Character c)
        {
            if (ModEntry.Config.Passable_saplings)
            {
                try
                {
                    if (__instance.growthStage.Value < 4)
                    {
                        var farmer = c as Farmer;
                        if (farmer != null)
                        {
                            __result = true;
                            if (farmer is not null && ModEntry.Config.Slowdown_when_passing)
                            {
                                if (farmer.stats.Get("Book_Grass") == 0)
                                {
                                    farmer.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                                }
                                else
                                {
                                    farmer.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                                }
                            }
                            if ((ModEntry.Config.Shake_when_passing == true) && c is not null && ___maxShake == 0f)
                            {
                                ___shakeLeft.Value = c.StandingPixel.X > (__instance.Tile.X + 0.5f) * 64f || (c.Tile.X == __instance.Tile.X && Game1.random.NextBool());
                                ___maxShake = (float)(Math.PI / 64.0);
                                PlayRustleSound(__instance.Tile, __instance.Location);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableFruitSaplings_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Fruit Tree Shaking
        private static void Prefix_Draw_Fruit_Tree()
        {
            isDrawing = true;
        }
        private static void Postfix_Fruit_Tree_BoundingBox(FruitTree __instance, ref Rectangle __result)
        {
            if (isDrawing)
            {
                isDrawing = false;
                var skew = __instance.growthStage.Value switch
                {
                    0 => -36,
                    _ => 0
                };
                __result = new Rectangle(__result.X, __result.Y + skew, __result.Width, __result.Height);
            }
        }

        /// Passable Weeds Patch
        private static void PassableWeeds_Patch(StardewValley.Object __instance, ref bool __result)
        {
            if (ModEntry.Config.Passable_weeds)
            {
                try
                {
                    if (__instance.IsWeeds() && __instance.QualifiedItemId != "(O)321" && __instance.QualifiedItemId != "(O)320" && __instance.QualifiedItemId != "(O)319")
                    {
                        var character = Last_shake_data.Character = Game1.player;
                        var character_bounding_box = character.GetBoundingBox();
                        var object_bounding_box = __instance.GetBoundingBoxAt((int)__instance.TileLocation.X, (int)__instance.TileLocation.Y);
                        var character_bounding_box_enlarged = character_bounding_box.Clone();
                        character_bounding_box_enlarged.Inflate(16, 16);
                        var intersect = character_bounding_box_enlarged.Intersects(object_bounding_box);
                        Last_shake_data.Passable = intersect;
                        __result = character_bounding_box_enlarged.Intersects(object_bounding_box);

                        if (character_bounding_box.Intersects(object_bounding_box))
                        {
                            if (ModEntry.Config.Slowdown_when_passing && character == Game1.player)
                            {
                                if (Game1.player.stats.Get("Book_Grass") == 0)
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                                }
                                else
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                                }
                            }
                            if (Last_shake_data.Character is not null && (!__instance!.modData.TryGetValue(KeyDataShake, out var data) || !float.TryParse((data = "").Split(';')[0], out var maxShake) || maxShake <= 0f))
                            {
                                maxShake = Shake_max_stiff;
                                var shakeLeft = character_bounding_box.Center.X > __instance.TileLocation.X * 64f + 32f;
                                var shakeRotation = 0f;
                                __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                                PlayRustleSound(__instance.TileLocation, __instance.Location);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableWeeds_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }

        /// Passable Forage Patch
        private static void PassableForage_Patch(StardewValley.Object __instance, ref bool __result)
        {
            if (ModEntry.Config.Passable_forage)
            {
                try
                {
                    if (__instance.isForage())
                    {
                        var character = Last_shake_data.Character = Game1.player;
                        var character_bounding_box = character.GetBoundingBox();
                        var object_bounding_box = __instance.GetBoundingBoxAt((int)__instance.TileLocation.X, (int)__instance.TileLocation.Y);
                        var character_bounding_box_enlarged = character_bounding_box.Clone();
                        character_bounding_box_enlarged.Inflate(16, 16);
                        var intersect = character_bounding_box_enlarged.Intersects(object_bounding_box);
                        Last_shake_data.Passable = intersect;
                        __result = character_bounding_box_enlarged.Intersects(object_bounding_box);

                        if (character_bounding_box.Intersects(object_bounding_box))
                        {
                            if (ModEntry.Config.Slowdown_when_passing && character == Game1.player)
                            {
                                if (Game1.player.stats.Get("Book_Grass") == 0)
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_amount;
                                }
                                else
                                {
                                    Game1.player.temporarySpeedBuff = ModEntry.Config.Slowdown_when_passing_book_amount;
                                }
                            }
                            if (Last_shake_data.Character is not null && (!__instance!.modData.TryGetValue(KeyDataShake, out var data) || !float.TryParse((data = "").Split(';')[0], out var maxShake) || maxShake <= 0f))
                            {
                                maxShake = Shake_max_stiff;
                                var shakeLeft = character_bounding_box.Center.X > __instance.TileLocation.X * 64f + 32f;
                                var shakeRotation = 0f;
                                __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                                PlayRustleSound(__instance.TileLocation, __instance.Location);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /// If an error occurs, log it
                    ModEntry.TheMonitor.Log($"Failed in {nameof(PassableForage_Patch)}:\n{e}", LogLevel.Error);
                }
            }
        }
    }
}
