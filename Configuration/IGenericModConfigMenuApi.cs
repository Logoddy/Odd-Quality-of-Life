using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Odd_Quality_of_Life.Configuration
{
    public interface IGenericModConfigMenuApi
    {
        /// Registration
        /// Register Mod
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        ///  Unregister Mod
        void Unregister(IManifest mod);

        /// Formatting
        /// Add a title for the section on the form
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        /// Add a paragraph for the section on the form
        void AddParagraph(IManifest mod, Func<string> text);

        /// Add an image for the section on the form
        void AddImage(IManifest mod, Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = Game1.pixelZoom);

        /// Input Options
        /// Boolean
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        /// Number (integer)
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        /// Number (Float)
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        /// Text
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        /// Keybind
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        /// Keybind List
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        /// Complex and chaotic options
        void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);

        /// Pages/Navigation
        /// Add a new page
        void AddPage(IManifest mod, string pageId, Func<string> pageTitle = null);

        /// Add a link to a specific page
        void AddPageLink(IManifest mod, string pageId, Func<string> text, Func<string> tooltip = null);

        /// Set the following options to only be avilable form the title screen
        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);

        /// Register a method to check when any config from this mod is changed through the UI
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);

        /// Open a specific mods menu
        void OpenModMenu(IManifest mod);

        /// Get the current displayed mod menu
        bool TryGetCurrentMenu(out IManifest mod, out string page);
    }
}
