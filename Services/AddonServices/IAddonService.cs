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
        public Task<Addon> UpdateAddon(AddonInput addonInput);
        public  Task<Addon> ChangeStatus(AddonInput addonInput);
        public Task<List<Addon>> AllAddonByResturantId(string resturantId);
        public Task<MenuItem> AssignAddon(AssignAddon assignAddon);
        public Task<List<Addon>> FindAddonByItemId(string ItemId);
        public Task<Addon> FindAddonById(string AddonInputId);
    }
}
