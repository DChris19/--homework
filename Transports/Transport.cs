using Module1.Models;

namespace Module1.Transports;

abstract class Transport
{
    public string Name { get; set; }
    public bool IsBusy { get; set; }
    public Order? CurrentOrder { get; set; }

    public Transport(string name)
    {
        Name = name;
        IsBusy = false;
        CurrentOrder = null;
    }

    public abstract double CalculateCost(Order order);

    public virtual bool CanTake(Order order)
    {
        return order.Weight > 0 && order.Distance > 0;
    }
}
