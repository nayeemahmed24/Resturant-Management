using Model.Entities;
using Model.View_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.UtilityService
{
    public static class RestaurantExtensionMehods
    {
        public static RestaurantModel RemovePassword(this RestaurantModel restaurantModel)
        {
            if(restaurantModel == null)
            {
                return null;
            }

            restaurantModel.password = null;

            return restaurantModel;
        }

        public static PaginatorModel<RestaurantModel> RemoveAllPassword(this PaginatorModel<RestaurantModel> paginatorModel)
        {
            paginatorModel.data = paginatorModel.data.Select(data => data.RemovePassword());
            return paginatorModel;
        }
    }
}
