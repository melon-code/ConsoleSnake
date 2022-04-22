namespace ConsoleSnake {
    public struct SettingsResult {
        public int Height { get; }
        public int Width { get; }
        public bool BigFood { get; }
        public bool PortalBorders { get; }
        public int SnakeSpeed { get; }
        public bool Borderless { get; }
        public bool IsCustomGrid { get; }
        public int? CustomGridType { get; }

        public SettingsResult(int height, int width, bool bigFood, bool portalBorders, int snakeSpeed, bool borderless, bool isCustomGrid, int? customGridType) {
            Height = height;
            Width = width;
            BigFood = bigFood;
            PortalBorders = portalBorders;
            SnakeSpeed = snakeSpeed;
            Borderless = borderless;
            IsCustomGrid = isCustomGrid;
            CustomGridType = customGridType;
        }
    }
}
