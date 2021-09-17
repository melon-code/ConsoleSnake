using System;

namespace ConsoleSnake {
    public class MenuItem : MenuItemBase {
        public MenuItem(string name) : base(name) {
        }

        public override void Draw() {
            Console.Write("\t" + Name + "\n\n");
        }

        public override void ProcessInput(ConsoleKey input) {
            //mb add inside menus
        }
    }
}
