using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Repository;

namespace Services.Sort_Service
{
    public class SortService : ISortService
    {
        private readonly IMongoRepository  _repository;
        public SortService(IMongoRepository repository)
        {
            _repository = repository;
        }

        public async Task AddSort(string parentId, string childId,bool isTable)
        {
            if (parentId == null && isTable == false)
            {
                parentId = "base";
            }
            if (parentId == null && isTable )
            {
                parentId = "tablebase";
            }

            if (parentId != null)
            {
                var sort = await FindSortUsingParentId(parentId);
                if (sort == null)
                {

                    var newSort = new SortOrder
                    {
                        ParentId = parentId,
                        SortList = new List<string>()
                    };
                    newSort.SortList.Add(childId);
                    await _repository.SaveAsync<SortOrder>(newSort);
                }
                else
                {
                    sort.SortList.Add(childId);
                    await _repository.UpdateAsync<SortOrder>(d => d.ParentId == parentId, sort);
                }
                
            }
            
            
        }
        public async Task<SortOrder> EditSort(SortOrder sort)
        {
            if (sort.ParentId != null)
            {
                var resSort = await FindSortUsingParentId(sort.ParentId);
                if (resSort != null)
                {
                    resSort.SortList = sort.SortList;
                    await _repository.UpdateAsync<SortOrder>(d => d.ParentId == sort.ParentId, resSort);
                    return sort;
                }

            }
            return null;
        }

        

       public async Task<SortOrder> FindSortUsingParentId(string parentId)
       {
            return await _repository.GetItemAsync<SortOrder>(d => d.ParentId == parentId);
       }


       public async Task DeleteSort(SortOrder sort)
       {
           await _repository.DeleteAsync<SortOrder>(d => d.Id == sort.Id);
       }
       public List<MenuCatergory> SortCategory(SortOrder sort, List<MenuCatergory> catergories)
       {
           var res = new List<MenuCatergory>();
           foreach (var sortId in sort.SortList)
           {
               var resulty = catergories.Find(d => d.Id == sortId);
               if (resulty != null)
               {

                   res.Add(resulty);
               }
            }
           return res;
       }

       public List<MenuItem> SortItems(SortOrder sort, List<MenuItem> menuItems)
       {
           var res = new List<MenuItem>();
           foreach (var sortId in sort.SortList)
           {
               var resulty = menuItems.Find(d => d.Id == sortId);
               if (resulty != null)
               {

                   res.Add(resulty);
               }
            }

           return res;
       }

       public List<TableCategory> SortTableCategories(SortOrder sort, List<TableCategory> menuItems)
       {
           var res = new List<TableCategory>();
           foreach (var sortId in sort.SortList)
           {
               var resulty = menuItems.Find(d => d.Id == sortId);
               if (resulty != null)
               {

                   res.Add(resulty);
               }
           }

           return res;
        }
    }
}
