using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Model.Entities;
using Model.Input_Model;
using Repository;
using Services.Sort_Service;
using Services.UserServices;

namespace Services.MenuServices
{
    public class MenuServices:IMenuServices
    {
        private IMongoRepository _repository;
        private IUserAccessService _userAccessService;
        private ISortService _sortService;
        public MenuServices(ISortService sort,IMongoRepository repository,IUserAccessService userAccessService)
        {
            _sortService = sort;
            _userAccessService = userAccessService;
            _repository = repository;
        }
        
        public async Task<MenuCatergory> AddCategory(MenuCategoryInput menuCategoryInput)
        {
            var category = await buildCatergory(menuCategoryInput);
            category.Id = GetUniqueId();
            MenuCatergory parent;
            if (category.Parent != null)
            {
                parent = _repository.GetItem<MenuCatergory>(c => c.Id == category.Parent);
                if (parent.ItemAdded)
                {
                    return null;
                }
            }

            category.Restaurant.password = null;
   
            await _repository.SaveAsync<MenuCatergory>(category);
            if (category.Parent != null)
            {
                parent = _repository.GetItem<MenuCatergory>(c => c.Id == category.Parent);
                await UpdateChildAvailable(parent);

                await _sortService.AddSort(category.Parent, category.Id);
            }
            else 
                await _sortService.AddSort(null, category.Id);
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
            var parent = await FindParentByMenuItemInput(menu);
            if (menuItem.ParentId != null)
            {
                if (parent.IsChildAvailable) return null;

            }

            menuItem.Id = Guid.NewGuid().ToString();
            menuItem.Restaurant.password = null; 
            await _repository.SaveAsync<MenuItem>(menuItem);
            await _sortService.AddSort(menu.ParentId, menuItem.Id);
            if (!parent.ItemAdded)
            {
                await UpdateItemAdded(parent);
            }

            return menuItem;

        }

        public async Task<MenuItem> UpdateMenu(MenuItemInput menu)
        {
            var menuItem = await FindMenuByMenuItemInput(menu);
            menuItem.Restaurant.password = null;
            if (menuItem == null) return null;
            menuItem.ItemTitle = menu.ItemTitle;
            menuItem.Price = menu.Price;
            

            await _repository.UpdateAsync<MenuItem>(d => d.Id == menuItem.Id, menuItem);
            return menuItem;
        }
        public async Task<MenuCatergory> UpdateMenuCategory(MenuCategoryInput menu)
        {
            var menuItem = await FindCategoryById(menu.Id);
            menuItem.Restaurant.password = null;
            if (menuItem == null) return null;
            menuItem.CategoryTitle = menu.CategoryTitle;
            
            await _repository.UpdateAsync<MenuCatergory>(d => d.Id == menuItem.Id, menuItem);
            return menuItem;
        }


        private async Task<MenuItem> FindMenuByMenuItemInput(MenuItemInput menu)
        {
            return await _repository.GetItemAsync<MenuItem>(d => d.Id == menu.Id);
        }

        private async Task<MenuCatergory> FindCategoryById(string id)
        {
            return await _repository.GetItemAsync<MenuCatergory>(d => d.Id == id);
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

        public async Task<List<MenuItem>> FindMenuParentId(string parentId)
        {
            var items = await _repository.GetItemsAsync<MenuItem>(d => d.ParentId == parentId);
            var listItem = items?.ToList();
            return listItem;
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
                ItemAdded = false,
                
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
            category.Restaurant.password = null;
            return category;
        }

        private async Task<MenuItem> buildMenuItem(MenuItemInput menuItemInput)
        {
            var menuItem = new MenuItem
            {
                ItemTitle = menuItemInput.ItemTitle,
                Price = menuItemInput.Price,
                Available = true
            };
            if (menuItemInput.ParentId != null)
            {
                var parent = await FindParentByMenuItemInput(menuItemInput);
                if (parent != null)
                {
                    menuItem.ParentId = parent.Id.ToString();
                }
            }
            menuItem.Restaurant = _userAccessService.GetUser(menuItemInput.ResturantId);
            menuItem.Restaurant.password = null;
            return menuItem;

        }

        private async Task UpdateChildAvailable(MenuCatergory menuCatergory)
        {
            menuCatergory.IsChildAvailable = true;
            menuCatergory.Restaurant.password = null;
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
        public string GetUniqueId()
        {
            string first = DateTime.Now.ToString("yyMMddHHmmssff");
            string last = GetRandomString();
            string unique_id = first + "-" + last;

            return unique_id;
        }
        public string GetRandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 20; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString().ToLower();
        }

        

    }
}
