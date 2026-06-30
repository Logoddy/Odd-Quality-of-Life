using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using Odd_Quality_of_Life.Configuration;
using Odd_Quality_of_Life.Patches;

namespace Odd_Quality_of_Life
{
    public class ModEntry : Mod
    {
        /// The Config!!
        public static ModConfig Config;

        /// WhoAmI
        public const string MOD_ID = "Lt_oddy.Odd_Quality_of_Life";

        /// The All Seeing Eye
        internal static IMonitor TheMonitor { get; private set; } = null;

        /// The Assistant
        internal static IModHelper TheHelper { get; private set; } = null;

        /// Create the helper and start the magic!
        public override void Entry(IModHelper helper)
        {
            TheMonitor = this.Monitor;
            TheHelper = this.Helper;
            Config = Helper.ReadConfig<ModConfig>();
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.DayEnding += Patches.Friendship.EndDay;
            Helper.Events.GameLoop.TimeChanged += Patches.Home.LightFireplace;
            Helper.Events.GameLoop.DayStarted += Patches.Home.StartDay;
            Helper.Events.GameLoop.DayStarted += Patches.Friendship.AutofillPetBowl;
            Helper.Events.GameLoop.UpdateTicked += Patches.Utility.AFK_Protection;
            Helper.Events.GameLoop.UpdateTicked += Patches.Utility.SitToRegen;
            Helper.Events.Input.CursorMoved += Patches.Utility.AFKPlayerInput;
            Helper.Events.Input.MouseWheelScrolled += Patches.Utility.AFKPlayerInput;
            Helper.Events.Input.ButtonPressed += Patches.Utility.AFKPlayerInput;
            Helper.Events.Player.InventoryChanged += Patches.Utility.InventoryUpdated;
            Helper.Events.World.ChestInventoryChanged += Patches.Utility.ChestInventoryUpdated;
            Helper.Events.Content.AssetRequested += Patches.Fishing.Unlimited_Legendary_Fish;
            Helper.Events.GameLoop.UpdateTicked += Patches.Fishing.Max_Cast_Assist;
            Helper.Events.GameLoop.UpdateTicked += Patches.Fishing.Fish_Hook_Assist;
            Helper.Events.GameLoop.UpdateTicked += Patches.Ranching.OnUpdateTicked;
            Helper.Events.Input.ButtonPressed += Patches.Ranching.OnButtonPressed;
            Helper.Events.GameLoop.DayStarted += Patches.UIandAlerts.StartDay;
            Helper.Events.GameLoop.TimeChanged += Patches.UIandAlerts.TimeChanged;
            Helper.Events.Content.AssetRequested += Patches.Shopping.PierreSellsMoreSeedsAndSaplings;
            Helper.Events.Content.AssetRequested += Patches.Shopping.WillyBuysFishingRods;
            Helper.Events.Content.AssetRequested += Patches.Shopping.MarnieSellsMoreMaterials;
            Helper.Events.Content.AssetRequested += Patches.Shopping.RobinSellsMoreMaterials;
            Helper.Events.Content.AssetRequested += Patches.Shopping.ClintSellsMoreMaterials;
            Helper.Events.Content.AssetRequested += Patches.Shopping.ClintSellsSpareTools;
            Helper.Events.GameLoop.DayStarted += Patches.Utility.PassOutPenalty;
            Helper.Events.GameLoop.UpdateTicked += Patches.Utility.AutoGates;
            Helper.Events.GameLoop.ReturnedToTitle += Patches.Utility.AutoGatesClear;
            Helper.Events.GameLoop.TimeChanged += Patches.AnimalDoors.TimeChanged;
            Helper.Events.GameLoop.DayEnding += Patches.AnimalDoors.DayEnding;
            Helper.Events.GameLoop.DayStarted += Patches.AnimalDoors.DayStarted;
            Helper.Events.GameLoop.ReturnedToTitle += Patches.AnimalDoors.ReturnedToTitle;

            /// Harmony Patches
            Harmony harmony = new Harmony(ModManifest.UniqueID);
            Patches.FenceDecay.ApplyPatches(harmony);
            Patches.PassableObjects.ApplyPatches(harmony);
            Patches.Utility.ApplyPatches(harmony);

            /// Check if debugging is enabled
            if (Config.Enable_debugging_friendship)
            {
                Helper.Events.GameLoop.DayStarted += Patches.Friendship.Debugger;
            } 
        }     

