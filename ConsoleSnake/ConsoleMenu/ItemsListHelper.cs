using System;
using System.Collections.Generic;
using ConsoleMenuAPI;

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
            IntMenuItem customTypes = new IntMenuItem(SnakeLocalization.CustomFieldTypeKey, 1, 1, 3);
            IList<DependencyItem> dependencies = new List<DependencyItem>() { new DependencyItem(customTypes) };
            return new List<IMenuItem> {
                new IntMenuItem(SnakeLocalization.HeightKey, defaultHeight, minHeight, Console.LargestWindowHeight),
                new IntMenuItem(SnakeLocalization.WidthKey, defaultWidth, minWidth, Console.LargestWindowWidth), new BoolMenuItem(SnakeLocalization.BigFoodKey, false),
                new BoolMenuItem(SnakeLocalization.PortalBorderKey, false), new IntMenuItem(SnakeLocalization.SpeedKey, defaultSpeed, minSpeed, maxSpeed),
                new DependencyBoolMenuItem(SnakeLocalization.CustomFieldKey, false, dependencies), customTypes
            };
        }

        static ConsoleMenu CreateSettingsMenu() {
            return new SettingsMenu(GetSettingsMenuList());
        }

        public static IList<IMenuItem> GetMainMenuList() {
            return new IMenuItem[] {
                new ContinueItem(SnakeLocalization.NewGameKey), new InsertedMenuItem(SnakeLocalization.SettingsKey, CreateSettingsMenu()), new ExitItem(SnakeLocalization.ExitStringKey)
            };
        }

        public static IList<IMenuItem> GetEndscreenMenuList() {
            return new List<IMenuItem>() { new ContinueItem(SnakeLocalization.SameRepeatKey), new ContinueItem(SnakeLocalization.ToMainMenuKey) };
        }
    }
}
