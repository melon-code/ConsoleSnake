namespace ConsoleSnake {
    public class FoodItem : FieldItem {
        public int Value { get; }

        public FoodItem(int value) : base(FieldItemType.Food) {
            Value = value;
        }

        public override CollisionType GetCollision() {
            return CollisionType.Food;
        }
    }
}
