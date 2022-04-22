using System;

namespace ConsoleSnake {
    public abstract class GameGrid {
        public static GameGrid CreateGrid(int height, int width, bool borderless) {
            if (borderless)
                return new BorderlessGameGrid(height, width);
            return new BorderGameGrid(height, width);
        }

        public static GameGrid CreateGrid(int height, int width, bool borderless, FieldItem[,] customGameGrid) {
            if (borderless)
                return new BorderlessGameGrid(height, width, customGameGrid);
            return new BorderGameGrid(height, width, customGameGrid);
        }

        protected readonly FieldItem[,] gameGrid;
        Point tailCoordinates;
        SnakeItem snakeHead;

        protected abstract int MinHeight { get; }
        protected abstract int MinWidth { get; }
        public int Height { get; }
        public int Width { get; }
        public FieldItem this[int i, int j] { get { return gameGrid[i, j]; } }

        protected GameGrid(int height, int width) {
            Height = height < MinHeight ? MinHeight : height;
            Width = width < MinWidth ? MinWidth : width;
            gameGrid = new FieldItem[height, width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    InitializeItem(i, j);
        }

        protected GameGrid(int height, int width, FieldItem[,] customGameGrid) : this(height, width) {
            if (customGameGrid.Length < height * width)
                throw new ArgumentOutOfRangeException();
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++) {
                    if (customGameGrid[i, j] == null)
                        gameGrid[i, j] = new EmptyItem();
                    CopyItem(i, j, customGameGrid);
                }
        }

        protected abstract void InitializeItem(int indX, int indY);

        protected abstract void CopyItem(int indX, int indY, FieldItem[,] source);

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
