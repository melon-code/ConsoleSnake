using ConsoleMenuAPI;

namespace ConsoleSnake {
    public class SnakeRusDictionary : RusLangDictionary {
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
        const string changeLanguage = "Изменить язык";

        public SnakeRusDictionary() : base() {
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
