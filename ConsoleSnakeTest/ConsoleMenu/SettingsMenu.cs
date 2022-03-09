using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleMenuAPI;

namespace ConsoleSnake {
    public class SettingsMenu : StandardConsoleMenu {
        public static CustomGameGrid GetCustomGrid(int type) {
            switch (type) {
                case 1:
                    return CustomGameGridTypes.TypeA;
                case 2:
                    return CustomGameGridTypes.TypeB;
                case 3:
                    return CustomGameGridTypes.TypeC;
                default:
                    return CustomGameGridTypes.TypeA;
            }
        }

        const int heightIndex = 0;
        const int widthIndex = 1;
        const int bigFoodIndex = 2;
        const int portalBorderIndex = 3;
        const int snakeSpeedIndex = 4;
        const int isCustomGridIndex = 5;
        const int customGridTypeIndex = 6;

        public bool LanguageChanged { get; private set; } = false;
        public int Height => GetInt(heightIndex);
        public int Width => GetInt(widthIndex);
        public bool BigFood => GetBool(bigFoodIndex);
        public bool PortalBorders => GetBool(portalBorderIndex);
        public int SnakeSpeed => GetInt(snakeSpeedIndex);
        public bool IsCustomGrid => GetBool(isCustomGridIndex);
        public int? CustomGridType {
            get {
                if (IsCustomGrid)
                    return GetInt(customGridTypeIndex);
                return null;
            }
        }

        public SettingsMenu(IList<IMenuItem> settingsItems) : base(settingsItems, SnakeLocalization.ExitStringKey) {
        }
    }
}
