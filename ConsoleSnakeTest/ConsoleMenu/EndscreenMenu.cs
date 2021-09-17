using System;

namespace ConsoleSnake {
    public class EndscreenMenu : ConsoleMenu {
        public SnakeGameStats GameResults { get; set; }
        public bool Restart { get; private set; } = false;

        public EndscreenMenu() : base(ItemsListHelper.GetEndscreenMenuList(), Localization.ExitString) {
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter) {
                Restart = CurrentPosition == 0 ? true : false;
                IsEnd = true;
            }
        }

        protected override void Draw() {
            Console.Clear();
            Console.WriteLine(string.Format("\t{0}\n", GameResults.Win ? Localization.Win : Localization.GameOver));
            Console.WriteLine(string.Format("\t{0}" + GameResults.SnakeLength + "\n", Localization.DisplaySnakeLength));
            DrawMenu();
        }
    }
}
