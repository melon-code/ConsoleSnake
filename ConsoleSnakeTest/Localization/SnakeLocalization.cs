using ConsoleMenuAPI;

namespace ConsoleSnake {
    public static class SnakeLocalization {
        public const int ExitStringKey = LocalizationDictionary.ExitStringKey;
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
        public const int ChangeLanguageKey = 40;

        static bool IsRusSelected { get; set; } = true;
        public static string DisplaySnakeLength => Localization.GetString(DisplaySnakeLengthKey);
        public static string Win => Localization.GetString(WinKey);
        public static string GameOver => Localization.GetString(GameOverKey);
        public static string ChangeLanguageString => Localization.GetString(ChangeLanguageKey);

        static LocalizationDictionary GetNextDictionary() {
            if (IsRusSelected)
                return new SnakeEngDictionary();
            return new SnakeRusDictionary();
        }

        static void ChangeLanguage(LocalizationDictionary dictionary) {
            Localization.ChangeLanguage(dictionary);
        }

        public static void SetRusLocalizationDictionary() {
            ChangeLanguage(new SnakeRusDictionary());
        }

        public static void ChangeLanguage() {
            ChangeLanguage(GetNextDictionary());
            IsRusSelected ^= true;
        }
    }
}
