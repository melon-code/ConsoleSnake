namespace ConsoleSnake {
    public static class CustomGameGridTypeA {
        public const int Height = 10;
        public const int Width = 12;
        public const int HeadX = Height / 2;
        public const int HeadY = Width / 2;
        public const Direction HeadDirection = Direction.Right;
        public const bool PortalBorders = false;
        public const bool Borderless = false;
        public const string Grid = "BBBBBBBBBBBB" +
                                   "B          B" +
                                   "B          B" +
                                   "B   BBBB   B" +
                                   "B          B" +
                                   "B          B" +
                                   "B   BBBB   B" +
                                   "B          B" +
                                   "B          B" +
                                   "BBBBBBBBBBBB";
    }
}
