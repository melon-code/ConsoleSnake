namespace ConsoleSnake {
    public interface IMenuValueItem<T> : IMenuItem {
        T Value { get; }
    }
}
