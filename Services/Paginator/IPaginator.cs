using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Model.View_Model;

namespace Services.Paginator
{
    public interface IPaginator
    {
        PaginatorModel<Type> GetPaginatedData<Type>(string sortBy, int pageNumber, int pageSize, Expression<Func<Type, bool>> datafilters);
    }
}
