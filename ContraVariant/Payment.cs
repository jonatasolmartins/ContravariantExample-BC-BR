namespace ContraVariant;

public class Payment
{
    public decimal Amount { get; set; }
}

public class CashPayment : Payment { }
public class DigitalPayment : Payment { }
public class PaypalPayment : DigitalPayment { }
public class CreditCardPayment : DigitalPayment { }

