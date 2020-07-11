using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            MenuCatergory parent ;
            if (category.Parent != null)
            {
                parent = _repository.GetItem<MenuCatergory>(c => c.Id == category.Parent);
                if (parent.ItemAdded)
                {
                    return null;
                }
            }
            
            await _repository.SaveAsync<MenuCatergory>(category);
            if (category.Parent != null)
            {
                parent = _repository.GetItem<MenuCatergory>(c => c.Id == category.Parent);
                await UpdateChildAvailable(parent);
            }
            return category;
        }

        public async Task<List<MenuCatergory>> GetChildCategories(string parentId)
        {
            var animals = await _repository.GetItemsAsync<MenuCatergory>(d => d.Parent == parentId);
            var list = animals?.ToList();
            return list;
        }

        public async Task<List<MenuCatergory>> GetBaseCategories(string Id)
        {
            var animals = await _repository.GetItemsAsync<MenuCatergory>(d => d.Parent == null && d.Restaurant.Id==Id);
            var list = animals?.ToList();
            return list;
        }

        public async Task<MenuItem> AddMenuItem(MenuItemInput menu)
        {
            var menuItem = await buildMenuItem(menu);
            if (menuItem.Parent != null)
            {
                if (menuItem.Parent.IsChildAvailable) return null;

            }
            await _repository.SaveAsync<MenuItem>(menuItem);
            if (!menuItem.Parent.ItemAdded)
            {
                await UpdateItemAdded(menuItem.Parent);
            }

            return menuItem;

        }

        public async Task<MenuItem> UpdateMenu(MenuItemInput menu)
        {
            var menuItem = await FindMenuByMenuItemInput(menu);
            if (menuItem == null) return null;
            menuItem.ItemTitle = menu.ItemTitle;
            menuItem.Price = menu.Price;
            

            await _repository.UpdateAsync<MenuItem>(d => d.Id == menuItem.Id, menuItem);
            return menuItem;
        }


        private async Task<MenuItem> FindMenuByMenuItemInput(MenuItemInput menu)
        {
            return await _repository.GetItemAsync<MenuItem>(d => d.Id == menu.Id);
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
        public async Task<MenuCatergory> FindParentByMenuItemInput(MenuItemInput menuItemInput)
        {
            if (menuItemInput.ResturantId != null)
            {
                if (menuItemInput.ParentId != null)
                {
                    return await _repository.GetItemAsync<MenuCatergory>(d =>
                        d.Id == menuItemInput.ParentId && d.Restaurant.Id == menuItemInput.ResturantId);
                }
            }

            return null;
        }

        public async Task<MenuItem> FindMenuItemById(string itemId)
        {
            return await _repository.GetItemAsync<MenuItem>(d => d.Id == itemId);
        }

        private async Task<MenuCatergory> buildCatergory(MenuCategoryInput menuCategoryInput)
        {
            var category = new MenuCatergory
            {
                CategoryTitle = menuCategoryInput.CategoryTitle,
                IsChildAvailable = false,
                ItemAdded = false
            };
            if (menuCategoryInput.ParentId != null)
            {
                 var parent = await FindParentCatergoryById(menuCategoryInput);
                if (parent != null)
                {
                    category.Parent = menuCategoryInput.ParentId;
                }
            }
            

            category.Restaurant = _userAccessService.GetUser(menuCategoryInput.RestaurantId);
            return category;
        }

        private async Task<MenuItem> buildMenuItem(MenuItemInput menuItemInput)
        {
            var menuItem = new MenuItem
            {
                ItemTitle = menuItemInput.ItemTitle,
                Price = menuItemInput.Price,

            };
            if (menuItemInput.ParentId != null)
            {
                menuItem.Parent = await FindParentByMenuItemInput(menuItemInput);
            }
            menuItem.Restaurant = _userAccessService.GetUser(menuItemInput.ResturantId);
            return menuItem;

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

        public async Task<MenuItem> ChangeAvailableStatus(MenuItemInput menu)
        {
            var menuItem = await FindMenuByMenuItemInput(menu);
            if (menuItem == null) return null;
            menuItem.Available = !menuItem.Available;
            await _repository.UpdateAsync<MenuItem>(d => d.Id == menuItem.Id, menuItem);
            return menuItem;

        }
    }
}
