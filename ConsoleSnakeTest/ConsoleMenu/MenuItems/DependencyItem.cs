namespace ConsoleSnake {
    public class DependencyItem {
        readonly IMenuItem item;
        readonly bool inverted;

        public DependencyItem(IMenuItem menuItem, bool invertedLogic) {
            item = menuItem;
            inverted = invertedLogic;
        }

        public DependencyItem(IMenuItem menuItem) : this(menuItem, false) {
        }

        public void ChangeVisibility(bool value) {
            item.Visible = value ^ inverted;
        }
    }
}
