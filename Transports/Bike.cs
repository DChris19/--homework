using Module1.Models;

namespace Module1.Transports;

class Bike : Transport
{
    public Bike() : base("Велосипед") { }

    public override double CalculateCost(Order order)
    {
        return 20 + 5 * order.Distance;
    }

    public override bool CanTake(Order order)
    {
        return base.CanTake(order) && order.Distance <= 10;
    }
}
