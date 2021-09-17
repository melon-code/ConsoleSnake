namespace ConsoleSnake {
    public class EngLangDictionary : LocalizationDictionary {
        const string onTitle = "On";
        const string offTitle = "Off";
        const string inputNumber = "Input integer value: ";
        const string exitString = "Exit";
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

        public EngLangDictionary() : base() {
            dictionary.Add(OnTitleKey, onTitle);
            dictionary.Add(OffTitleKey, offTitle);
            dictionary.Add(InputNumberKey, inputNumber);
            dictionary.Add(ExitStringKey, exitString);
            dictionary.Add(HeightKey, height);
            dictionary.Add(WidthKey, width);
            dictionary.Add(BigFoodKey, bigFood);
            dictionary.Add(PortalBorderKey, portalBorder);
            dictionary.Add(SpeedKey, speed);
            dictionary.Add(CustomFieldKey, width);
            dictionary.Add(CustomFieldTypeKey, customFieldType);
            dictionary.Add(NewGameKey, newGame);
            dictionary.Add(SettingsKey, settings);
            dictionary.Add(SameRepeatKey, sameRepeat);
            dictionary.Add(ToMainMenuKey, toMainMenu);
            dictionary.Add(DisplaySnakeLengthKey, displaySnakeLength);
            dictionary.Add(WinKey, win);
            dictionary.Add(GameOverKey, gameOver);
        }
    }
}
