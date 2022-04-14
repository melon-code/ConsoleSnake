namespace ConsoleSnake {
    public abstract class FieldItem {
        public FieldItemType Type { get; }

        protected FieldItem(FieldItemType type) {
            Type = type;
        }

        public SnakeItem GetSnakeItem() {
            return this as SnakeItem;
        }

        public abstract CollisionType GetCollision();
    }
}
