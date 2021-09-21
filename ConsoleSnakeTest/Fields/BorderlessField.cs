namespace ConsoleSnake {
    public class BorderlessField : Field {
        const int borderWidth = 0;

        protected override int BorderWidth => borderWidth;
        protected override int PlayableArea => Height * Width;
        public override bool Borderless => true;


        public BorderlessField(int height, int width, bool allowPortalBorders, bool enableBigFood) : base(height, width, allowPortalBorders, enableBigFood) {
        }

        public BorderlessField(int height, int width, bool allowPortalBorders, int bigFoodInterval) : base(height, width, allowPortalBorders, bigFoodInterval) {
        }

        public BorderlessField(int height, int width, bool allowPortalBorders) : base(height, width, allowPortalBorders) {
        }

        public BorderlessField(int height, int width) : this(height, width, false) {
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
