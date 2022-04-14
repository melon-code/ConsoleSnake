namespace ConsoleSnake {
    public struct CustomGameGrid {
        public GameGrid Grid { get; }
        public int SnakeHeadX { get; }
        public int SnakeHeadY { get; }
        public Direction SnakeHeadDirection { get; }
        public bool PortalBorders { get; }
        public bool Borderless { get; }

        public CustomGameGrid(int height, int width, string grid, bool borderless, int snakeHeadX, int snakeHeadY, Direction snakeHeadDirection, bool portalBorders) {
            Grid = CustomGameGridParser.Parse(height, width, grid, borderless);
            SnakeHeadX = snakeHeadX;
            SnakeHeadY = snakeHeadY;
            SnakeHeadDirection = snakeHeadDirection;
            PortalBorders = portalBorders;
            Borderless = borderless;
        }
    }
}
