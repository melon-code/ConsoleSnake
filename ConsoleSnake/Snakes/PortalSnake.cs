namespace ConsoleSnake {
    public class PortalSnake : Snake {
        public PortalSnake(int headX, int headY, int limitX, int limitY, int minimum) : base() {
            snake.AddFirst(new PortalSnakePoint(headX, headY, limitX, limitY, minimum));
        }

        public PortalSnake(int headX, int headY, int limitX, int limitY, int minimum, Direction direction) : base() {
            snake.AddFirst(new PortalSnakePoint(headX, headY, limitX, limitY, minimum, direction));
        }
    }
}
