using System;

namespace ConsoleSnake {
    public class GameGrid {
        static FieldItem GetBorderMark(bool borderless) {
            if (borderless)
                return new EmptyItem();
            return new BorderItem();
        }

        readonly FieldItem[,] gameGrid;
        Point tailCoordinates;
        SnakeItem snakeHead;

        public int Height { get; }
        public int Width { get; }
        public FieldItem this[int i, int j] { get { return gameGrid[i, j]; } }

        GameGrid(int height, int width) {
            Height = height;
            Width = width;
            gameGrid = new FieldItem[height, width];
        }

        public GameGrid(int height, int width, bool borderless) : this(height, width) {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    if (!borderless && IsBorder(i, j))
                        gameGrid[i, j] = new BorderItem();
                    else
                        gameGrid[i, j] = new EmptyItem();
        }

        public GameGrid(int height, int width, bool borderless, FieldItem[,] customGameGrid) : this(height, width) {
            if (customGameGrid.Length < height * width)
                throw new ArgumentOutOfRangeException();
            FieldItemType borderMark = borderless ? FieldItemType.Empty : FieldItemType.Border;
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++) {
                    if (customGameGrid[i, j] == null)
                        gameGrid[i, j] = new EmptyItem();
                    if (IsBorder(i, j))
                        gameGrid[i, j] = GetBorderMark(borderless);
                    else
                        gameGrid[i, j] = customGameGrid[i, j];
                }
        }

        bool IsBorder(int indX, int indY) {
            if (indX == 0 || indY == 0 || indX == Height - 1 || indY == Width - 1)
                return true;
            return false;
        }

        void AddNewItem(Point point, FieldItem item) {
            gameGrid[point.X, point.Y] = item;
        }

        SnakeItem GetSnakeItem(Point point) {
            return gameGrid[point.X, point.Y].GetSnakeItem();
        }

        public void SetNewSnakeHead(SnakePoint newHead) {
            SnakeItem headItem = new SnakeItem(newHead.Direction, SnakeType.Head);
            if (snakeHead != null) {
                snakeHead.AddHead(headItem);
                snakeHead = headItem;
            }
            else {
                snakeHead = headItem;
                tailCoordinates = newHead;
            }
            AddNewItem(newHead, snakeHead);
        }

        public void RemoveSnakeTail(Point newTail) {
            if (newTail.X != tailCoordinates.X || newTail.Y != tailCoordinates.Y) {
                GetSnakeItem(newTail).BecomeTail();
                if (GetSnakeItem(tailCoordinates).BodyPart != SnakeType.Head)
                    AddNewItem(tailCoordinates, new EmptyItem());
                tailCoordinates = newTail;
            }
        }

        public void AddFood(Point food, int value) {
            AddNewItem(food, new FoodItem(value));
        }
    }
}
