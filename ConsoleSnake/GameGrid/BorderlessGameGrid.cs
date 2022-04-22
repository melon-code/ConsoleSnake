namespace ConsoleSnake {
    public class BorderlessGameGrid : GameGrid {
        const int minHeight = 2;
        const int minWidth = 2;

        protected override int MinHeight => minHeight;
        protected override int MinWidth => minWidth;

        public BorderlessGameGrid(int height, int width) : base(height, width) {
        }

        public BorderlessGameGrid(int height, int width, FieldItem[,] customGameGrid) : base(height, width, customGameGrid) {
        }

        protected override void InitializeItem(int i, int j) {
            gameGrid[i, j] = new EmptyItem();
        }

        protected override void CopyItem(int i, int j, FieldItem[,] source) {
            gameGrid[i, j] = source[i, j];
        }
    }
}
