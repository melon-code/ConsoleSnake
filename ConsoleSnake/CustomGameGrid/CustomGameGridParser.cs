namespace ConsoleSnake {
    public static class CustomGameGridParser {
        public static GameGrid Parse(int height, int width, string grid, bool borderless) {
            if (height * width != grid.Length)
                return null;
            FieldItem[,] items = new FieldItem[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    items[i, j] = ParseItem(grid[i * width + j]);
            return GameGrid.CreateGrid(height, width, borderless, items);
        }

        static FieldItem ParseItem(char item) {
            switch (item) {
                case ' ':
                    return new EmptyItem();
                case 'B':
                    return new BorderItem();
                default:
                    return new EmptyItem();
            }
        }
    }
}
