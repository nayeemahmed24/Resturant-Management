using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class PageParameters
    {
        const int maxPageSize = 15;
        private int pageSize = 10;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value > maxPageSize ? maxPageSize : value;
            }
        }
    }
}
