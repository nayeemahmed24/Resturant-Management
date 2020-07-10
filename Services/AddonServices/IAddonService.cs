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
    }
}
