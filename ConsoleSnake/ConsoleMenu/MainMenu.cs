using System;
using ConsoleMenuAPI;

namespace ConsoleSnake {
    public class MainMenu : ConsoleMenu {
        const int settingsMenuIndex = 1;

        SettingsMenu Menu => GetInsertedMenu(settingsMenuIndex) as SettingsMenu;
        public SettingsResult Settings => 
            new SettingsResult(Menu.Height, Menu.Width, Menu.BigFood, Menu.PortalBorders, Menu.SnakeSpeed, Menu.Borderless, Menu.IsCustomGrid, Menu.CustomGridType);

        public MainMenu() : base(ItemsListHelper.GetMainMenuList()) {
        }

        protected override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Tab) 
                SnakeLocalization.ChangeLanguage();
            ProcessInputByItem(input);
        }

        protected override void Draw() {
            base.Draw();
            Console.WriteLine(string.Format("\n\t{0} -> Tab", SnakeLocalization.ChangeLanguageString));
        }
    }
}
