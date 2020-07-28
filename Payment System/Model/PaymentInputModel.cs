using System;
using System.Collections.Generic;
using System.Text;

namespace Payment_System.Model
{
    public class PaymentInputModel
    {
        public string cardNumber { get; set; }

        public int month { get; set; }

        public int year { get; set; }

        public string cvc { get; set; }

        public int value { get; set; }

        public string OrderId { set; get; }
    }
}
