using System;
using ConsoleMenuAPI;

namespace ConsoleSnake {
    public class EndscreenMenu : StandardConsoleMenu {
        public SnakeGameStats GameResults { get; set; }
        public bool Restart => CurrentPosition == 0;

        public EndscreenMenu() : base(ItemsListHelper.GetEndscreenMenuList(), SnakeLocalization.ExitStringKey) {
        }

        protected override void Draw() {
            ConsoleMenuDrawer.SetCursorToLeftTopCorner();
            Console.WriteLine(string.Format("\t{0}\n", GameResults.Win ? SnakeLocalization.Win : SnakeLocalization.GameOver));
            Console.WriteLine(string.Format("\t{0}" + GameResults.SnakeLength + "\n", SnakeLocalization.DisplaySnakeLength));
            DrawMenu();
        }
    }
}
