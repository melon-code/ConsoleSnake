using System;

namespace ConsoleSnake {
    public class ConsoleSnakeDrawer {
        readonly int initialWindowHeight = Console.WindowHeight;
        readonly int initialWindowWidth = Console.WindowWidth;
        readonly int initialBufferHeight = Console.BufferHeight;
        readonly int initialBufferWidth = Console.BufferWidth;
        Field drawingField;

        public ConsoleSnakeDrawer(Field gameField) {
            drawingField = gameField;
        }

        public void SetConsoleWindow() {
            Console.Clear();
            Console.CursorVisible = false;
            if (drawingField.Borderless) {
                Console.WindowHeight = 1;
                //Console.SetWindowSize(1, 1);
                //var left = Console.WindowLeft;
                //var top = Console.WindowTop;
                //Console.SetBufferSize(gameField.Width, gameField.Height);
                //Console.BufferHeight = gameField.Height + 1;
                //Console.WindowHeight = gameField.Height;
                //Console.BufferWidth = 10;
                //Console.SetWindowSize(gameField.Width, gameField.Height);
                var left = Console.WindowLeft;
                var top = Console.WindowTop;
            }
        }

        public void RestoreConsoleWindow() {
            //Console.SetWindowSize(1, 1);
            //Console.SetBufferSize(initialBufferHeight, initialBufferWidth);
            //Console.SetWindowSize(initialWindowHeight, initialWindowWidth);
        }

        string GetHeadChar(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    return "^";
                case Direction.Down:
                    return "v";
                case Direction.Left:
                    return "<";
                case Direction.Right:
                    return ">";
                default:
                    return ">";
            }
        }

        string GetDirectionSymbol(Direction direction) { //debug
            switch (direction) {
                case Direction.Up:
                    return "U";
                case Direction.Down:
                    return "D";
                case Direction.Left:
                    return "L";
                case Direction.Right:
                    return "R";
                default:
                    return "?";
            }
        }

        string DrawSnakeItem(SnakeItem item) {
            Direction direction = item.Direction;
            switch (item.BodyPart) {
                case SnakeType.Head:
                    return GetHeadChar(direction);
                case SnakeType.Body:
                    return GetDirectionSymbol(direction);
                case SnakeType.Tail:
                    return "*";
                default:
                    return "?";
            }
        }

        void DrawItem(FieldItem item) {
            if (item != null) {
                switch (item.Type) {
                    case FieldItemType.Snake:
                        //Console.Write("#");
                        Console.Write(DrawSnakeItem(item.GetSnakeItem()));
                        break;
                    case FieldItemType.Border:
                        Console.Write("B");
                        break;
                    case FieldItemType.Food:
                        Console.Write("o");
                        break;
                    default:
                        Console.Write(" ");
                        break;
                }
            }
            else
                Console.Write("?");
        }

        public void DrawGameField() {
            for (int i = 0; i < drawingField.Height; i++) {
                for (int j = 0; j < drawingField.Width; j++) {
                    DrawItem(drawingField.Grid[i, j]);
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, 0);
        }
    }
}
