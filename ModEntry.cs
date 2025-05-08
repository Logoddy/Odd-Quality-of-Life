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
            Helper.Events.GameLoop.UpdateTicked += Patches.Utility.GameLoop_UpdateTicked;
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
                text: () => "Odd Quality of Life!"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Welcome to the Odd Quality of Life config page! Please enable, disable or modify the below configs for the modules this mod provides."
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_friendship_decay",
                text: () => "Friendship Decay Module"
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_fence_decay",
                text: () => "Fence Decay Module"
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_home",
                text: () => "Home Life Module"
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_passable_objects",
                text: () => "Passable Objects Module"
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_fishing",
                text: () => "Fishing Module"
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_ranching",
                text: () => "Ranching Module"
            );
            configMenu.AddPageLink
            (
                mod: ModManifest,
                pageId: "odd_utility",
                text: () => "Utility Module"
            );
            /// Friendship Decay Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_friendship_decay",
                pageTitle: () => "Friendship Decay"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Friendship Decay Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to preventing friendship decay for villagers or farm animals"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Spouse Decay",
                tooltip: () => "Prevent spouse friendship decay",
                getValue: () => Config.Spouse_decay,
                setValue: value => Config.Spouse_decay = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Dating Decay",
                tooltip: () => "Prevent dating partner friendship decay",
                getValue: () => Config.Dating_decay,
                setValue: value => Config.Dating_decay = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Other Decay",
                tooltip: () => "Prevent all other friendship decay",
                getValue: () => Config.Other_decay,
                setValue: value => Config.Other_decay = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Animal Decay",
                tooltip: () => "Prevent farm animal friendship decay",
                getValue: () => Config.Animal_decay,
                setValue: value => Config.Animal_decay = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Debugging Mode",
                tooltip: () => "Enable Debugging for Friendship",
                getValue: () => Config.Enable_debugging_friendship,
                setValue: value => Config.Enable_debugging_friendship = value
            );
            /// Fence Decay Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_fence_decay",
                pageTitle: () => "Fence Decay"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Fence Decay Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to modifying decay over time for fences"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Fence Decay",
                tooltip: () => "Enable fences decay modifications",
                getValue: () => Config.Fence_decay,
                setValue: value => Config.Fence_decay = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Fence Decay Speed",
                tooltip: () => "Adjust the percentage speed in which fences decay compared to vanilla. (0 = no decay, 100 = vanilla, 200 = twice the decay speed)",
                getValue: () => Config.Fence_decay_speed,
                setValue: value => Config.Fence_decay_speed = value,
                min: 0,
                max: 300
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Enable Debugging",
                tooltip: () => "Enable Debugging for Fence Decay",
                getValue: () => Config.Enable_debugging_fence_decay,
                setValue: value => Config.Enable_debugging_fence_decay = value
            );
            /// Passable Items Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_passable_objects",
                pageTitle: () => "Passable Objects"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Passable Objects Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to modifying the ability to pass by certain objects"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Sprinklers",
                tooltip: () => "Enable the ability to walk through/past sprinklers",
                getValue: () => Config.Passable_sprinklers,
                setValue: value => Config.Passable_sprinklers = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Scarecrows",
                tooltip: () => "Enable the ability to walk through/past scarecrows",
                getValue: () => Config.Passable_scarecrows,
                setValue: value => Config.Passable_scarecrows = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Tree Saplings",
                tooltip: () => "Enable the ability to walk through/past tree saplings",
                getValue: () => Config.Passable_saplings,
                setValue: value => Config.Passable_saplings = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Crops",
                tooltip: () => "Enable the ability to walk through/past crops",
                getValue: () => Config.Passable_crops,
                setValue: value => Config.Passable_crops = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Bushes",
                tooltip: () => "Enable the ability to walk through/past bushes",
                getValue: () => Config.Passable_bushes,
                setValue: value => Config.Passable_bushes = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Weeds",
                tooltip: () => "Enable the ability to walk through/past weeds",
                getValue: () => Config.Passable_weeds,
                setValue: value => Config.Passable_weeds = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Passable Forage",
                tooltip: () => "Enable the ability to walk through/past forage",
                getValue: () => Config.Passable_forage,
                setValue: value => Config.Passable_forage = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Slowdown Effect",
                tooltip: () => "Enable a character movement slowdown effect when walking through objects",
                getValue: () => Config.Slowdown_when_passing,
                setValue: value => Config.Slowdown_when_passing = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Shaking Effect",
                tooltip: () => "Enable a shaking effect on the sprite when walking through objects",
                getValue: () => Config.Shake_when_passing,
                setValue: value => Config.Shake_when_passing = value
            );
            /// Home Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_home",
                pageTitle: () => "Home Life"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Home Life Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to home life"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Automatic Fireplaces",
                tooltip: () => "Automatically turns fireplaces on at certain times",
                getValue: () => Config.Auto_fireplace,
                setValue: value => Config.Auto_fireplace = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Automatic Fireplace Dousing",
                tooltip: () => "Automatically turns fireplaces off in the morning",
                getValue: () => Config.Auto_fireplace_douse,
                setValue: value => Config.Auto_fireplace_douse = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Spring)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during spring",
                getValue: () => Config.Auto_fireplace_spring,
                setValue: value => Config.Auto_fireplace_spring = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Spring/Rain)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during spring while raining",
                getValue: () => Config.Auto_fireplace_spring_rain,
                setValue: value => Config.Auto_fireplace_spring_rain = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Summer)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during summer",
                getValue: () => Config.Auto_fireplace_summer,
                setValue: value => Config.Auto_fireplace_summer = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Summer/Rain)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during summer while raining",
                getValue: () => Config.Auto_fireplace_summer_rain,
                setValue: value => Config.Auto_fireplace_summer_rain = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Fall)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during fall",
                getValue: () => Config.Auto_fireplace_fall,
                setValue: value => Config.Auto_fireplace_fall = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Fall/Rain)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during fall while raining",
                getValue: () => Config.Auto_fireplace_fall_rain,
                setValue: value => Config.Auto_fireplace_fall_rain = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Winter)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during winter",
                getValue: () => Config.Auto_fireplace_winter,
                setValue: value => Config.Auto_fireplace_winter = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Auto Fireplace Time (Winter/Snow)",
                tooltip: () => "Sets the time in which fireplaces will automatically turn on during winter while snowing",
                getValue: () => Config.Auto_fireplace_winter_snow,
                setValue: value => Config.Auto_fireplace_winter_snow = value
            );
            /// Fishing Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_fishing",
                pageTitle: () => "Fishing"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Fishing Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to Fishing"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Unlimited Legendary Fish",
                tooltip: () => "Makes Legendary fish able to be caught an unlimited number of times",
                getValue: () => Config.Unlimited_legendary_fish,
                setValue: value => Config.Unlimited_legendary_fish = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Fishing Auto Hook",
                tooltip: () => "Makes Fishing rods instantly hook fish",
                getValue: () => Config.Fishing_hook_assist,
                setValue: value => Config.Fishing_hook_assist = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Max Cast Fishing",
                tooltip: () => "Makes Fishing rods always cast at max strength",
                getValue: () => Config.Fishing_max_cast_assist,
                setValue: value => Config.Fishing_max_cast_assist = value
            );
            /// Ranching Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_ranching",
                pageTitle: () => "Ranching"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Ranching Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to Ranching"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Prevent Failed Milking/Sheering",
                tooltip: () => "Prevent the use of the milk bucket or sheers when they can't be used",
                getValue: () => Config.Prevent_Failed_Animal_Harvest,
                setValue: value => Config.Prevent_Failed_Animal_Harvest = value
            );
            /// Utility Module
            configMenu.AddPage
            (
                mod: ModManifest,
                pageId: "odd_utility",
                pageTitle: () => "Utility"
            );
            configMenu.AddSectionTitle
            (
                mod: ModManifest,
                text: () => "Utility Module"
            );
            configMenu.AddParagraph
            (
                mod: ModManifest,
                text: () => "Config options pertaining to general Utilities"
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "AFK Protection",
                tooltip: () => "Enable the game to automatically freeze time when no actions are detected for a specific amount of time",
                getValue: () => Config.AFK_protection,
                setValue: value => Config.AFK_protection = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "AFK Timer",
                tooltip: () => "Enable the game to automatically freeze time when no actions are detected for a specific amount of time",
                getValue: () => Config.AFK_timer,
                setValue: value => Config.AFK_timer = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "AFK Notification",
                tooltip: () => "Display a notification banner stating that you are AFK",
                getValue: () => Config.AFK_notification,
                setValue: value => Config.AFK_notification = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "AFK Mouse Release",
                tooltip: () => "Disable the ability for simple mouse movement to deactivate AFK mode",
                getValue: () => Config.AFK_release_mouse,
                setValue: value => Config.AFK_release_mouse = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Safe Passout on Farm",
                tooltip: () => "Allows farmers to safely passout on the farm without losing gold",
                getValue: () => Config.Passout_protection_farm,
                setValue: value => Config.Passout_protection_farm = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Safe Passout Everywhere",
                tooltip: () => "Allows farmers to safely passout anywhere without losing gold",
                getValue: () => Config.Passout_protection_everywhere,
                setValue: value => Config.Passout_protection_everywhere = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Safe Passout Penalty",
                tooltip: () => "Sets the stamina penalty divisor for passing out in a safe area (2 will set your stamina to half its maximum; 0 will give no stamina penalty)",
                getValue: () => Config.Passout_protection_stamina_penalty,
                setValue: value => Config.Passout_protection_stamina_penalty = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Auto Stack Fishing Rod Bait",
                tooltip: () => "Automatically add matching bait to fishing rods that already contain the same bait",
                getValue: () => Config.Auto_stack_fishing_bait,
                setValue: value => Config.Auto_stack_fishing_bait = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Auto Stack Slingshot Ammo",
                tooltip: () => "Automatically add matching ammo to fislingshots that already contain the same ammo",
                getValue: () => Config.Auto_stack_slingshot_ammo,
                setValue: value => Config.Auto_stack_slingshot_ammo = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Festival Notifications Heads Up",
                tooltip: () => "Sends a heads up notification at the beginning of the festival day",
                getValue: () => Config.Festival_notifications_heads_up,
                setValue: value => Config.Festival_notifications_heads_up = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Festival Notifications Warning",
                tooltip: () => "Sends a warning notification at a determined time prior to the festival ending",
                getValue: () => Config.Festival_notifications_warning,
                setValue: value => Config.Festival_notifications_warning = value
            );
            configMenu.AddNumberOption
            (
                mod: ModManifest,
                name: () => "Festival Notifications Warning Timing",
                tooltip: () => "Sets the amount of hours prior to a festivals end time to send a warning",
                getValue: () => Config.Festival_notifications_warning_time,
                setValue: value => Config.Festival_notifications_warning_time = value
            );
            configMenu.AddBoolOption
            (
                mod: ModManifest,
                name: () => "Non-Destructive NPC's",
                tooltip: () => "Prevents NPC's from walking into and desroying objects placed by the player outside the farm",
                getValue: () => Config.Non_Destructive_NPCS,
                setValue: value => Config.Non_Destructive_NPCS = value
            );
        }        
    }
}
