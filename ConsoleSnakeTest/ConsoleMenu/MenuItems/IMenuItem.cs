using System;

namespace ConsoleSnake {
    public interface IMenuItem {
        bool Visible { get; set; }
        bool Interactive { get; set; }
        string Name { get; }

        void Draw();
        void ProcessInput(ConsoleKey input);
        void ChangeName(string newName);
    }
}
