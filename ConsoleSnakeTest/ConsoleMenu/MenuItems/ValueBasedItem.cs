namespace ConsoleSnake {
    public abstract class ValueBasedItem<T> : MenuItemBase {
        public T Value { get; protected set; }

        public ValueBasedItem(string name, T defaultValue) : base(name) {
            Value = defaultValue;
        }
    }
}
