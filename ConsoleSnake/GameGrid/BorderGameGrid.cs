namespace ConsoleSnake {
    public class BorderGameGrid : GameGrid {
        const int minHeight = 4;
        const int minWidth = 4;

        protected override int MinHeight => minHeight;
        protected override int MinWidth => minWidth;

        public BorderGameGrid(int height, int width) : base(height, width) {
        }

        public BorderGameGrid(int height, int width, FieldItem[,] customGameGrid) : base(height, width, customGameGrid) {
        }

        bool IsBorder(int indX, int indY) {
            if (indX == 0 || indY == 0 || indX == Height - 1 || indY == Width - 1)
                return true;
            return false;
        }

        protected override void InitializeItem(int i, int j) {
            if (IsBorder(i, j))
                gameGrid[i, j] = new BorderItem();
            else
                gameGrid[i, j] = new EmptyItem();
        }

        protected override void CopyItem(int i, int j, FieldItem[,] source) {
            if (IsBorder(i, j))
                gameGrid[i, j] = new BorderItem();
            else
                gameGrid[i, j] = source[i, j];
        }
    }
}
