namespace Module1.Payments;

class CashPayment : IPaymentMethod
{
    public void Pay(double amount)
    {
        Console.WriteLine($"Оплата готівкою {amount:F2} грн");
    }
}
