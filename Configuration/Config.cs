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
        public bool Shake_when_passing { get; set; } = true;

        /// Fishing Module 
        public bool Unlimited_legendary_fish { get; set; } = true;
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
    }
}
