using System;

namespace ConsoleSnake {
    public class ConsoleSnakeDrawer {
        static int VerifyValue(int value, int min, int max) {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static int MaxWindowHeight => Console.LargestWindowHeight;
        public static int MaxWindowWidth => Console.LargestWindowWidth;

        public const int minWindowHeight = 1;
        public const int minWindowWidth = 15;
        const string borderSymbol = "B";
        const string emptySymbol = " ";
        const string smallFoodSymbol = "o";
        const string bigFoodSymbol = "O";
        const string unknownSymbol = "?";

        readonly int verifiedHeight;
        readonly int verifiedWidth;
        int initialHeight;
        int initialWidth;
        int initialBufferHeight;
        int initialBufferWidth;
        bool initialCursorVisibility;
        Field drawingField;

        public ConsoleSnakeDrawer(Field gameField) {
            drawingField = gameField;
            verifiedHeight = VerifyValue(drawingField.Height, minWindowHeight, MaxWindowHeight);
            verifiedWidth = VerifyValue(drawingField.Width, minWindowWidth, MaxWindowWidth);
        }

        void SaveInitialConsoleValues() {
            initialHeight = Console.WindowHeight;
            initialWidth = Console.WindowWidth;
            initialBufferHeight = Console.BufferHeight;
            initialBufferWidth = Console.BufferWidth;
            initialCursorVisibility = Console.CursorVisible;
        }

        void SetConsoleValues(int height, int width, int bufferHeight, int bufferWidth, bool cursorVisible) {
            Console.CursorVisible = cursorVisible;
            Console.SetWindowSize(minWindowWidth, minWindowHeight);
            Console.SetBufferSize(bufferWidth, bufferHeight);
            Console.SetWindowSize(width, height);
        }

        public void SetConsoleWindow() {
            Console.Clear();
            SaveInitialConsoleValues();
            SetConsoleValues(verifiedHeight, verifiedWidth, verifiedHeight, verifiedWidth + 1, false);
        }

        public void RestoreConsoleWindow() {
            SetConsoleValues(initialHeight, initialWidth, initialBufferHeight, initialBufferWidth, initialCursorVisibility);
        }

        string GetHeadChar(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    return SnakeSymbols.HeadUp;
                case Direction.Down:
                    return SnakeSymbols.HeadDown;
                case Direction.Left:
                    return SnakeSymbols.HeadLeft;
                case Direction.Right:
                    return SnakeSymbols.HeadRight;
                default:
                    return unknownSymbol;
            }
        }

        string GetTailChar(Direction direction) {
            switch (direction) {
                case Direction.Up:
                case Direction.Left:
                    return SnakeSymbols.TailUL;
                case Direction.Down:
                case Direction.Right:
                    return SnakeSymbols.TailDR;
                default:
                    return unknownSymbol;
            }
        }

        string DrawSnakeItem(SnakeItem item) {
            Direction direction = item.Direction;
            switch (item.BodyPart) {
                case SnakeType.Head:
                    return GetHeadChar(direction);
                case SnakeType.Body:
                    return SnakeSymbols.Body;
                case SnakeType.Tail:
                    return GetTailChar(direction);
                default:
                    return unknownSymbol;
            }
        }

        void DrawItem(FieldItem item) {
            if (item != null) {
                switch (item.Type) {
                    case FieldItemType.Snake:
                        Console.Write(DrawSnakeItem(item.GetSnakeItem()));
                        break;
                    case FieldItemType.Border:
                        Console.Write(borderSymbol);
                        break;
                    case FieldItemType.Food:
                        Console.Write(Field.GetFoodValue(item) == Field.smallFoodValue ? smallFoodSymbol : bigFoodSymbol);
                        break;
                    default:
                        Console.Write(emptySymbol);
                        break;
                }
            }
            else
                Console.Write(unknownSymbol);
        }

        void DrawLine(int index) {
            for (int j = 0; j < verifiedWidth; j++)
                DrawItem(drawingField.Grid[index, j]);
        }

        public void DrawGameField() {
            for (int i = 0; i < verifiedHeight - 1; i++) {
                DrawLine(i);
                Console.WriteLine();
            }
            DrawLine(verifiedHeight - 1);
            Console.SetCursorPosition(0, 0);
        }
    }
}
