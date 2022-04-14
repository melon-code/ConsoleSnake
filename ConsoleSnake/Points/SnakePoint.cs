namespace ConsoleSnake {
    public class SnakePoint : Point {
        protected const Direction defaultDirection = Direction.Right;

        public Direction Direction { get; set; }

        public SnakePoint(int x, int y) : base(x, y) {
            Direction = defaultDirection;
        }

        public SnakePoint(int x, int y, Direction direction) : base(x, y) {
            Direction = direction;
        }

        public virtual SnakePoint GetPointAfterMove() {
            SnakePoint point = new SnakePoint(X, Y, Direction);
            switch (Direction) {
                case Direction.Up:
                    point.X -= 1;
                    break;
                case Direction.Down:
                    point.X += 1;
                    break;
                case Direction.Left:
                    point.Y -= 1;
                    break;
                case Direction.Right:
                    point.Y += 1;
                    break;
            }
            return point;
        }
    }
}
