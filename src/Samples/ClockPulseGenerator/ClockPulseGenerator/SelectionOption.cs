namespace QuickApp.Support
{
    public record SelectionOption<T>(T Value)
    {
        public T Value { get; } = Value;
        internal SelectionBase<T>? Parent { get; init; }

        public void Select()
        {
            Parent!.TrySelect(this);
        }

        public bool Selected { get; internal set; }
    }
}
