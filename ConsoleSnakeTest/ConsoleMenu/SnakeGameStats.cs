namespace ConsoleSnake {
    public struct SnakeGameStats {
        public bool Win { get; }
        public int SnakeLength { get; }

        public SnakeGameStats(bool win, int snakeLenght) {
            Win = win;
            SnakeLength = snakeLenght;
        }
    }
}
