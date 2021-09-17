namespace ConsoleSnake {
    public class PortalSnakePoint : SnakePoint {
        const int borderlessMinValue = 0;

        readonly int minValue;
        readonly int limitX;
        readonly int limitY;

        public PortalSnakePoint(int x, int y, int limitX, int limitY, int minimum, Direction direction) : base(x, y, direction) {
            this.limitX = limitX;
            this.limitY = limitY;
            minValue = minimum;
        }

        public PortalSnakePoint(int x, int y, int limitX, int limitY, int minimum) : this(x, y, limitX, limitY, minimum, defaultDirection) {
        }

        public override SnakePoint GetPointAfterMove() {
            SnakePoint point = base.GetPointAfterMove();
            if (point.X < minValue)
                point.X += limitX - minValue;
            if (point.Y < minValue)
                point.Y += limitY - minValue;
            point.X %= limitX;
            point.Y %= limitY;
            if (minValue != borderlessMinValue) {
                if (point.X == 0)
                    point.X += minValue;
                if (point.Y == 0)
                    point.Y += minValue;
            }
            return new PortalSnakePoint(point.X, point.Y, limitX, limitY, minValue, point.Direction);
        }
    }
}
