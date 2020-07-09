using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Error_Handler
{
    public class ExeptionModel<Type>
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public Type Result { get; set; }
    }
}
