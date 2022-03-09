using ConsoleMenuAPI;

namespace ConsoleSnake {
    public class SnakeEngDictionary : EngLangDictionary {
        const string height = "Height";
        const string width = "Width";
        const string bigFood = "Big food";
        const string portalBorder = "Portal borders";
        const string speed = "Speed";
        const string customField = "Custom field";
        const string customFieldType = "Custom field type";
        const string newGame = "New Game";
        const string settings = "Settings";
        const string sameRepeat = "Start again";
        const string toMainMenu = "Go to Main Menu";
        const string displaySnakeLength = "Snake length is ";
        const string win = "Congratulations! You WON!";
        const string gameOver = "GAME OVER Try fortune next time!";
        const string changeLanguage = "Change language";

        public SnakeEngDictionary() : base() {
            dictionary.Add(SnakeLocalization.HeightKey, height);
            dictionary.Add(SnakeLocalization.WidthKey, width);
            dictionary.Add(SnakeLocalization.BigFoodKey, bigFood);
            dictionary.Add(SnakeLocalization.PortalBorderKey, portalBorder);
            dictionary.Add(SnakeLocalization.SpeedKey, speed);
            dictionary.Add(SnakeLocalization.CustomFieldKey, customField);
            dictionary.Add(SnakeLocalization.CustomFieldTypeKey, customFieldType);
            dictionary.Add(SnakeLocalization.NewGameKey, newGame);
            dictionary.Add(SnakeLocalization.SettingsKey, settings);
            dictionary.Add(SnakeLocalization.SameRepeatKey, sameRepeat);
            dictionary.Add(SnakeLocalization.ToMainMenuKey, toMainMenu);
            dictionary.Add(SnakeLocalization.DisplaySnakeLengthKey, displaySnakeLength);
            dictionary.Add(SnakeLocalization.WinKey, win);
            dictionary.Add(SnakeLocalization.GameOverKey, gameOver);
            dictionary.Add(SnakeLocalization.ChangeLanguageKey, changeLanguage);
        }
    }
}
