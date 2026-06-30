using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;
using xTile.Layers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Linq;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Locations;
using StardewValley.GameData.Shops;
using HarmonyLib;
using StardewValley.GameData.Tools;
using StardewValley.Menus;
using static StardewValley.Menus.CarpenterMenu;

namespace Odd_Quality_of_Life.Patches
{
    internal class Shopping
    {     
        /// Pierre Sells More Seeds/Saplings
        public static void PierreSellsMoreSeedsAndSaplings(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                e.Edit(asset =>
                {
                    var shopData = asset.AsDictionary<string, ShopData>().Data;
                    var seedShop = shopData["SeedShop"];                    
                    if (ModEntry.Config.More_Seeds_Pierre_Strawberries)
                    {
                        var strawberry_price = ModEntry.Config.More_Seeds_Pierre_Strawberries_Price;
                        var strawberry_unlock = "SEASON spring";
                        if (ModEntry.Config.More_Seeds_Pierre_Strawberries_Unlock == "Progression")
                        {
                            strawberry_unlock = "SEASON spring, YEAR 2";
                        }
                        seedShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Strawberries",
                            ItemId = "(O)745",
                            Price = strawberry_price,
                            Condition = strawberry_unlock
                        });
                    }
                    if (ModEntry.Config.More_Saplings_Pierre_Tea_Sapling)
                    {
                        var tea_sapling_price = ModEntry.Config.More_Saplings_Pierre_Tea_Sapling_Price;
                        var tea_sapling_unlock = "YEAR 1 1";
                        if (ModEntry.Config.More_Saplings_Pierre_Tea_Sapling_Unlock == "Progression")
                        {
                            tea_sapling_unlock = "PLAYER_HAS_CRAFTING_RECIPE Current Tea Sapling";
                        }
                        seedShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_TeaSapling",
                            ItemId = "(O)251",
                            Price = tea_sapling_price,
                            Condition = tea_sapling_unlock
                        });
                    }
                });
            }
            else
            {
                return;
            }
        }

        /// Willy Buys Fishing Rods
        public static void WillyBuysFishingRods(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                if (ModEntry.Config.Willy_Buys_Fishing_Rods)
                {
                    e.Edit(asset =>
                    {
                        var shopData = asset.AsDictionary<string, ShopData>().Data;
                        var fishShop = shopData["FishShop"];
                        fishShop.SalableItemTags.Add(new("item_bamboopole"));
                        fishShop.SalableItemTags.Add(new("item_trainingrod"));
                        fishShop.SalableItemTags.Add(new("item_fiberglassrod"));
                        fishShop.SalableItemTags.Add(new("item_iridiumrod"));
                    });
                }
            }            
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Tools", false))
            {
                e.Edit(asset =>
                {
                    var toolData = asset.AsDictionary<string, ToolData>().Data;
                    foreach ((string itemID, ToolData itemData) in toolData)
                    {                        
                        var toolName = itemData.Name;
                        if (toolName == "Training Rod")
                        {
                            itemData.SalePrice = 25;
                        }
                        else if (toolName == "Fiberglass Rod")
                        {
                            itemData.SalePrice = 1800;
                        }
                        else if (toolName == "Iridium Rod")
                        {
                            itemData.SalePrice = 7500;
                        }
                        else if (toolName == "Advanced Iridium Rod")
                        {
                            itemData.SalePrice = 25000;
                        }
                    }
                });
            }
        }

        /// Marnie Sells More Materials
        public static void MarnieSellsMoreMaterials(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                e.Edit(asset =>
                {
                    var shopData = asset.AsDictionary<string, ShopData>().Data;
                    var animalShop = shopData["AnimalShop"];
                    if (ModEntry.Config.More_Materials_Marnie_Fiber)
                    {
                        var fiber_price = ModEntry.Config.More_Materials_Marnie_Fiber_Price_Base;
                        animalShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Fiber_Year1",
                            ItemId = "(O)771",
                            Price = fiber_price,
                            Condition = "YEAR 1 1"
                        });
                        animalShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Fiber_Year2",
                            ItemId = "(O)771",
                            Price = (fiber_price * 2),
                            Condition = "YEAR 2"
                        });
                    }
                });
            }
            else
            {
                return;
            }
        }

        /// Robin Sells More Materials
        public static void RobinSellsMoreMaterials(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                e.Edit(asset =>
                {
                var shopData = asset.AsDictionary<string, ShopData>().Data;
                var carpenterShop = shopData["Carpenter"];
                    if (ModEntry.Config.More_Materials_Robin_Clay)
                    {
                        var clay_price = ModEntry.Config.More_Materials_Robin_Clay_Price_Base;                        
                        carpenterShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Clay_Year1",
                            ItemId = "(O)330",
                            Price = clay_price,
                            Condition = "YEAR 1 1"
                        });
                        carpenterShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Clay_Year2",
                            ItemId = "(O)330",
                            Price = (clay_price *2),
                            Condition = "YEAR 2"
                        });
                    }
                    if (ModEntry.Config.More_Materials_Robin_Hardwood)
                    {
                        var hardwood_price = ModEntry.Config.More_Materials_Robin_Hardwood_Price_Base;
                        var hardwood_unlock = "YEAR 1 1";
                        var hardwood_unlock_2 = "YEAR 2";
                        if (ModEntry.Config.More_Materials_Robin_Hardwood_Unlock == "Progression")
                        {
                            hardwood_unlock = "PLAYER_VISITED_LOCATION Current Woods, YEAR 1 1";
                            hardwood_unlock_2 = "PLAYER_VISITED_LOCATION Current Woods, YEAR 2";
                        }
                        carpenterShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Hardwood_Year1",
                            ItemId = "(O)709",
                            Price = hardwood_price,
                            Condition = hardwood_unlock
                        });
                        carpenterShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Hardwood_Year2",
                            ItemId = "(O)709",
                            Price = (hardwood_price * 2),
                            Condition = hardwood_unlock_2
                        });
                    }
                    if (ModEntry.Config.More_Materials_Robin_Battery_Pack)
                    {
                        var battery_price = ModEntry.Config.More_Materials_Robin_Battery_Pack_Price;
                        var battery_unlock = "YEAR 1 1";
                        if (ModEntry.Config.More_Materials_Robin_Battery_Pack_Unlock == "Progression")
                        {
                            battery_unlock = "YEAR 2";
                        }
                        carpenterShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Battery_Pack",
                            ItemId = "(O)787",
                            Price = battery_price,
                            Condition = battery_unlock
                        });
                    }
                });
            }
            else
            {
                return;
            }           
        }

        /// Purchase Missing Materials (Robin)        

        /// Clint Sells More Materials
        public static void ClintSellsMoreMaterials(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                e.Edit(asset =>
                {
                    var shopData = asset.AsDictionary<string, ShopData>().Data;
                    var blacksmithShop = shopData["Blacksmith"];
                    if (ModEntry.Config.More_Materials_Clint_Iridium_Ore)
                    {
                        var iridium_ore_price = ModEntry.Config.More_Materials_Clint_Iridium_Ore_Price_Base;
                        var iridium_ore_unlock = "YEAR 1 1";
                        var iridium_ore_unlock_2 = "YEAR 2";
                        if (ModEntry.Config.More_Materials_Clint_Iridium_Ore_Unlock == "Progression")
                        {
                            iridium_ore_unlock = "MINE_LOWEST_LEVEL_REACHED 120, YEAR 1 1";
                            iridium_ore_unlock_2 = "MINE_LOWEST_LEVEL_REACHED 120, YEAR 2";
                        }
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Iridium_Ore_Year1",
                            ItemId = "(O)386",
                            Price = iridium_ore_price,
                            Condition = iridium_ore_unlock
                        });
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Iridium_Ore_Year2",
                            ItemId = "(O)386",
                            Price = (iridium_ore_price * 2),
                            Condition = iridium_ore_unlock_2
                        });
                    }
                    if (ModEntry.Config.More_Materials_Clint_Radioactive_Ore)
                    {
                        var radioactive_ore_price = ModEntry.Config.More_Materials_Clint_Radioactive_Ore_Price_Base;
                        var radioactive_ore_unlock = "YEAR 1 1";
                        var radioactive_ore_unlock_2 = "YEAR 2";
                        if (ModEntry.Config.More_Materials_Clint_Radioactive_Ore_Unlock == "Progression")
                        {
                            radioactive_ore_unlock = "PLAYER_HAS_ACHIEVEMENT Current 41, YEAR 1 1";
                            radioactive_ore_unlock_2 = "PLAYER_HAS_ACHIEVEMENT Current 41, YEAR 2";
                        }
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Radioactive_Ore_Year1",
                            ItemId = "(O)909",
                            Price = radioactive_ore_price,
                            Condition = radioactive_ore_unlock
                        });
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Radioactive_Ore_Year2",
                            ItemId = "(O)909",
                            Price = (radioactive_ore_price * 2),
                            Condition = radioactive_ore_unlock_2
                        });
                    }
                });
            }
            else
            {
                return;
            }
        }

        /// Clint Sells Spare Tools
        public static void ClintSellsSpareTools(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                e.Edit(asset =>
                {
                    var shopData = asset.AsDictionary<string, ShopData>().Data;
                    var blacksmithShop = shopData["Blacksmith"];
                    if (ModEntry.Config.Clint_Sells_Spare_Tools)
                    {
                        var spare_tools_price = ModEntry.Config.Clint_Sells_Spare_Tools_Price_Base;
                        var spare_tools_unlock = "YEAR 1 1";
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Spare_Pick",
                            ItemId = "(T)Pickaxe",
                            Price = spare_tools_price,
                            Condition = spare_tools_unlock
                        });
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Spare_Axe",
                            ItemId = "(T)Axe",
                            Price = spare_tools_price,
                            Condition = spare_tools_unlock
                        });
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Spare_Hoe",
                            ItemId = "(T)Hoe",
                            Price = spare_tools_price,
                            Condition = spare_tools_unlock
                        });
                        blacksmithShop.Items.Add(new()
                        {
                            Id = ModEntry.MOD_ID + "_Spare_Watering_Can",
                            ItemId = "(T)WateringCan",
                            Price = spare_tools_price,
                            Condition = spare_tools_unlock
                        });
                    }
                });
            }
            else
            {
                return;
            }
        }

            /// Purchase Missing Materials (Clint)
        }
}
