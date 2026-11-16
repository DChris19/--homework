using System;
using System.Collections.Generic;
using System.Linq;

interface IPayment
{
    void Process(double sum);
}

class CardPayment : IPayment
{
    public void Process(double sum)
    {
        Console.WriteLine($"Оплачено карткою: {sum:F2} грн");
    }
}

class CashPayment : IPayment
{
    public void Process(double sum)
    {
        Console.WriteLine($"Оплачено готівкою: {sum:F2} грн");
    }
}

class Order
{
    public int Id { get; }
    public string Title { get; }
    public double WeightKg { get; }
    public double DistanceKm { get; }
    public bool Delivered { get; set; }

    public Order(int id, string title, double weightKg, double distanceKm)
    {
        Id = id;
        Title = title;
        WeightKg = weightKg;
        DistanceKm = distanceKm;
    }
}

abstract class Transport
{
    public string Title { get; }
    public bool Busy { get; set; }
    public Order? Current { get; set; }

    protected Transport(string title)
    {
        Title = title;
    }

    public abstract double GetPrice(Order order);

    public virtual bool CanDeliver(Order order)
    {
        return order.WeightKg > 0 && order.DistanceKm > 0;
    }
}

class Bike : Transport
{
    public Bike() : base("Велосипед") { }

    public override double GetPrice(Order order)
    {
        return 20 + order.DistanceKm * 5;
    }

    public override bool CanDeliver(Order order)
    {
        return base.CanDeliver(order) && order.DistanceKm <= 10;
    }
}

class Car : Transport
{
    public Car() : base("Авто") { }

    public override double GetPrice(Order order)
    {
        return 50 + order.DistanceKm * 8 + order.WeightKg;
    }
}

class Truck : Transport
{
    public Truck() : base("Вантажівка") { }

    public override double GetPrice(Order order)
    {
        return 200 + order.DistanceKm * 15 + order.WeightKg * 0.5;
    }
}

class DeliveryService
{
    private readonly List<Transport> fleet = new();
    private readonly List<Order> active = new();
    private readonly List<Order> archive = new();

    public void AddTransport(Transport t)
    {
        fleet.Add(t);
        Console.WriteLine($"Транспорт додано: {t.Title}");
    }

    public void AddOrder(Order o)
    {
        active.Add(o);
        Console.WriteLine($"Нове замовлення №{o.Id}: {o.Title}");
    }

    public void PrintPrices(Order o)
    {
        Console.WriteLine($"\nВартість доставки замовлення №{o.Id}:");

        foreach (var t in fleet)
        {
            if (t.CanDeliver(o))
                Console.WriteLine($"  {t.Title}: {t.GetPrice(o):F2} грн");
            else
                Console.WriteLine($"  {t.Title}: не підходить для цього замовлення");
        }
    }

    public void Assign(int orderId, Transport t)
    {
        var order = active.FirstOrDefault(o => o.Id == orderId && !o.Delivered);

        if (order == null)
        {
            Console.WriteLine($"Замовлення №{orderId} не знайдено");
            return;
        }

        if (t.Busy)
        {
            Console.WriteLine($"{t.Title} зараз зайнятий");
            return;
        }

        if (!t.CanDeliver(order))
        {
            Console.WriteLine($"{t.Title} не може виконати це замовлення");
            return;
        }

        t.Busy = true;
        t.Current = order;
        Console.WriteLine($"Замовлення №{order.Id} передано: {t.Title}");
    }

    public void Complete(int orderId, IPayment payment)
    {
        var order = active.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            Console.WriteLine($"Замовлення №{orderId} не знайдено");
            return;
        }

        var transport = fleet.FirstOrDefault(t => t.Current == order);

        if (transport == null)
        {
            Console.WriteLine("Для цього замовлення ще не призначено транспорт");
            return;
        }

        double price = transport.GetPrice(order);

        Console.WriteLine($"\nДоставка замовлення №{order.Id}");
        Console.WriteLine($"Транспорт: {transport.Title}");
        payment.Process(price);

        order.Delivered = true;
        transport.Busy = false;
        transport.Current = null;

        active.Remove(order);
        archive.Add(order);

        Console.WriteLine("Замовлення доставлено, транспорт вільний");
    }

    public void PrintActive()
    {
        Console.WriteLine("\nАктивні замовлення:");

        if (active.Count == 0)
        {
            Console.WriteLine("  Активних замовлень немає");
            return;
        }

        foreach (var o in active)
        {
            var t = fleet.FirstOrDefault(x => x.Current == o);
            string who = t != null ? t.Title : "не призначено";

            Console.WriteLine($"  №{o.Id} | {o.Title} | {o.WeightKg} кг | {o.DistanceKm} км | {who}");
        }
    }

    public void PrintArchive()
    {
        Console.WriteLine("\nІсторія доставок:");

        if (archive.Count == 0)
        {
            Console.WriteLine("  Історія порожня");
            return;
        }

        foreach (var o in archive)
        {
            Console.WriteLine($"  №{o.Id} | {o.Title} | {o.WeightKg} кг | {o.DistanceKm} км | доставлено");
        }
    }
}

class Program
{
    static void Main()
    {
        var service = new DeliveryService();

        Console.WriteLine("=== Служба доставки ===\n");

        var bike = new Bike();
        var car = new Car();
        var truck = new Truck();

        service.AddTransport(bike);
        service.AddTransport(car);
        service.AddTransport(truck);

        var docs = new Order(1, "Документи", 2, 5);
        var furniture = new Order(2, "Меблі", 100, 15);
        var food = new Order(3, "Продукти", 10, 8);

        Console.WriteLine();

        service.AddOrder(docs);
        service.AddOrder(furniture);
        service.AddOrder(food);

        service.PrintPrices(docs);
        service.PrintPrices(furniture);
        service.PrintPrices(food);

        Console.WriteLine("\n=== Призначення замовлень ===");

        service.Assign(1, bike);
        service.Assign(2, car);
        service.Assign(3, bike);

        service.PrintActive();

        Console.WriteLine("\n=== Доставка ===");

        service.Complete(1, new CardPayment());
        service.Complete(2, new CashPayment());
        service.Complete(3, new CashPayment());

        Console.WriteLine();

        service.Assign(3, bike);
        service.Complete(3, new CashPayment());

        service.PrintActive();
        service.PrintArchive();
    }
}
