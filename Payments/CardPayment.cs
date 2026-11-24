namespace Module1.Payments;

class CardPayment : IPaymentMethod
{
    public void Pay(double amount)
    {
        Console.WriteLine($"Оплата картою {amount:F2} грн");
    }
}
