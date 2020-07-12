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
            if (resAddon.ParentMenuItem == null)
            {
                return null;
            }

            await _repository.SaveAsync<Addon>(resAddon);
            
            return resAddon;
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

        public async Task<List<Addon>> AllAddonByMenuItemId(string menuItemId)
        {
            List<Addon> lists = new List<Addon>();
            if (menuItemId != null)
            {
                var list = await _repository.GetItemsAsync<Addon>(d => d.ParentMenuItem.Id == menuItemId);
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


        private async Task<Addon> FindAddonById(string AddonInputId)
        {
            if (AddonInputId != null)
            {
                var addon = await _repository.GetItemAsync<Addon>(d => d.Id == AddonInputId);
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
            };
            resAddon.ParentMenuItem = await _menuServices.FindMenuItemById(addon.ParentMenuItemId);
            resAddon.Available = false;
            return resAddon;

        }
    }
}
