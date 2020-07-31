using System;
using System.Collections.Generic;
using System.Text;

namespace Model.View_Model
{
    public class PaginatorModel<Type>
    {
        public IEnumerable<Type> data { get; set; }
        public int totalData { get; set; }
    }
}
