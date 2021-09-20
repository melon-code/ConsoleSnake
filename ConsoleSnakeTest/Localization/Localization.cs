namespace ConsoleSnake {
    public static class Localization {
        static LocalizationDictionary dictionary = new RusLangDictionary();

        public static string OnTitle => dictionary.GetItem(LocalizationDictionary.OnTitleKey);
        public static string OffTitle => dictionary.GetItem(LocalizationDictionary.OffTitleKey);
        public static string InputNumber => dictionary.GetItem(LocalizationDictionary.InputNumberKey);
        public static string ExitString => dictionary.GetItem(LocalizationDictionary.ExitStringKey);
        public static string Height => dictionary.GetItem(LocalizationDictionary.HeightKey);
        public static string Width => dictionary.GetItem(LocalizationDictionary.WidthKey);
        public static string BigFood => dictionary.GetItem(LocalizationDictionary.BigFoodKey);
        public static string PortalBorders => dictionary.GetItem(LocalizationDictionary.PortalBorderKey);
        public static string Speed => dictionary.GetItem(LocalizationDictionary.SpeedKey);
        public static string CustomField => dictionary.GetItem(LocalizationDictionary.CustomFieldKey);
        public static string CustomFieldType => dictionary.GetItem(LocalizationDictionary.CustomFieldTypeKey);
        public static string NewGame => dictionary.GetItem(LocalizationDictionary.NewGameKey);
        public static string Settings => dictionary.GetItem(LocalizationDictionary.SettingsKey);
        public static string SameRepeat => dictionary.GetItem(LocalizationDictionary.SameRepeatKey);
        public static string ToMainMenu => dictionary.GetItem(LocalizationDictionary.ToMainMenuKey);
        public static string DisplaySnakeLength => dictionary.GetItem(LocalizationDictionary.DisplaySnakeLengthKey);
        public static string Win => dictionary.GetItem(LocalizationDictionary.WinKey);
        public static string GameOver => dictionary.GetItem(LocalizationDictionary.GameOverKey);
        public static string ChangeLanguageString => dictionary.GetItem(LocalizationDictionary.ChangeLanguageKey);

        public static void ChangeLanguage() {
            if (dictionary is RusLangDictionary)
                dictionary = new EngLangDictionary();
            else
                dictionary = new RusLangDictionary();
        }
    }
}
