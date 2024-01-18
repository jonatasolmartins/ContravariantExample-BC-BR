
using ContraVariant;

var digitalPayments = new DigitalPayment[]
{
    new PaypalPayment(){Amount = 500m},
    new PaypalPayment(){Amount = 0m},
    new CreditCardPayment(){Amount = 1000m},
    new CreditCardPayment(){Amount = 0m}
};

var payments = new List<Payment>
{
    new Payment(){Amount = 500m},
    new CashPayment(){Amount = 1000m},
    new CashPayment(){Amount = 0m},
    new CreditCardPayment(){Amount = 5000m},
    new PaypalPayment(){Amount = 200m}
};

/*
 * GetValidPayment expects an IEnumerable of Payment and a validator of Payment
 * as digitalPayments is an IEnumerable of DigitalPayment(That inherit from Payment ) and PaymentValidator is a validator of Payment
 * we can pass digitalPayments and PaymentValidator to GetValidPayment
 * Covariant allows us to pass a more derived type to a method that expects a less derived type
 */

var paymentValidator = new PaymentValidator();
 PrintPayment(GetValidPayment(digitalPayments, paymentValidator));
 PrintPayment(GetValidPayment(payments, paymentValidator));

 /* Remove in from IValidator<T> and Uncomment the line below to see the error */
 //PrintPayment(GetValidDigitalPayment(digitalPayments, paymentValidator));

/* Why this doesn't work?
 * GetValidDigitalPayment expects an IEnumerable of DigitalPayment and a validator of DigitalPayment
 * paymentValidator is a validator of Payment and not of DigitalPayment
 * we can't pass IValidator<Payment> to IValidator<DigitalPayment>
 * uncomment the line below to see the error */
 //IValidator<DigitalPayment> digital = new PaymentValidator(); // IValidator<Payment>
 /* The same reason we can't assign Payment to DigitalPayment
 * uncomment the line below to see the error */
 //DigitalPayment digital = new Payment();
 
/* Add in to IValidator<T> and Uncomment the line below, It will work again */
// PrintPayment(GetValidDigitalPayment(digitalPayments, paymentValidator));
// This is contravariance

//Another way to solve this problem is to use a extension method
PrintPayment(digitalPayments.WhereAmountGraterThanZero(paymentValidator.Validate));
// Just another example of extension method, with a constraint
PrintPayment(payments.WherePaymentAmountGraterThanZero(paymentValidator.Validate));

//Because the parameter type of the extension method is generic we can pass a lambda expression of any type

var intList =new List<int> {1, 3, 5};
intList.WhereAmountGraterThanZero((int id) => id > 0);

// We can also use a delegate function
Func<int, bool> func = (int id) => id > 0;
intList.WhereAmountGraterThanZero(func);


static void PrintPayment(IEnumerable<Payment> payments)
{
    foreach (var pay in payments)
    {
        Console.WriteLine($"Payment: {pay.Amount}");
    }
}

List<Payment> GetValidPayment(IEnumerable<Payment> payments, IValidator<Payment> validator)
{
    return payments.Where(x => validator.Validate(x)).ToList();
}

IEnumerable<DigitalPayment> GetValidDigitalPayment(IEnumerable<DigitalPayment> payments, IValidator<DigitalPayment> validator)
{
    // Also valid
    // payments.Where(validator.Validate);
    return payments.Where(x => validator.Validate(x));
}

/*
 We added the keyword in to the generic interface IValidator
 This allows us to pass a more derived type to a method that expects a less derived type
 This is called contravariant
 The keyword in is used to specify that a type parameter is contravariant
 and it's only used as a parameter type in methods never in return types 
 */
public interface IValidator<in T>
 {
     bool Validate(T item);
 }
 
 public class PaymentValidator : IValidator<Payment>
 {
     public bool Validate(Payment item)
     {
         return item.Amount > 0;
     }
 }
 
// Extension method to IEnumerable<T>
// Must be a static class
 public static class EnumerableExtensions
 {
     // Must be a static method
        // The this keyword specifies that we're adding this method to IEnumerable<T> (extending IEnumerable<T>)
        // We're passing a predicate to filter the IEnumerable<T> as a parameter
        // It's a function that takes a T and returns a bool (This function is a delegate) AKA Callback Function in other languages
     
     public static IEnumerable<T> WhereAmountGraterThanZero<T>(this IEnumerable<T> source, Func<T, bool> predicate)
     {
         List<T> result = new();
         foreach (var item in source)
         {
             // We're calling the predicate function with the item as a parameter
             if (predicate(item))
             {
                 result.Add(item);
             }
         }

         return result;
     }
     
     // Same method but with a constraint
        // We're constraining T to Payment
     public static IEnumerable<Payment> WherePaymentAmountGraterThanZero(this IEnumerable<Payment> source, Func<Payment, bool> predicate)
     {
         List<Payment> result = new();
         foreach (var item in source)
         {
             if (predicate(item))
             {
                 result.Add(item);
             }
         }

         return result.ToArray();
     }
     
 }