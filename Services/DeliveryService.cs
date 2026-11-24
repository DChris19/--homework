using Module1.Models;
using Module1.Payments;
using Module1.Transports;

namespace Module1.Services;

class DeliveryService
{
    private List<Transport> transports = new List<Transport>();
    private List<Order> activeOrders = new List<Order>();
    private List<Order> history = new List<Order>();

    public void AddTransport(Transport transport)
    {
        transports.Add(transport);
        Console.WriteLine($"Додано транспорт {transport.Name}");
    }

    public void AddOrder(Order order)
    {
        activeOrders.Add(order);
        Console.WriteLine($"Замовлення №{order.Number}: {order.Description}");
    }

    public void ShowPrices(Order order)
    {
        Console.WriteLine($"\nСума доставки №{order.Number}:");

        foreach (Transport transport in transports)
        {
            if (transport.CanTake(order))
            {
                Console.WriteLine(
                    $"{transport.Name}: {transport.CalculateCost(order):F2} грн");
            }
            else
            {
                Console.WriteLine(
                    $"{transport.Name}: Не підходить до цього замовлення");
            }
        }
    }

    public void AssignOrder(int orderNumber, Transport transport)
    {
        Order? order = activeOrders.FirstOrDefault(
            o => o.Number == orderNumber && !o.IsDelivered);

        if (order == null)
        {
            Console.WriteLine($"Заказ №{orderNumber} не знайдено");
            return;
        }

        if (transport.IsBusy)
        {
            Console.WriteLine($"{transport.Name} вже зянято");
            return;
        }

        if (!transport.CanTake(order))
        {
            Console.WriteLine($"{transport.Name} не можливо");
            return;
        }

        transport.IsBusy = true;
        transport.CurrentOrder = order;

        Console.WriteLine(
            $"Заказ №{order.Number} траспорт - {transport.Name}");
    }

    public void DeliverOrder(int orderNumber, IPaymentMethod payment)
    {
        Order? order = activeOrders.FirstOrDefault(
            o => o.Number == orderNumber);

        if (order == null)
        {
            Console.WriteLine($"Заказ №{orderNumber} не знайдено");
            return;
        }

        Transport? transport = transports.FirstOrDefault(
            t => t.CurrentOrder == order);

        if (transport == null)
        {
            Console.WriteLine("замовлення не зе не доданно");
            return;
        }

        double cost = transport.CalculateCost(order);

        Console.WriteLine($"\nДоставка замовлення№{order.Number}");
        Console.WriteLine($"Транспорт,{transport.Name}");

        payment.Pay(cost);

        order.IsDelivered = true;

        transport.IsBusy = false;
        transport.CurrentOrder = null;

        activeOrders.Remove(order);
        history.Add(order);

        Console.WriteLine("Замовлення доставлено");
        Console.WriteLine("Транспорт вільний");
    }

    public void ShowActiveOrders()
    {
        Console.WriteLine("\n Активні замовлення");

        if (activeOrders.Count == 0)
        {
            Console.WriteLine("Активованих  замовлень немає");
            return;
        }

        foreach (Order order in activeOrders)
        {
            Transport? transport = transports.FirstOrDefault(
                t => t.CurrentOrder == order);

            Console.WriteLine(
                $"№{order.Number} | {order.Description} | " +
                $"{order.Weight} кг | {order.Distance} км | " +
                $"Транспорт: {transport?.Name ?? ""}");
        }
    }

    public void ShowHistory()
    {
        Console.WriteLine("\nДоставки");

        if (history.Count == 0)
        {
            Console.WriteLine("Доставки пусті");
            return;
        }

        foreach (Order order in history)
        {
            Console.WriteLine(
                $"№{order.Number} | {order.Description} | " +
                $"{order.Weight} кг | {order.Distance} км | доставленно");
        }
    }
}
