using Module1.Models;

namespace Module1.Transports;

class Car : Transport
{
    public Car() : base("Авто") { }

    public override double CalculateCost(Order order)
    {
        return 50 + 8 * order.Distance + order.Weight;
    }
}
