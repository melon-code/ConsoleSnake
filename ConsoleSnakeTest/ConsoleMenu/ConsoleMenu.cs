using System;
using System.Collections.Generic;

namespace ConsoleSnake {
    public abstract class ConsoleMenu {
        readonly bool hasExitItem = false;

        bool IsExitSelected { get { return hasExitItem && CurrentPosition == ItemCount - 1; } }
        int ItemCount { get { return Items.Count; } }
        protected IList<IMenuItem> Items { get; }
        protected bool IsEnd { get; set; }
        protected int CurrentPosition { get; private set; } = 0;
        protected IMenuItem CurrentItem => Items[CurrentPosition];
        public MenuEndResult EndResult { get; protected set; }

        public ConsoleMenu(IList<IMenuItem> menuItems) {
            Items = menuItems;
        }

        public ConsoleMenu(IList<IMenuItem> menuItems, string exitTitle) : this(menuItems) {
            Items.Add(new MenuItem(exitTitle));
            hasExitItem = true;
        }

        public MenuEndResult ShowDialog() {
            IsEnd = false;
            EndResult = MenuEndResult.Further;
            if (Console.KeyAvailable)
                Console.ReadKey(true);
            do {
                Draw();
            } while (Navigation(Console.ReadKey(true)) && !IsEnd);
            return EndResult;
        }

        public void DrawMenu() {
            for (int i = 0; i < ItemCount; i++) {
                if (i == CurrentPosition)
                    Console.Write("\t---> ");
                if (Items[i].Visible)
                    Items[i].Draw();
            }
        }

        protected virtual void Draw() {
            Console.Clear();
            DrawMenu();
        }

        void ChangeCurrentPosition(bool increase) {
            int iterations = 0;
            do {
                CurrentPosition = increase ? CurrentPosition + 1 : ItemCount + CurrentPosition - 1;
                CurrentPosition %= ItemCount;
                iterations++;
            } while (iterations != ItemCount && !CurrentItem.Visible);
        }

        void IncreaseCurrentPosition() {
            ChangeCurrentPosition(true);
        }

        void DecreaseCurrentPosition() {
            ChangeCurrentPosition(false);
        }

        void CheckInteractivityAndProcessInput(ConsoleKey input) {
            if (CurrentItem.Interactive)
                ProcessInput(input);
        }

        public bool Navigation(ConsoleKeyInfo info) {
            switch (info.Key) {
                case ConsoleKey.DownArrow:
                    IncreaseCurrentPosition();
                    break;
                case ConsoleKey.UpArrow:
                    DecreaseCurrentPosition();
                    break;
                case ConsoleKey.Enter:
                    if (IsExitSelected) {
                        EndResult = MenuEndResult.Exit;
                        return false;
                    }
                    CheckInteractivityAndProcessInput(info.Key);
                    break;
                case ConsoleKey.Escape:
                    EndResult = MenuEndResult.Exit;
                    return false;
                default:
                    CheckInteractivityAndProcessInput(info.Key);
                    break;
            }
            return true;
        }

        public abstract void ProcessInput(ConsoleKey input);
    }
}