        /// Config Menu
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            /// Get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            /// Register mod
            configMenu.Register
            (
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            /// Config Settings
            /// Main Page            
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => Helper.Translation.Get("Config.Main.Title")
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => Helper.Translation.Get("Config.Main.Description")
            );
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_friendship_decay", text: () => Helper.Translation.Get("Config.Main.Link.Friendship"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_fence_decay", text: () => Helper.Translation.Get("Config.Main.Link.Fence"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_home", text: () => Helper.Translation.Get("Config.Main.Link.Home"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_passable_objects", text: () => Helper.Translation.Get("Config.Main.Link.Passable"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_fishing", text: () => Helper.Translation.Get("Config.Main.Link.Fishing"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_ranching", text: () => Helper.Translation.Get("Config.Main.Link.Ranching"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_shopping", text: () => Helper.Translation.Get("Config.Main.Link.Shopping"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_utility", text: () => Helper.Translation.Get("Config.Main.Link.Utility"));

            /// Friendship Decay Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_friendship_decay", pageTitle: () => Helper.Translation.Get("Config.Friendship.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Friendship.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Friendship.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Friendship.SpouseDecay.name"), tooltip: () => Helper.Translation.Get("Config.Friendship.SpouseDecay.description"), getValue: () => Config.Spouse_decay, setValue: value => Config.Spouse_decay = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Friendship.DatingDecay.name"), tooltip: () => Helper.Translation.Get("Config.Friendship.DatingDecay.description"), getValue: () => Config.Dating_decay, setValue: value => Config.Dating_decay = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Friendship.OtherDecay.name"), tooltip: () => Helper.Translation.Get("Config.Friendship.OtherDecay.description"), getValue: () => Config.Other_decay, setValue: value => Config.Other_decay = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Friendship.AnimalDecay.name"), tooltip: () => Helper.Translation.Get("Config.Friendship.AnimalDecay.description"), getValue: () => Config.Animal_decay, setValue: value => Config.Animal_decay = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Friendship.PetBowl.name"), tooltip: () => Helper.Translation.Get("Config.Friendship.PetBowl.description"), getValue: () => Config.Pet_Bowl_Fill, setValue: value => Config.Pet_Bowl_Fill = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Friendship.Debug.name"), tooltip: () => Helper.Translation.Get("Config.Friendship.Debug.description"), getValue: () => Config.Enable_debugging_friendship, setValue: value => Config.Enable_debugging_friendship = value);

            /// Fence Decay Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_fence_decay", pageTitle: () => Helper.Translation.Get("Config.Fence.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Fence.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Fence.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Fence.Decay.name"), tooltip: () => Helper.Translation.Get("Config.Fence.Decay.description"), getValue: () => Config.Fence_decay, setValue: value => Config.Fence_decay = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Fence.Speed.name"), tooltip: () => Helper.Translation.Get("Config.Fence.Speed.description"), getValue: () => Config.Fence_decay_speed, setValue: value => Config.Fence_decay_speed = value, min: 0, max: 300);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Fence.Debug.name"), tooltip: () => Helper.Translation.Get("Config.Fence.Debug.description"), getValue: () => Config.Enable_debugging_fence_decay, setValue: value => Config.Enable_debugging_fence_decay = value);

            /// Passable Objects Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_passable_objects", pageTitle: () => Helper.Translation.Get("Config.Passable.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Passable.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Passable.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Sprinklers.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Sprinklers.description"), getValue: () => Config.Passable_sprinklers, setValue: value => Config.Passable_sprinklers = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Scarecrows.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Scarecrows.description"), getValue: () => Config.Passable_scarecrows, setValue: value => Config.Passable_scarecrows = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Saplings.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Saplings.description"), getValue: () => Config.Passable_saplings, setValue: value => Config.Passable_saplings = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Crops.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Crops.description"), getValue: () => Config.Passable_crops, setValue: value => Config.Passable_crops = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Bushes.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Bushes.description"), getValue: () => Config.Passable_bushes, setValue: value => Config.Passable_bushes = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Weeds.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Weeds.description"), getValue: () => Config.Passable_weeds, setValue: value => Config.Passable_weeds = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Forage.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Forage.description"), getValue: () => Config.Passable_forage, setValue: value => Config.Passable_forage = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Slowdown.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Slowdown.description"), getValue: () => Config.Slowdown_when_passing, setValue: value => Config.Slowdown_when_passing = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.SlowdownAmount.name"), tooltip: () => Helper.Translation.Get("Config.Passable.SlowdownAmount.description"), getValue: () => Config.Slowdown_when_passing_amount, setValue: value => Config.Slowdown_when_passing_amount = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.SlowdownBook.name"), tooltip: () => Helper.Translation.Get("Config.Passable.SlowdownBook.description"), getValue: () => Config.Slowdown_when_passing_book_amount, setValue: value => Config.Slowdown_when_passing_book_amount = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Passable.Shake.name"), tooltip: () => Helper.Translation.Get("Config.Passable.Shake.description"), getValue: () => Config.Shake_when_passing, setValue: value => Config.Shake_when_passing = value);

            /// Home Life Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_home", pageTitle: () => Helper.Translation.Get("Config.Home.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Home.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Home.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.Fireplace.name"), tooltip: () => Helper.Translation.Get("Config.Home.Fireplace.description"), getValue: () => Config.Auto_fireplace, setValue: value => Config.Auto_fireplace = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceDouse.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceDouse.description"), getValue: () => Config.Auto_fireplace_douse, setValue: value => Config.Auto_fireplace_douse = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceSpring.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceSpring.description"), getValue: () => Config.Auto_fireplace_spring, setValue: value => Config.Auto_fireplace_spring = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceSpringRain.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceSpringRain.description"), getValue: () => Config.Auto_fireplace_spring_rain, setValue: value => Config.Auto_fireplace_spring_rain = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceSummer.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceSummer.description"), getValue: () => Config.Auto_fireplace_summer, setValue: value => Config.Auto_fireplace_summer = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceSummerRain.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceSummerRain.description"), getValue: () => Config.Auto_fireplace_summer_rain, setValue: value => Config.Auto_fireplace_summer_rain = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceFall.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceFall.description"), getValue: () => Config.Auto_fireplace_fall, setValue: value => Config.Auto_fireplace_fall = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceFallRain.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceFallRain.description"), getValue: () => Config.Auto_fireplace_fall_rain, setValue: value => Config.Auto_fireplace_fall_rain = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceWinter.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceWinter.description"), getValue: () => Config.Auto_fireplace_winter, setValue: value => Config.Auto_fireplace_winter = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Home.FireplaceWinterSnow.name"), tooltip: () => Helper.Translation.Get("Config.Home.FireplaceWinterSnow.description"), getValue: () => Config.Auto_fireplace_winter_snow, setValue: value => Config.Auto_fireplace_winter_snow = value);

            /// Fishing Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_fishing", pageTitle: () => Helper.Translation.Get("Config.Fishing.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Fishing.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Fishing.Description"));
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Fishing.Legendary.name"), tooltip: () => Helper.Translation.Get("Config.Fishing.Legendary.description"), getValue: () => Config.Slightly_Limited_legendary_fish, setValue: value => Config.Slightly_Limited_legendary_fish = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Fishing.AutoHook.name"), tooltip: () => Helper.Translation.Get("Config.Fishing.AutoHook.description"), getValue: () => Config.Fishing_hook_assist, setValue: value => Config.Fishing_hook_assist = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Fishing.MaxCast.name"), tooltip: () => Helper.Translation.Get("Config.Fishing.MaxCast.description"), getValue: () => Config.Fishing_max_cast_assist, setValue: value => Config.Fishing_max_cast_assist = value);

            /// Ranching Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_ranching", pageTitle: () => Helper.Translation.Get("Config.Ranching.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.PreventFailed.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.PreventFailed.description"), getValue: () => Config.Prevent_Failed_Animal_Harvest, setValue: value => Config.Prevent_Failed_Animal_Harvest = value);

            /// Animal Doors sub-page link
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_ranching_animal_doors", text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.Link"));

