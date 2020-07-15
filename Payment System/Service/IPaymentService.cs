using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Payment_System.Model;

namespace Payment_System.Service
{
    public interface IPaymentService
    {
        public Task<dynamic> MakePayment(PaymentInputModel pm);
    }
}
