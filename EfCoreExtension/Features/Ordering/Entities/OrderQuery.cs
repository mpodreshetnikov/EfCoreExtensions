namespace EfCoreExtensions.Ordering
{
    public sealed class OrderQuery
    {
        public string PropertyName { get; set; }

        public bool IsAscending { get; set; } = true;
    }
}
