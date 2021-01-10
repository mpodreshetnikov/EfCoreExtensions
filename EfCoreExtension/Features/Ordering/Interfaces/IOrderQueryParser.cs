namespace EfCoreExtensions.Ordering
{
    public interface IOrderQueryParser
    {
        OrderQuery ParseOrderQuery(string orderQuery);
    }
}
