namespace ConsoleSnake {
    public struct SettingsResult {
        public int Height { get; }
        public int Width { get; }
        public bool BigFood { get; }
        public bool PortalBorders { get; }
        public int SnakeSpeed { get; }
        public bool IsCustomGrid { get; }
        public int? CustomGridType { get; }

        public SettingsResult(int height, int width, bool bigFood, bool portalBorders, int snakeSpeed, bool isCustomGrid, int? customGridType) {
            Height = height;
            Width = width;
            BigFood = bigFood;
            PortalBorders = portalBorders;
            SnakeSpeed = snakeSpeed;
            IsCustomGrid = isCustomGrid;
            CustomGridType = customGridType;
        }
    }
}
