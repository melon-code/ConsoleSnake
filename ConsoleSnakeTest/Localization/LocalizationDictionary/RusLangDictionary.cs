namespace ConsoleSnake {
    public class RusLangDictionary : LocalizationDictionary {
        const string onTitle = "Да";
        const string offTitle = "Нет";
        const string inputNumber = "Введите числовое значение: ";
        const string exitString = "Выход";
        const string height = "Высота";
        const string width = "Ширина";
        const string bigFood = "Большая еда";
        const string portalBorder = "Портальные границы";
        const string speed = "Скорость";
        const string customField = "Пользовательское поле";
        const string customFieldType = "Тип пользовательского поля";
        const string newGame = "Новая игра";
        const string settings = "Настройки";
        const string sameRepeat = "Повторить снова";
        const string toMainMenu = "В главное меню";
        const string displaySnakeLength = "Длина змейки: ";
        const string win = "Победа!";
        const string gameOver = "Вы проиграли, попробуйте снова!";

        public RusLangDictionary() : base() {
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
