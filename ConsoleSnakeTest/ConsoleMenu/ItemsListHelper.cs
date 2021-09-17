using System;
using System.Collections.Generic;

namespace ConsoleSnake {
    public static class ItemsListHelper {
        const int defaultHeight = 10;
        const int defaultWidth = 10;
        const int minHeight = 10;
        const int minWidth = 10;
        const int defaultSpeed = 5; //500 ms
        const int minSpeed = 1;
        const int maxSpeed = 10;

        public static IList<IMenuItem> GetSettingsMenuList() {
            IntMenuItem customTypes = new IntMenuItem(Localization.CustomFieldType, 1, 1, 3);
            IList<DependencyItem> dependencies = new List<DependencyItem>() { new DependencyItem(customTypes) };
            return new List<IMenuItem> { new IntMenuItem(Localization.Height, defaultHeight, minHeight, Console.LargestWindowHeight),
                new IntMenuItem(Localization.Width, defaultWidth, minWidth, Console.LargestWindowWidth), new BoolMenuItem(Localization.BigFood, false),
                new BoolMenuItem(Localization.PortalBorders, false), new IntMenuItem(Localization.Speed, defaultSpeed, minSpeed, maxSpeed),
                new DependencyBoolMenuItem(Localization.CustomField, false, dependencies), customTypes };
        }

        public static IList<IMenuItem> GetMainMenuList() {
            return new IMenuItem[] { new MenuItem(Localization.NewGame), new MenuItem(Localization.Settings), new MenuItem(Localization.ExitString) };
        }

        public static IList<IMenuItem> GetEndscreenMenuList() {
            return new List<IMenuItem>() { new MenuItem(Localization.SameRepeat), new MenuItem(Localization.ToMainMenu) };
        }
    }
}
