namespace ConsoleSnake {
    public class BorderlessField : Field {
        const int borderWidth = 0;

        protected override int BorderWidth => borderWidth;
        protected override int PlayableArea => Height * Width;
        public override bool Borderless => true;

        public BorderlessField(int h, int w, bool allowPortalBorders) : base(h, w, allowPortalBorders) {
        }

        public BorderlessField(int h, int w) : this(h, w, false) {
        }

        public BorderlessField(GameGrid customGameGrid, int initialSnakeHeadX, int initialSnakeHeadY, Direction initialSnakeDirection, bool allowPortalBorders)
            : base(customGameGrid, initialSnakeHeadX, initialSnakeHeadY, initialSnakeDirection, allowPortalBorders) {
        }

        protected override CollisionType IsCollision(Point head) {
            int headX = head.X;
            int headY = head.Y;
            if (headX < 0 || headY < 0 || headX > Height - 1 || headY > Width - 1) // do not need with portal snake
                return CollisionType.Border;
            return IsCollision(headX, headY);
        }

        protected override void ProcessBorderCollision() {
            IsGameOver = true;
        }
    }
}
