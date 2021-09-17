using System.Collections.Generic;

namespace ConsoleSnake {
    public class LocalizationDictionary {
        const string errorString = "NULL";
        public const int OnTitleKey = 1;
        public const int OffTitleKey = 2;
        public const int InputNumberKey = 3;
        public const int ExitStringKey = 4;
        public const int HeightKey = 5;
        public const int WidthKey = 6;
        public const int BigFoodKey = 7;
        public const int PortalBorderKey = 8;
        public const int SpeedKey = 9;
        public const int CustomFieldKey = 10;
        public const int CustomFieldTypeKey = 11;
        public const int NewGameKey = 20;
        public const int SettingsKey = 21;
        public const int SameRepeatKey = 22;
        public const int ToMainMenuKey = 23;
        public const int DisplaySnakeLengthKey = 25;
        public const int WinKey = 30;
        public const int GameOverKey = 31;

        readonly protected Dictionary<int, string> dictionary;

        public string this[int key] => GetItem(key); 

        public LocalizationDictionary() {
            dictionary = new Dictionary<int, string>();
        }

        public string GetItem(int key) {
            if (dictionary.TryGetValue(key, out string value))
                return value;
            return errorString;
        }
    }
}
