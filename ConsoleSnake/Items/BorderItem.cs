namespace ConsoleSnake {
    public class BorderItem : FieldItem {
        public BorderItem() : base(FieldItemType.Border) {
        }

        public override CollisionType GetCollision() {
            return CollisionType.Border;
        }
    }
}
