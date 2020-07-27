using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Payment_System.Model;
using Stripe;

namespace Payment_System.Service
{
    public class PaymentSevice:IPaymentService
    {
        
        public async Task<bool> MakePayment(PaymentInputModel pm)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51H6B94HNroh8Enm6yjNDBW3eZHJNYAHmmZ2FEseqT9VomRePdbeJz02bBxoZLQbFAY6xLaPkuOvaN8LysjVGgAS600SNByQSwC";
                var optionsToken = new TokenCreateOptions
                {
                    Card = new CreditCardOptions
                    {
                        Number = pm.cardNumber,
                        ExpMonth = pm.month,
                        ExpYear = pm.year,
                        Cvc = pm.cvc,
                    }
                };
                var serviceToken = new TokenService();
                Token stripeToken = await serviceToken.CreateAsync(optionsToken);

                var options = new ChargeCreateOptions
                {
                    Amount = pm.value,
                    Currency = "usd",
                    Description = "testmode",
                    Source = stripeToken.Id,
                };
                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                if (charge.Paid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
