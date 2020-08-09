using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Model.Input_Model;

namespace Services.AddonServices
{
    public interface IAddonService
    {
        public  Task<Addon> AddAddon(AddonInput addon);
        public Task<AddonCategory> AddAddonCategory(AddonCategory addonCategory);
        public Task<Addon> UpdateAddon(AddonInput addonInput);
        public  Task<Addon> ChangeStatus(Addon addonInput);
        public Task<List<Addon>> AllAddonByResturantId(string resturantId);
        public Task<dynamic> AssignAddon(AssignAddon assignAddon);
        public Task<List<Addon>> FindAddonByItemId(string ItemId);
        public Task<Addon> FindAddonById(string AddonInputId);
        public Task<AddonCategory> FindAddonCategoryById(string AddonCategortId);
        public Task<List<Addon>> AllAddonChildByCategoryId(string CategoryId);
        public Task<List<AddonCategory>> FindBaseAddonCategory(string resturantId);
        public Task DeleteAddon(string addonid);
        public Task DeleteAddonCategory(string addoncategoryid);

    }
}