            /// Animal Doors - Main Page
            configMenu.AddPage(mod: ModManifest, pageId: "odd_ranching_animal_doors", pageTitle: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.AutoOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.AutoOpen.description"), getValue: () => Config.Animal_doors_auto_open, setValue: value => Config.Animal_doors_auto_open = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.AutoClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.AutoClose.description"), getValue: () => Config.Animal_doors_auto_close, setValue: value => Config.Animal_doors_auto_close = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.Sound.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.Sound.description"), getValue: () => Config.Animal_doors_sound, setValue: value => Config.Animal_doors_sound = value);
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.BuildingsSection"));
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.CoopLevel.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.CoopLevel.description"), getValue: () => Config.Animal_doors_coop_level, setValue: value => Config.Animal_doors_coop_level = value, min: 1, max: 3);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.BarnLevel.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.BarnLevel.description"), getValue: () => Config.Animal_doors_barn_level, setValue: value => Config.Animal_doors_barn_level = value, min: 1, max: 3);

            /// SVE Compatibility
            if (Helper.ModRegistry.IsLoaded("FlashShifter.SVECode"))
            {
                configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SVEPremiumBarn.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SVEPremiumBarn.description"), getValue: () => Config.Animal_doors_sve_premium_barn, setValue: value => Config.Animal_doors_sve_premium_barn = value);
                configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SVEPremiumCoop.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SVEPremiumCoop.description"), getValue: () => Config.Animal_doors_sve_premium_coop, setValue: value => Config.Animal_doors_sve_premium_coop = value);
            }

            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.OtherBuildings.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.OtherBuildings.description"), getValue: () => Config.Animal_doors_other_buildings, setValue: value => Config.Animal_doors_other_buildings = value);
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SeasonalSection"));
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SpringOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SpringOpen.description"), getValue: () => Config.Animal_doors_spring_open, setValue: value => Config.Animal_doors_spring_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SpringClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SpringClose.description"), getValue: () => Config.Animal_doors_spring_close, setValue: value => Config.Animal_doors_spring_close = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SummerOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SummerOpen.description"), getValue: () => Config.Animal_doors_summer_open, setValue: value => Config.Animal_doors_summer_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SummerClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SummerClose.description"), getValue: () => Config.Animal_doors_summer_close, setValue: value => Config.Animal_doors_summer_close = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.FallOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.FallOpen.description"), getValue: () => Config.Animal_doors_fall_open, setValue: value => Config.Animal_doors_fall_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.FallClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.FallClose.description"), getValue: () => Config.Animal_doors_fall_close, setValue: value => Config.Animal_doors_fall_close = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WinterOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WinterOpen.description"), getValue: () => Config.Animal_doors_winter_open, setValue: value => Config.Animal_doors_winter_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WinterClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WinterClose.description"), getValue: () => Config.Animal_doors_winter_close, setValue: value => Config.Animal_doors_winter_close = value);
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_ranching_animal_doors_weather", text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherLink"));

            /// Animal Doors - Weather Overrides Page
            configMenu.AddPage(mod: ModManifest, pageId: "odd_ranching_animal_doors_weather", pageTitle: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherPageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherSectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherDescription"));
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.RainMode.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.RainMode.description"), getValue: () => Config.Animal_doors_rain_mode, setValue: value => Config.Animal_doors_rain_mode = value, allowedValues: new string[] { "Default", "Custom", "Block" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.RainOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_rain_open, setValue: value => Config.Animal_doors_rain_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.RainClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_rain_close, setValue: value => Config.Animal_doors_rain_close = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.StormMode.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.StormMode.description"), getValue: () => Config.Animal_doors_storm_mode, setValue: value => Config.Animal_doors_storm_mode = value, allowedValues: new string[] { "Default", "Custom", "Block" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.StormOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_storm_open, setValue: value => Config.Animal_doors_storm_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.StormClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_storm_close, setValue: value => Config.Animal_doors_storm_close = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SnowMode.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SnowMode.description"), getValue: () => Config.Animal_doors_snow_mode, setValue: value => Config.Animal_doors_snow_mode = value, allowedValues: new string[] { "Default", "Custom", "Block" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SnowOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_snow_open, setValue: value => Config.Animal_doors_snow_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.SnowClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_snow_close, setValue: value => Config.Animal_doors_snow_close = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WindMode.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WindMode.description"), getValue: () => Config.Animal_doors_wind_mode, setValue: value => Config.Animal_doors_wind_mode = value, allowedValues: new string[] { "Default", "Custom", "Block" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WindOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_wind_open, setValue: value => Config.Animal_doors_wind_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WindClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_wind_close, setValue: value => Config.Animal_doors_wind_close = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.GreenRainMode.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.GreenRainMode.description"), getValue: () => Config.Animal_doors_green_rain_mode, setValue: value => Config.Animal_doors_green_rain_mode = value, allowedValues: new string[] { "Default", "Custom", "Block" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.GreenRainOpen.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_green_rain_open, setValue: value => Config.Animal_doors_green_rain_open = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.GreenRainClose.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.WeatherTimeTooltip.description"), getValue: () => Config.Animal_doors_green_rain_close, setValue: value => Config.Animal_doors_green_rain_close = value);
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.ModdedWeatherSection"));
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.BlockedWeatherIds.name"), tooltip: () => Helper.Translation.Get("Config.Ranching.AnimalDoors.BlockedWeatherIds.description"), getValue: () => Config.Animal_doors_blocked_weather_ids, setValue: value => Config.Animal_doors_blocked_weather_ids = value);

            /// Shopping Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_shopping", pageTitle: () => Helper.Translation.Get("Config.Shopping.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Description"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_shopping_pierre", text: () => Helper.Translation.Get("Config.Shopping.Link.Pierre"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_shopping_willy", text: () => Helper.Translation.Get("Config.Shopping.Link.Willy"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_shopping_robin", text: () => Helper.Translation.Get("Config.Shopping.Link.Robin"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_shopping_clint", text: () => Helper.Translation.Get("Config.Shopping.Link.Clint"));
            configMenu.AddPageLink(mod: ModManifest, pageId: "odd_shopping_marnie", text: () => Helper.Translation.Get("Config.Shopping.Link.Marnie"));

            /// Shopping Module - Pierre
            configMenu.AddPage(mod: ModManifest, pageId: "odd_shopping_pierre", pageTitle: () => Helper.Translation.Get("Config.Shopping.Pierre.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Pierre.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Pierre.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Pierre.Strawberry.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Pierre.Strawberry.description"), getValue: () => Config.More_Seeds_Pierre_Strawberries, setValue: value => Config.More_Seeds_Pierre_Strawberries = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Pierre.StrawberryUnlock.name"), getValue: () => Config.More_Seeds_Pierre_Strawberries_Unlock, setValue: value => Config.More_Seeds_Pierre_Strawberries_Unlock = value, allowedValues: new string[] { "Always", "Progression" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Pierre.StrawberryPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Pierre.StrawberryPrice.description"), getValue: () => Config.More_Seeds_Pierre_Strawberries_Price, setValue: value => Config.More_Seeds_Pierre_Strawberries_Price = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Pierre.Tea.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Pierre.Tea.description"), getValue: () => Config.More_Saplings_Pierre_Tea_Sapling, setValue: value => Config.More_Saplings_Pierre_Tea_Sapling = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Pierre.TeaUnlock.name"), getValue: () => Config.More_Saplings_Pierre_Tea_Sapling_Unlock, setValue: value => Config.More_Saplings_Pierre_Tea_Sapling_Unlock = value, allowedValues: new string[] { "Always", "Progression" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Pierre.TeaPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Pierre.TeaPrice.description"), getValue: () => Config.More_Saplings_Pierre_Tea_Sapling_Price, setValue: value => Config.More_Saplings_Pierre_Tea_Sapling_Price = value);

            /// Shopping Module - Willy
            configMenu.AddPage(mod: ModManifest, pageId: "odd_shopping_willy", pageTitle: () => Helper.Translation.Get("Config.Shopping.Willy.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Willy.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Willy.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Willy.BuysRods.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Willy.BuysRods.description"), getValue: () => Config.Willy_Buys_Fishing_Rods, setValue: value => Config.Willy_Buys_Fishing_Rods = value);

            /// Shopping Module - Robin
            configMenu.AddPage(mod: ModManifest, pageId: "odd_shopping_robin", pageTitle: () => Helper.Translation.Get("Config.Shopping.Robin.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Robin.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Robin.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.Clay.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Robin.Clay.description"), getValue: () => Config.More_Materials_Robin_Clay, setValue: value => Config.More_Materials_Robin_Clay = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.ClayPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Robin.ClayPrice.description"), getValue: () => Config.More_Materials_Robin_Clay_Price_Base, setValue: value => Config.More_Materials_Robin_Clay_Price_Base = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.Hardwood.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Robin.Hardwood.description"), getValue: () => Config.More_Materials_Robin_Hardwood, setValue: value => Config.More_Materials_Robin_Hardwood = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.HardwoodUnlock.name"), getValue: () => Config.More_Materials_Robin_Hardwood_Unlock, setValue: value => Config.More_Materials_Robin_Hardwood_Unlock = value, allowedValues: new string[] { "Always", "Progression" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.HardwoodPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Robin.HardwoodPrice.description"), getValue: () => Config.More_Materials_Robin_Hardwood_Price_Base, setValue: value => Config.More_Materials_Robin_Hardwood_Price_Base = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.Battery.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Robin.Battery.description"), getValue: () => Config.More_Materials_Robin_Battery_Pack, setValue: value => Config.More_Materials_Robin_Battery_Pack = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.BatteryUnlock.name"), getValue: () => Config.More_Materials_Robin_Battery_Pack_Unlock, setValue: value => Config.More_Materials_Robin_Battery_Pack_Unlock = value, allowedValues: new string[] { "Always", "Progression" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Robin.BatteryPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Robin.BatteryPrice.description"), getValue: () => Config.More_Materials_Robin_Battery_Pack_Price, setValue: value => Config.More_Materials_Robin_Battery_Pack_Price = value);

            /// Shopping Module - Clint
            configMenu.AddPage(mod: ModManifest, pageId: "odd_shopping_clint", pageTitle: () => Helper.Translation.Get("Config.Shopping.Clint.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Clint.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Clint.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.Iridium.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Clint.Iridium.description"), getValue: () => Config.More_Materials_Clint_Iridium_Ore, setValue: value => Config.More_Materials_Clint_Iridium_Ore = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.IridiumUnlock.name"), getValue: () => Config.More_Materials_Clint_Iridium_Ore_Unlock, setValue: value => Config.More_Materials_Clint_Iridium_Ore_Unlock = value, allowedValues: new string[] { "Always", "Progression" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.IridiumPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Clint.IridiumPrice.description"), getValue: () => Config.More_Materials_Clint_Iridium_Ore_Price_Base, setValue: value => Config.More_Materials_Clint_Iridium_Ore_Price_Base = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.Radioactive.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Clint.Radioactive.description"), getValue: () => Config.More_Materials_Clint_Radioactive_Ore, setValue: value => Config.More_Materials_Clint_Radioactive_Ore = value);
            configMenu.AddTextOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.RadioactiveUnlock.name"), getValue: () => Config.More_Materials_Clint_Radioactive_Ore_Unlock, setValue: value => Config.More_Materials_Clint_Radioactive_Ore_Unlock = value, allowedValues: new string[] { "Always", "Progression" });
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.RadioactivePrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Clint.RadioactivePrice.description"), getValue: () => Config.More_Materials_Clint_Radioactive_Ore_Price_Base, setValue: value => Config.More_Materials_Clint_Radioactive_Ore_Price_Base = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.SpareTools.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Clint.SpareTools.description"), getValue: () => Config.Clint_Sells_Spare_Tools, setValue: value => Config.Clint_Sells_Spare_Tools = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Clint.SpareToolsPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Clint.SpareToolsPrice.description"), getValue: () => Config.Clint_Sells_Spare_Tools_Price_Base, setValue: value => Config.Clint_Sells_Spare_Tools_Price_Base = value);

            /// Shopping Module - Marnie
            configMenu.AddPage(mod: ModManifest, pageId: "odd_shopping_marnie", pageTitle: () => Helper.Translation.Get("Config.Shopping.Marnie.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Marnie.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Shopping.Marnie.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Marnie.Fiber.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Marnie.Fiber.description"), getValue: () => Config.More_Materials_Marnie_Fiber, setValue: value => Config.More_Materials_Marnie_Fiber = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Shopping.Marnie.FiberPrice.name"), tooltip: () => Helper.Translation.Get("Config.Shopping.Marnie.FiberPrice.description"), getValue: () => Config.More_Materials_Marnie_Fiber_Price_Base, setValue: value => Config.More_Materials_Marnie_Fiber_Price_Base = value);

            /// Utility Module
            configMenu.AddPage(mod: ModManifest, pageId: "odd_utility", pageTitle: () => Helper.Translation.Get("Config.Utility.PageTitle"));
            configMenu.AddSectionTitle(mod: ModManifest, text: () => Helper.Translation.Get("Config.Utility.SectionTitle"));
            configMenu.AddParagraph(mod: ModManifest, text: () => Helper.Translation.Get("Config.Utility.Description"));
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AFK.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AFK.description"), getValue: () => Config.AFK_protection, setValue: value => Config.AFK_protection = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AFKTimer.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AFKTimer.description"), getValue: () => Config.AFK_timer, setValue: value => Config.AFK_timer = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AFKNotification.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AFKNotification.description"), getValue: () => Config.AFK_notification, setValue: value => Config.AFK_notification = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AFKMouse.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AFKMouse.description"), getValue: () => Config.AFK_release_mouse, setValue: value => Config.AFK_release_mouse = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.PassoutFarm.name"), tooltip: () => Helper.Translation.Get("Config.Utility.PassoutFarm.description"), getValue: () => Config.Passout_protection_farm, setValue: value => Config.Passout_protection_farm = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.PassoutEverywhere.name"), tooltip: () => Helper.Translation.Get("Config.Utility.PassoutEverywhere.description"), getValue: () => Config.Passout_protection_everywhere, setValue: value => Config.Passout_protection_everywhere = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.PassoutPenalty.name"), tooltip: () => Helper.Translation.Get("Config.Utility.PassoutPenalty.description"), getValue: () => Config.Passout_protection_stamina_penalty, setValue: value => Config.Passout_protection_stamina_penalty = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoBait.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoBait.description"), getValue: () => Config.Auto_stack_fishing_bait, setValue: value => Config.Auto_stack_fishing_bait = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoAmmo.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoAmmo.description"), getValue: () => Config.Auto_stack_slingshot_ammo, setValue: value => Config.Auto_stack_slingshot_ammo = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.FestivalHeadsUp.name"), tooltip: () => Helper.Translation.Get("Config.Utility.FestivalHeadsUp.description"), getValue: () => Config.Festival_notifications_heads_up, setValue: value => Config.Festival_notifications_heads_up = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.FestivalWarning.name"), tooltip: () => Helper.Translation.Get("Config.Utility.FestivalWarning.description"), getValue: () => Config.Festival_notifications_warning, setValue: value => Config.Festival_notifications_warning = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.FestivalWarningTime.name"), tooltip: () => Helper.Translation.Get("Config.Utility.FestivalWarningTime.description"), getValue: () => Config.Festival_notifications_warning_time, setValue: value => Config.Festival_notifications_warning_time = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.NonDestructiveNPCs.name"), tooltip: () => Helper.Translation.Get("Config.Utility.NonDestructiveNPCs.description"), getValue: () => Config.Non_Destructive_NPCS, setValue: value => Config.Non_Destructive_NPCS = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.NoLightningSound.name"), tooltip: () => Helper.Translation.Get("Config.Utility.NoLightningSound.description"), getValue: () => Config.Prevent_Lightning_Strike_Sound, setValue: value => Config.Prevent_Lightning_Strike_Sound = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.NoThunderSound.name"), tooltip: () => Helper.Translation.Get("Config.Utility.NoThunderSound.description"), getValue: () => Config.Prevent_Thunder_Sound, setValue: value => Config.Prevent_Thunder_Sound = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.NonDestructiveLightningTrees.name"), tooltip: () => Helper.Translation.Get("Config.Utility.NonDestructiveLightningTrees.description"), getValue: () => Config.Non_Destructive_Lightning_Fruit_Trees, setValue: value => Config.Non_Destructive_Lightning_Fruit_Trees = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.NonDestructiveLightningObjects.name"), tooltip: () => Helper.Translation.Get("Config.Utility.NonDestructiveLightningObjects.description"), getValue: () => Config.Non_Destructive_Lightning_Objects, setValue: value => Config.Non_Destructive_Lightning_Objects = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.SitRegen.name"), tooltip: () => Helper.Translation.Get("Config.Utility.SitRegen.description"), getValue: () => Config.Sit_to_Regen, setValue: value => Config.Sit_to_Regen = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.SitRegenRate.name"), tooltip: () => Helper.Translation.Get("Config.Utility.SitRegenRate.description"), getValue: () => Config.Sit_to_Regen_Rate, setValue: value => Config.Sit_to_Regen_Rate = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.SitRegenMax.name"), tooltip: () => Helper.Translation.Get("Config.Utility.SitRegenMax.description"), getValue: () => Config.Sit_to_Regen_Rate_Maximum, setValue: value => Config.Sit_to_Regen_Rate_Maximum = value);

            /// Auto Gates
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesOnFoot.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesOnFoot.description"), getValue: () => Config.Auto_gates_on_foot, setValue: value => Config.Auto_gates_on_foot = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesOnHorse.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesOnHorse.description"), getValue: () => Config.Auto_gates_on_horse, setValue: value => Config.Auto_gates_on_horse = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesFacingOnly.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesFacingOnly.description"), getValue: () => Config.Auto_gates_facing_only, setValue: value => Config.Auto_gates_facing_only = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesOpenSound.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesOpenSound.description"), getValue: () => Config.Auto_gates_open_sound, setValue: value => Config.Auto_gates_open_sound = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesAutoClose.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesAutoClose.description"), getValue: () => Config.Auto_gates_auto_close, setValue: value => Config.Auto_gates_auto_close = value);
            configMenu.AddBoolOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesCloseSound.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesCloseSound.description"), getValue: () => Config.Auto_gates_close_sound, setValue: value => Config.Auto_gates_close_sound = value);
            configMenu.AddNumberOption(mod: ModManifest, name: () => Helper.Translation.Get("Config.Utility.AutoGatesDelay.name"), tooltip: () => Helper.Translation.Get("Config.Utility.AutoGatesDelay.description"), getValue: () => Config.Auto_gates_close_delay, setValue: value => Config.Auto_gates_close_delay = value, min: 0, max: 300);
        }
    }
}
