using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Linq;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.TerrainFeatures;

namespace Odd_Quality_of_Life.Patches
{
    internal class Ranching
    {
        private static FarmAnimal AnimalTargetted { get; set; }
        public static class ToolCheck
        {
            public struct Tools
            {
                public const string MilkPail = "Milk Pail";
                public const string Shears = "Shears";
            }
        }
        public static bool AnimalRanchable(FarmAnimal animal, string toolName)
        {
            return animal.currentProduce.Value != null && animal.isAdult() && animal.GetAnimalData().HarvestTool == toolName;
        }

        public static bool HoldingHarvestingTool()
        {
            return Game1.player.CurrentTool?.Name is ToolCheck.Tools.MilkPail or ToolCheck.Tools.Shears;
        }

        public static bool IsClickableArea()
        {
            if (Game1.activeClickableMenu != null)
            {
                return false;
            }

            var (x, y) = Game1.getMousePosition();
            return Game1.onScreenMenus.All(screen => !screen.isWithinBounds(x, y));
        }

        public static bool PlayerCanGrabSomething()
        {
            var who = Game1.player;
            GameLocation location = Game1.currentLocation;
            Point tilePoint = who.TilePoint;

            if (Game1.player.canOnlyWalk)
            {
                return true;
            }
            Vector2 position = !Game1.wasMouseVisibleThisFrame ? Game1.player.GetToolLocation() : new Vector2((float)(Game1.getOldMouseX() + Game1.viewport.X), (float)(Game1.getOldMouseY() + Game1.viewport.Y));
            return StardewValley.Utility.canGrabSomethingFromHere((int)position.X, (int)position.Y, Game1.player);
        }

        public static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.Config.Prevent_Failed_Animal_Harvest && HoldingHarvestingTool() && IsClickableArea() && Game1.mouseClickPolling > 50)
            {
                Game1.mouseClickPolling = 50;
            }

            if (!Game1.player.UsingTool && AnimalTargetted != null)
            {
                AnimalTargetted = null;
            }
        }

        public static void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (!e.Button.IsUseToolButton() || !ModEntry.Config.Prevent_Failed_Animal_Harvest || !HoldingHarvestingTool() || !IsClickableArea() || PlayerCanGrabSomething())
            {
                return;
            }
            var who = Game1.player;
            Vector2 position = ((!Game1.wasMouseVisibleThisFrame) ? who.GetToolLocation() : new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y));
            Vector2 toolLocation = who.GetToolLocation(position);
            who.FacingDirection = who.getGeneralDirectionTowards(new Vector2((int)toolLocation.X, (int)toolLocation.Y));
            who.lastClick = new Vector2((int)position.X, (int)position.Y);

            var toolRect = new Rectangle((int)toolLocation.X - 32, (int)toolLocation.Y - 32, 64, 64);

            AnimalTargetted = StardewValley.Utility.GetBestHarvestableFarmAnimal(Game1.currentLocation.animals.Values, Game1.player.CurrentTool, toolRect);

            OverrideRanching(Game1.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, who, e.Button, who.CurrentTool?.Name);
        }

        private static void OverrideRanching(GameLocation currentLocation, int x, int y, Farmer who, SButton button, string toolName)
        {
            AnimalTargetted = null;
            FarmAnimal animal = null;
            var ranchActionPast = string.Empty;
            var ranchActionPresent = string.Empty;
            var ranchAction = string.Empty;

            if (toolName == null) return;

            switch (toolName)
            {
                case ToolCheck.Tools.MilkPail:
                    ranchActionPast = "milked";
                    ranchActionPresent = "milking";
                    ranchAction = "milk";
                    break;
                case ToolCheck.Tools.Shears:
                    ranchActionPast = "sheared";
                    ranchActionPresent = "shearing";
                    ranchAction = "shear";
                    break;
            }

            animal = StardewValley.Utility.GetBestHarvestableFarmAnimal(toolRect: new Rectangle(x - 32, y - 32, 64, 64), animals: currentLocation.animals.Values, tool: who.CurrentTool);

            if (animal == null)
            {
                ModEntry.TheHelper.Input.Suppress(button);
                Game1.showRedMessage("No animals in range for harvesting");
                return;
            }

            FarmAnimalData animalData = animal.GetAnimalData();

            if (AnimalRanchable(animal, toolName))
            {
                if (who.couldInventoryAcceptThisItem(animal.currentProduce.Value, (!animal.hasEatenAnimalCracker.Value) ? 1 : 2, animal.produceQuality.Value))
                {
                    AnimalTargetted = animal;
                }
                else
                {
                    ModEntry.TheHelper.Input.Suppress(button);
                    Game1.showRedMessage("Inventory is too full");
                }
            }
            else if (animal.isBaby() && animalData.HarvestTool == toolName)
            {
                ModEntry.TheHelper.Input.Suppress(button);
                Game1.showRedMessage(animal.displayName + " is too young to be " + ranchActionPast + ". They will be old enough in " + animalData.DaysToMature + " day(s)");
            }
            else if (animal.currentProduce.Value == null && animalData.HarvestTool == toolName)
            {
                ModEntry.TheHelper.Input.Suppress(button);
                Game1.showRedMessage(animal.displayName + " has been " + ranchActionPast + " recently. They can be " + ranchAction + " every " + animalData.DaysToProduce + " day(s)");
            }
            else if (animal.type.Value == "White Chicken" || animal.type.Value == "Brown Chicken" || animal.type.Value == "Blue Chicken" || animal.type.Value == "Void Chicken")
            {
                ModEntry.TheHelper.Input.Suppress(button);
                Game1.showRedMessage(animal.displayName + " is only a chicken. Please do not " + ranchAction + " them!");
            }
            else
            {
                ModEntry.TheHelper.Input.Suppress(button);
                Game1.showRedMessage(animal.displayName + " is unable to be " + ranchActionPast);
            }
        }

    }
}
