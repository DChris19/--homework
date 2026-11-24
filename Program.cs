using Module1.Models;
using Module1.Payments;
using Module1.Services;
using Module1.Transports;

class Program
{
    static void Main()
    {
        DeliveryService service = new DeliveryService();

        Console.WriteLine(" служба доставки\n");

        Bike bike = new Bike();
        Car car = new Car();
        Truck truck = new Truck();

        service.AddTransport(bike);
        service.AddTransport(car);
        service.AddTransport(truck);

        Order order1 = new Order(1, "Документи", 2, 5);
        Order order2 = new Order(2, "Меблі", 100, 15);
        Order order3 = new Order(3, "Продукти", 10, 8);

        Console.WriteLine();

        service.AddOrder(order1);
        service.AddOrder(order2);
        service.AddOrder(order3);

        service.ShowPrices(order1);
        service.ShowPrices(order2);
        service.ShowPrices(order3);

        Console.WriteLine("\nназначення замовлень");

        service.AssignOrder(1, bike);
        service.AssignOrder(2, car);

        service.AssignOrder(3, bike);

        service.ShowActiveOrders();

        Console.WriteLine("\n Доставка");

        service.DeliverOrder(1, new CardPayment());

        service.DeliverOrder(2, new CashPayment());

        service.DeliverOrder(3, new CashPayment());

        Console.WriteLine("\n");

        service.AssignOrder(3, bike);
        service.DeliverOrder(3, new CashPayment());

        service.ShowActiveOrders();
        service.ShowHistory();
    }
}
