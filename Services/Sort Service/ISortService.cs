using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Services.Sort_Service
{
    public interface ISortService
    {
        public Task AddSort(string parentId, string childId);
        public Task<SortOrder> EditSort(SortOrder sort);
        public Task<SortOrder> FindSortUsingParentId(string parentId);
        public List<MenuCatergory> SortCategory(SortOrder sort, List<MenuCatergory> catergories);


    }
}
