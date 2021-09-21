using System.Collections.Generic;

namespace ConsoleSnake {
    public class Snake {
        static bool IsOpposite(Direction direction1, Direction direction2) {
            switch (direction1) {
                case Direction.Up:
                    if (direction2 == Direction.Down)
                        return true;
                    break;
                case Direction.Down:
                    if (direction2 == Direction.Up)
                        return true;
                    break;
                case Direction.Left:
                    if (direction2 == Direction.Right)
                        return true;
                    break;
                case Direction.Right:
                    if (direction2 == Direction.Left)
                        return true;
                    break;
            }
            return false;
        }

        int currentExtends = 0;
        protected LinkedList<SnakePoint> snake;

        bool IsExtending => currentExtends > 0;
        public int Length { get { return snake.Count; } }
        public SnakePoint Head { get { return HeadNode.Value; } }
        public SnakePoint Tail { get { return TailNode.Value; } }
        public LinkedListNode<SnakePoint> HeadNode { get { return snake.First; } }
        public LinkedListNode<SnakePoint> TailNode { get { return snake.Last; } }

        protected Snake() {
            snake = new LinkedList<SnakePoint>();
        }

        public Snake(int headX, int headY) : this() {
            snake.AddFirst(new SnakePoint(headX, headY));
        }

        public Snake(int headX, int headY, Direction direction) : this() {
            snake.AddFirst(new SnakePoint(headX, headY, direction));
        }

        public void SetHeadDirection(Direction direction) {
            var currentDirection = Head.Direction;
            if ((currentDirection != direction) && !IsOpposite(currentDirection, direction))
                Head.Direction = direction;
        }

        public SnakePoint GetNextHead() {
            return Head.GetPointAfterMove();
        }

        public void Move() {
            snake.AddFirst(Head.GetPointAfterMove());
            if (IsExtending)
                currentExtends--;
            else
                snake.RemoveLast();
        }

        public void Move(int ateFoodValue) {
            currentExtends += ateFoodValue;
            Move();
        }
    }
}
