namespace ConsoleSnake {
    public class EmptyItem : FieldItem {
        public EmptyItem() : base(FieldItemType.Empty) {
        }

        public override CollisionType GetCollision() {
            return CollisionType.No;
        }
    }
}
