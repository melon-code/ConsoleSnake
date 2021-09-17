using System.Collections.Generic;

namespace ConsoleSnake {
    public class DependencyBoolMenuItem : BoolMenuItem {
        readonly IList<DependencyItem> items;

        public DependencyBoolMenuItem(string name, bool defaultValue, IList<DependencyItem> dependencyItems) : base(name, defaultValue) {
            items = dependencyItems;
            SyncDependency(defaultValue);
        }

        void SyncDependency(bool value) {
            foreach (var item in items)
                item.ChangeVisibility(value);
        }

        public override void ChangeValue() {
            base.ChangeValue();
            SyncDependency(Value);
        }
    }
}
