using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Model.Input_Model;

namespace Services.MenuServices
{
    public interface IMenuServices
    {
        public Task<MenuCatergory> AddCategory(MenuCategoryInput menuCategoryInput);
        public Task<List<MenuCatergory>> GetChildCategories(MenuCategoryInput menuCategory);
        public Task<List<MenuCatergory>> GetBaseCategories(string Id);
    }
}
