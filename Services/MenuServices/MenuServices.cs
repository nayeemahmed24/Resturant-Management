using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Model.Input_Model;
using Repository;
using Services.UserServices;

namespace Services.MenuServices
{
    public class MenuServices:IMenuServices
    {
        private IMongoRepository _repository;
        private IUserAccessService _userAccessService;
        public MenuServices(IMongoRepository repository,IUserAccessService userAccessService)
        {
            _userAccessService = userAccessService;
            _repository = repository;
        }

        public async Task<MenuCatergory> AddCategory(MenuCategoryInput menuCategoryInput)
        {
            var category = await buildCatergory(menuCategoryInput);
            if (category.Parent.ItemAdded)
            {
                return null;
            }
            await _repository.SaveAsync<MenuCatergory>(category);
            if (category.Parent != null)
            {
                await UpdateChildAvailable(category.Parent);
            }
            return category;
        }

        public async Task<List<MenuCatergory>> GetChildCategories(MenuCategoryInput menuCategory)
        {
            var animals = await _repository.GetItemsAsync<MenuCatergory>(d => d.Parent.Id == menuCategory.Id && d.Restaurant.Id == menuCategory.RestaurantId);
            var list = animals?.ToList();
            return list;
        }

        public async Task<List<MenuCatergory>> GetBaseCategories(string Id)
        {
            var animals = await _repository.GetItemsAsync<MenuCatergory>(d => d.Parent == null);
            var list = animals?.ToList();
            return list;
        }

        public async Task<MenuCatergory> FindParentCatergoryById(MenuCategoryInput menuCategoryInput)
        {
            if (menuCategoryInput.RestaurantId != null)
            {
                if (menuCategoryInput.ParentId != null)
                {
                    return await _repository.GetItemAsync<MenuCatergory>(d =>
                        d.Id == menuCategoryInput.ParentId && d.Restaurant.Id == menuCategoryInput.RestaurantId);
                }
            }

            return null;
        }

        private async Task<MenuCatergory> buildCatergory(MenuCategoryInput menuCategoryInput)
        {
            var category = new MenuCatergory
            {
                CategoryTitle = menuCategoryInput.CategoryTitle,
            };
            if (menuCategoryInput.ParentId != null)
            {
                category.Parent = await FindParentCatergoryById(menuCategoryInput);
            }
            

            category.Restaurant = _userAccessService.GetUser(menuCategoryInput.RestaurantId);
            return category;
        }

        private async Task UpdateChildAvailable(MenuCatergory menuCatergory)
        {
            menuCatergory.IsChildAvailable = true;
            await _repository.UpdateAsync<MenuCatergory>(d => d.Id == menuCatergory.Id,
                menuCatergory);
        }

        private async Task UpdateItemAdded(MenuCatergory menuCatergory)
        {
            menuCatergory.ItemAdded = true;
            await _repository.UpdateAsync<MenuCatergory>(d => d.Id == menuCatergory.Id,
                menuCatergory);
        }
    }
}
