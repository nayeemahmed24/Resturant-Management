using System;
using System.Collections.Generic;
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
