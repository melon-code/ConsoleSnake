using System;

namespace ConsoleSnake {
    public class BoolMenuItem : ValueBasedItem<bool>, IMenuValueItem<bool> {
        public BoolMenuItem(string name, bool defaultValue) : base(name, defaultValue) {
        }

        public override void Draw() {
            Console.WriteLine("\t" + Name + string.Format(" < {0} >", Value ? Localization.OnTitle : Localization.OffTitle) + "\n");
        }

        public virtual void ChangeValue() {
            Value = !Value;
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter || input == ConsoleKey.LeftArrow || input == ConsoleKey.RightArrow)
                ChangeValue();
        }
    }
}
