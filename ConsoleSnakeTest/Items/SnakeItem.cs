namespace ConsoleSnake {
    public class SnakeItem : FieldItem {
        SnakeItem next;
        SnakeItem prev;

        public Direction Direction { get; }
        public SnakeType BodyPart { get; private set; }

        public SnakeItem(Direction direction, SnakeType bodyPart) : base(FieldItemType.Snake) {
            Direction = direction;
            BodyPart = bodyPart;
        }

        public override CollisionType GetCollision() {
            return CollisionType.Snake;
        }

        public void AddHead(SnakeItem newHead) {
            if (BodyPart == SnakeType.Head) {
                prev = newHead;
                BodyPart = next != null ? SnakeType.Body : SnakeType.Tail;
                newHead.next = this;
            }
        }

        public void BecomeTail() {
            if (BodyPart != SnakeType.Head)
                BodyPart = SnakeType.Tail;
            next = null;
        }
    }
}
