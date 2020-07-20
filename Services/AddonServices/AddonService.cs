using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Model.Input_Model;
using Repository;
using Services.MenuServices;
using Services.UserServices;

namespace Services.AddonServices
{
    public class AddonService:IAddonService
    {
        private IMongoRepository _repository;
        private IUserAccessService _userAccessService;
        private IMenuServices _menuServices;
        public AddonService(IMongoRepository respository,IMenuServices menuService, IUserAccessService userAccessService)
        {
            _repository = respository;
            _menuServices = menuService;
            _userAccessService = userAccessService;
        }

        public async Task<Addon> AddAddon(AddonInput addon)
        {
            var resAddon = await buildAddon(addon);
            
            await _repository.SaveAsync<Addon>(resAddon);
            
            return resAddon;
        }

        public async Task<List<AddonCategory>> FindChildAddonCategoryById(string addonCategoryId)
        {
            var addonCategories = await _repository.GetItemsAsync<AddonCategory>(d => d.ParentId == addonCategoryId);
            var list = addonCategories?.ToList();
            return list;
        }

        public async Task<AddonCategory> AddAddonCategory(AddonCategory addonCategory)
        {
            addonCategory.Id = Guid.NewGuid().ToString();
            if(addonCategory.ParentId != null)
            {
                var ParentCat = await FindAddonCategoryById(addonCategory.ParentId);
                if (ParentCat == null) return null;
            }

            await _repository.SaveAsync<AddonCategory>(addonCategory);
            return addonCategory;

        }



        public async Task<Addon> UpdateAddon(AddonInput addonInput)
        {
            var addon = await FindAddonById(addonInput.Id);
            
            if (addon != null)
            {
                var resAddon = addon;
                resAddon.AddonTitle = addonInput.AddonTitle;
                resAddon.Price = addonInput.Price;
                await _repository.UpdateAsync<Addon>(d => d.Id == resAddon.Id, resAddon);
                return resAddon;
            }

            return null;
        }

        public async Task<List<Addon>> AllAddonByResturantId(string resturantId)
        {
            List<Addon> lists = new List<Addon>();
            if (resturantId != null)
            {
                var list = await _repository.GetItemsAsync<Addon>(d => d.ResturantId == resturantId);
                list?.ToList();
                if (list != null)
                {
                    lists = list?.ToList();
                }
                
            }
            return lists;
        }

        public async Task<List<Addon>> AllAddonChildByCategoryId(string CategoryId)
        {
            List<Addon> lists = new List<Addon>();
            if (CategoryId != null)
            {
                var list = await _repository.GetItemsAsync<Addon>(d => d.ParentId == CategoryId);
                list?.ToList();
                if (list != null)
                {
                    lists = list?.ToList();
                }

            }
            return lists;
        }
        public async Task<Addon> ChangeStatus(AddonInput addonInput)
        {
            var addon = await  FindAddonById(addonInput.Id);
            if (addon != null)
            {
                addon.Available = !addon.Available;
                await _repository.UpdateAsync<Addon>(d => d.Id == addon.Id, addon);
                return addon;
            }

            return null;
        }

        public async Task<MenuItem> AssignAddon(AssignAddon assignAddon)
        {
            if (assignAddon != null)
            {
                var menu = await _menuServices.FindMenuItemById(assignAddon.MenuItemId);
                if (menu != null)
                {
                    var addon = await FindAddonById(assignAddon.AddonId);
                    if (addon != null)
                    {
                        if (menu.AddonsList == null)
                        {
                            var list = new List<string>();
                            list.Add(assignAddon.AddonId);
                            menu.AddonsList = list;
                            return await _menuServices.UpdateMenuByMenuItem(menu);
                            
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<Addon>> FindAddonByItemId(string ItemId)
        {
            var list = new List<Addon>();
            if (ItemId != null)
            {
                var menu = await _menuServices.FindMenuItemById(ItemId);
                if (menu != null)
                {
                    if (menu.AddonsList != null)
                    {
                        if (menu.AddonsList.Count > 0)
                        {
                            foreach (var id in menu.AddonsList)
                            {
                                var Addon = await FindAddonById(id);
                                if(Addon!=null)list.Add(Addon);
                            }
                            
                        }
                    }
                }
            }

            return list;
        }


        public async Task<Addon> FindAddonById(string AddonInputId)
        {
            if (AddonInputId != null)
            {
                var addon = await _repository.GetItemAsync<Addon>(d => d.Id == AddonInputId);
                return addon;
            }

            return null;
        }
        public async Task<List<AddonCategory>> FindBaseAddonCategory(string resturantId)
        {
            if (resturantId != null)
            {
                var addon = await _repository.GetItemsAsync<AddonCategory>(d => d.RestaurantId == resturantId && d.ParentId == null);
                return addon?.ToList();
            }

            return null;
        }

        public async Task DeleteAddon(string addonid)
        {
            var addon = await FindAddonById(addonid);
            if (addon != null)
            {
                await _repository.DeleteAsync<Addon>(d => d.Id == addonid);
            }
        }

        public async Task DeleteAddonCategory(string addoncategoryid)
        {
            var addoncat = await FindAddonCategoryById(addoncategoryid);
            if (addoncat != null)
            {
                await DeleteRec(addoncategoryid);
            }
        }
        public async Task DeleteRec(string addonid)
        {
            var addons = await AllAddonChildByCategoryId(addonid);
            await DelCat(addonid);
            if (addons != null)
            {
                foreach (var addon in addons)
                {
                    await DeleteAddon(addon.Id);
                }
            }
            else
            {
                var cats = await FindChildAddonCategoryById(addonid);
                if (cats != null)
                {
                    foreach (var cat in cats)
                    {
                        await DeleteRec(cat.Id);
                    }
                }
            }
        }

        private async Task Delete(string id)
        {
            var table = await FindAddonById(id);
            if (table != null)
            {
                await DeleteAddon(id);
            }
            else
            {
                await DelCat(id);
            }
        }

        private async Task DelCat(string id)
        {
            var cat = await FindAddonCategoryById(id);
            if (cat != null)
            {
                await _repository.DeleteAsync<AddonCategory>(d => d.Id == cat.Id);
            }
        }
        public async Task<AddonCategory> FindAddonCategoryById(string AddonCategortId)
        {
            if (AddonCategortId != null)
            {
                var addon = await _repository.GetItemAsync<AddonCategory>(d => d.Id == AddonCategortId);
                return addon;
            }

            return null;
        }
        private async Task<Addon> buildAddon(AddonInput addon)
        {
            var resAddon = new Addon
            {
                AddonTitle = addon.AddonTitle,
                Price = addon.Price,
                ParentId = addon.AddonCategoryId,
            };
            resAddon.ResturantId = addon.ResturantId;
            resAddon.Available = false;
            return resAddon;

        }
    }
}
