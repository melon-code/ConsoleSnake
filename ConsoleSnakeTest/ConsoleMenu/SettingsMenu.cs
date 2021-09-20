using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleSnake {
    public class SettingsMenu : ConsoleMenu {
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

        public SettingsMenu(IList<IMenuItem> settingsItems) : base(settingsItems, Localization.ExitString) {
        }

        T GetValue<T, Type>(int index) where Type : IMenuValueItem<T> {
            var item = Items[index];
            if (item is Type)
                return ((Type)item).Value;
            throw new ArgumentException();
        }

        int GetInt(int index) {
            return GetValue<int, IntMenuItem>(index);
        }

        bool GetBool(int index) {
            return GetValue<bool, BoolMenuItem>(index);
        }

        public override void ProcessInput(ConsoleKey input) {
            CurrentItem.ProcessInput(input);
        }
    }
}
