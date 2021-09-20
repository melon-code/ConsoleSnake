using System;

namespace ConsoleSnake {
    public class MainMenu : ConsoleMenu {
        SettingsMenu sMenu;

        public SettingsResult Settings => new SettingsResult(sMenu.Height, sMenu.Width, sMenu.BigFood, sMenu.PortalBorders, sMenu.SnakeSpeed, sMenu.IsCustomGrid, sMenu.CustomGridType);

        public MainMenu() : base(ItemsListHelper.GetMainMenuList()) {
            sMenu = new SettingsMenu(ItemsListHelper.GetSettingsMenuList());
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter) {
                if (CurrentPosition == 0) {
                    EndResult = MenuEndResult.Further;
                    IsEnd = true;
                }
                if (CurrentPosition == 1) {
                    sMenu.ShowDialog();
                }
                if (CurrentPosition == 2) {
                    EndResult = MenuEndResult.Exit;
                    IsEnd = true;
                }
            }
            if (input == ConsoleKey.Tab) {
                Localization.ChangeLanguage();
                UpdateItemsNames(ItemsListHelper.GetMainMenuNames());
                sMenu.UpdateItemsNames(ItemsListHelper.GetSettingsMenuNames());
            }
        }

        protected override void Draw() {
            base.Draw();
            Console.WriteLine(string.Format("\n\t{0} -> Tab", Localization.ChangeLanguageString));
        }
    }
}
