using Module1.Models;

namespace Module1.Transports;

class Truck : Transport
{
    public Truck() : base("Вантажівка") { }

    public override double CalculateCost(Order order)
    {
        return 200 + 15 * order.Distance + 0.5 * order.Weight;
    }
}
