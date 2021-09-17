using System;

namespace ConsoleSnake {
    public class IntMenuItem : ValueBasedItem<int>, IMenuValueItem<int> {
        static bool ValidateStringInput(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            bool valid = true;
            foreach (var item in str)
                if (!char.IsNumber(item))
                    valid = false;
            return valid;
        }

        readonly int minValue;
        readonly int maxValue;

        public IntMenuItem(string name, int defaultValue, int minLimit, int maxLimit) : base(name, defaultValue) {
            minValue = minLimit;
            maxValue = maxLimit;
        }

        public IntMenuItem(string name, int defaultValue) : this(name, defaultValue, int.MinValue, int.MaxValue) {
        }


        public override void Draw() {
            Console.WriteLine("\t" + Name + string.Format(" < {0} >", Value) + "\n");
        }

        public void IncrementValue() {
            if (Value < maxValue)
                Value++;
        }

        public void DecrementValue() {
            if (Value > minValue)
                Value--;
        }

        bool ValidateInteger(int number) {
            if (number >= minValue && number <= maxValue) {
                Value = number;
                return true;
            }
            return false;
        }

        public void InputValue() {
            Console.Clear();
            string input;
            bool isInputValid = false;
            do {
                Console.Write(Localization.InputNumber);
                input = Console.ReadLine();
                if (ValidateStringInput(input) && ValidateInteger(Convert.ToInt32(input)))
                    isInputValid = true;
            } while (!isInputValid);
        }

        public override void ProcessInput(ConsoleKey input) {
            switch (input) {
                case ConsoleKey.Enter:
                    InputValue();
                    Draw();
                    break;
                case ConsoleKey.LeftArrow:
                    DecrementValue();
                    break;
                case ConsoleKey.RightArrow:
                    IncrementValue();
                    break;
            }
        }
    }
}
