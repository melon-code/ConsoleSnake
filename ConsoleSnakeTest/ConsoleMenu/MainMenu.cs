using System;

namespace ConsoleSnake {
    public class MainMenu : ConsoleMenu {
        SettingsMenu sMenu;

        public SettingsResult Settings => new SettingsResult(sMenu.Height, sMenu.Width, sMenu.BigFood, sMenu.PortalBorders, sMenu.SnakeSpeed, sMenu.IsCustomGrid, sMenu.CustomGridType);

        public MainMenu() : base(ItemsListHelper.GetMainMenuList()) {
            sMenu = new SettingsMenu(ItemsListHelper.GetSettingsMenuList());
        }

        void UpdateNames() {
            var updatedItems = ItemsListHelper.GetMainMenuList();
            for (int i = 0; i < Items.Count; i++)
                Items[i].ChangeName(updatedItems[i].Name);
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
            if (input == ConsoleKey.L) {
                Localization.ChangeLanguage();
                UpdateNames();
                sMenu.UpdateNames();
            }
        }

        protected override void Draw() {
            base.Draw();
            Console.WriteLine("\n\tChange languege -> L");
        }
    }
}
