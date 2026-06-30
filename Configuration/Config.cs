using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odd_Quality_of_Life.Configuration
{
    /// Create the config model
    public sealed class ModConfig
    {
        /// Friendship Decay Module
        public bool Spouse_decay { get; set; } = true;
        public bool Dating_decay { get; set; } = true;
        public bool Other_decay { get; set; } = true;
        public bool Animal_decay { get; set; } = false;
        public bool Pet_Bowl_Fill { get; set; } = false;
        public bool Enable_debugging_friendship { get; set; } = false;

        /// Fence Decay Module
        public bool Fence_decay { get; set; } = true;
        public int Fence_decay_speed { get; set; } = 50;
        public bool Enable_debugging_fence_decay { get; set; } = false;

        /// Passable Objects Module
        public bool Passable_sprinklers { get; set; } = true;
        public bool Passable_scarecrows { get; set; } = true;
        public bool Passable_saplings { get; set; } = true;
        public bool Passable_crops { get; set; } = true;
        public bool Passable_bushes { get; set; } = true;
        public bool Passable_weeds { get; set; } = true;
        public bool Passable_forage { get; set; } = true;
        public bool Slowdown_when_passing { get; set; } = true;
        public float Slowdown_when_passing_amount { get; set; } = -1f;
        public float Slowdown_when_passing_book_amount { get; set; } = -0.33f;
        public bool Shake_when_passing { get; set; } = true;

        /// Fishing Module 
        public int Slightly_Limited_legendary_fish { get; set; } = 1;
        public bool Fishing_hook_assist { get; set; } = false;
        public bool Fishing_max_cast_assist { get; set; } = false;

        /// Home Module
        public bool Auto_fireplace { get; set; } = true;
        public int Auto_fireplace_spring { get; set; } = 2000;
        public int Auto_fireplace_summer { get; set; } = 2100;
        public int Auto_fireplace_fall { get; set; } = 2000;
        public int Auto_fireplace_winter { get; set; } = 1800;
        public int Auto_fireplace_spring_rain { get; set; } = 1700;
        public int Auto_fireplace_summer_rain { get; set; } = 1700;
        public int Auto_fireplace_fall_rain { get; set; } = 1700;
        public int Auto_fireplace_winter_snow { get; set; } = 600;
        public bool Auto_fireplace_douse { get; set; } = true;

        /// Ranching Module
        public bool Prevent_Failed_Animal_Harvest { get; set; } = true;

        /// Animal Doors - General
        public bool Animal_doors_auto_open { get; set; } = true;
        public bool Animal_doors_auto_close { get; set; } = true;
        public bool Animal_doors_sound { get; set; } = true;

        /// Animal Doors - Building Requirements
        public int Animal_doors_coop_level { get; set; } = 1;
        public int Animal_doors_barn_level { get; set; } = 1;
        public bool Animal_doors_sve_premium_barn { get; set; } = true;
        public bool Animal_doors_sve_premium_coop { get; set; } = true;
        public bool Animal_doors_other_buildings { get; set; } = true;

        /// Animal Doors - Seasonal Timing
        public int Animal_doors_spring_open { get; set; } = 730;
        public int Animal_doors_spring_close { get; set; } = 1800;
        public int Animal_doors_summer_open { get; set; } = 630;
        public int Animal_doors_summer_close { get; set; } = 1900;
        public int Animal_doors_fall_open { get; set; } = 730;
        public int Animal_doors_fall_close { get; set; } = 1800;
        public int Animal_doors_winter_open { get; set; } = 800;
        public int Animal_doors_winter_close { get; set; } = 1700;

        /// Animal Doors - Weather Overrides
        public string Animal_doors_rain_mode { get; set; } = "Block";
        public int Animal_doors_rain_open { get; set; } = 900;
        public int Animal_doors_rain_close { get; set; } = 1700;
        public string Animal_doors_storm_mode { get; set; } = "Block";
        public int Animal_doors_storm_open { get; set; } = 900;
        public int Animal_doors_storm_close { get; set; } = 1700;
        public string Animal_doors_snow_mode { get; set; } = "Block";
        public int Animal_doors_snow_open { get; set; } = 900;
        public int Animal_doors_snow_close { get; set; } = 1700;
        public string Animal_doors_wind_mode { get; set; } = "Default";
        public int Animal_doors_wind_open { get; set; } = 730;
        public int Animal_doors_wind_close { get; set; } = 1800;
        public string Animal_doors_green_rain_mode { get; set; } = "Block";
        public int Animal_doors_green_rain_open { get; set; } = 900;
        public int Animal_doors_green_rain_close { get; set; } = 1700;
        public string Animal_doors_blocked_weather_ids { get; set; } = "";

        /// Shopping              
        public bool More_Seeds_Pierre_Strawberries { get; set; } = true;
        public string More_Seeds_Pierre_Strawberries_Unlock { get; set; } = "Progression";
        public int More_Seeds_Pierre_Strawberries_Price { get; set; } = 150;
        public bool More_Saplings_Pierre_Tea_Sapling { get; set; } = true;
        public string More_Saplings_Pierre_Tea_Sapling_Unlock { get; set; } = "Progression";
        public int More_Saplings_Pierre_Tea_Sapling_Price { get; set; } = 500;
        public bool Willy_Buys_Fishing_Rods { get; set; } = true;
        public bool More_Materials_Marnie_Fiber { get; set; } = true;
        public int More_Materials_Marnie_Fiber_Price_Base { get; set; } = 10;
        public bool More_Materials_Robin_Clay { get; set; } = true;       
        public int More_Materials_Robin_Clay_Price_Base { get; set; } = 75;
        public bool More_Materials_Robin_Hardwood { get; set; } = true;
        public string More_Materials_Robin_Hardwood_Unlock { get; set; } = "Progression";
        public int More_Materials_Robin_Hardwood_Price_Base { get; set; } = 100;
        public bool More_Materials_Robin_Battery_Pack { get; set; } = true;
        public string More_Materials_Robin_Battery_Pack_Unlock { get; set; } = "Progression";
        public int More_Materials_Robin_Battery_Pack_Price { get; set; } = 1000;
        public bool More_Materials_Clint_Iridium_Ore { get; set; } = true;
        public string More_Materials_Clint_Iridium_Ore_Unlock { get; set; } = "Progression";
        public int More_Materials_Clint_Iridium_Ore_Price_Base { get; set; } = 750;
        public bool More_Materials_Clint_Radioactive_Ore { get; set; } = true;
        public string More_Materials_Clint_Radioactive_Ore_Unlock { get; set; } = "Progression";
        public int More_Materials_Clint_Radioactive_Ore_Price_Base { get; set; } = 1250;
        public bool Clint_Sells_Spare_Tools { get; set; } = true;
        public int Clint_Sells_Spare_Tools_Price_Base { get; set; } = 1000;

        /// Utility Module
        public bool AFK_protection { get; set; } = true;
        public int AFK_timer { get; set; } = 2000;
        public bool AFK_notification { get; set; } = true;
        public bool AFK_release_mouse { get; set; } = true;
        public bool Passout_protection_farm { get; set;} = true;
        public bool Passout_protection_everywhere { get; set; } = false;
        public int Passout_protection_stamina_penalty { get; set; } = 2;
        public bool Auto_stack_fishing_bait { get; set; } = true;
        public bool Auto_stack_slingshot_ammo { get; set; } = true;
        public bool Festival_notifications_heads_up { get; set; } = true;
        public bool Festival_notifications_warning { get; set; } = true;
        public int Festival_notifications_warning_time { get; set; } = 2;
        public bool Non_Destructive_NPCS { get; set; } = true;
        public bool Prevent_Lightning_Strike_Sound { get; set; } = false;
        public bool Prevent_Thunder_Sound { get; set; } = false;
        public bool Non_Destructive_Lightning_Fruit_Trees { get; set; } = false;
        public bool Non_Destructive_Lightning_Objects { get; set; } = true;
        public bool Sit_to_Regen { get; set; } = false;
        public float Sit_to_Regen_Rate { get; set; } = 50;
        public float Sit_to_Regen_Rate_Maximum { get; set; } = 75;

        /// Auto Gates
        public bool Auto_gates_on_foot { get; set; } = true;
        public bool Auto_gates_on_horse { get; set; } = true;
        public bool Auto_gates_facing_only { get; set; } = true;
        public bool Auto_gates_open_sound { get; set; } = true;
        public bool Auto_gates_auto_close { get; set; } = true;
        public bool Auto_gates_close_sound { get; set; } = true;
        public int Auto_gates_close_delay { get; set; } = 15;
    }
}
