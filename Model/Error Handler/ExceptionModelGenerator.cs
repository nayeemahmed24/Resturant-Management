using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Error_Handler
{
    public class ExceptionModelGenerator : IExceptionModelGenerator
    {
        public ExeptionModel<Type> setData<Type>(bool error, string message, Type result)
        {
            ExeptionModel<Type> exeptionModel = new ExeptionModel<Type>();
            exeptionModel.Error = error;
            exeptionModel.Message = message;
            exeptionModel.Result = result;

            return exeptionModel;
        }
    }

    public interface IExceptionModelGenerator
    {
        public ExeptionModel<Type> setData<Type>(bool error, string message, Type result);
    }
}
