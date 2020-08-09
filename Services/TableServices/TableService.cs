using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Repository;
using Services.Sort_Service;

namespace Services.TableServices
{
    public class TableService : ITableService
    {
        private IMongoRepository _repository;
        private ISortService _sortService;
        public TableService(ISortService sortService,IMongoRepository repository)
        {
            _sortService = sortService;
            _repository = repository;
        }

        public async Task<TableCategory> AddTableCategory(TableCategory tableCategory)
        {
            if (tableCategory.CategoryTitle != null)
            {
                tableCategory.Id = Guid.NewGuid().ToString();
                if (tableCategory.ParentId != null)
                {
                    var TableCategory = await FindTableCategoryById(tableCategory.ParentId);
                    if (TableCategory == null)
                    {
                        return null;
                    }

                    await _sortService.AddSort(tableCategory.ParentId, tableCategory.Id, true);
                    tableCategory.ParentId = TableCategory.Id;
                }
                else
                {
                    await _sortService.AddSort(null, tableCategory.Id, true);
                }

                await _repository.SaveAsync<TableCategory>(tableCategory);
                return tableCategory;
            }
            return null;
        }

        public async Task<TableCategory> EditTableCategory(TableCategory tableCategory)
        {
            if (tableCategory.Id != null)
            {
                var res = await FindTableCategoryById(tableCategory.Id);
                var parentCategory = new TableCategory();
                
                if (tableCategory.ParentId != null)
                {
                    parentCategory = await FindTableCategoryById(tableCategory.ParentId);
                    if (parentCategory == null)
                    {
                        return null;
                    }

                    res.ParentId = parentCategory.Id;

                }

                if (res != null)
                {
                    res.CategoryTitle = tableCategory.CategoryTitle;

                    await _repository.UpdateAsync<TableCategory>(d => d.Id == res.Id, res);
                }

               
                return tableCategory;
            }
            return null;
        }
        public async Task<Table> AddTable(Table table)
        {
            
            if (table.TableTitle >0)
            {
                if (table.TableCategoryId != null)
                {
                    var TableCategory = await FindTableCategoryById(table.TableCategoryId);
                    if (TableCategory == null)
                    {
                        return null;
                    }
                    table.Id = Guid.NewGuid().ToString();
                    await _sortService.AddSort(table.TableCategoryId, table.Id, true);
                    await _repository.SaveAsync<Table>(table);
                    return table;
                }
            }
            return null;
        }
        public async Task<Table> EditTable(Table table)
        {
            if (table.Id != null)
            {
                var res =await FindTableById(table.Id);
                var parentCategory = new TableCategory();
                if (table.TableTitle > 0)
                {
                    if (table.TableCategoryId != null)
                    {
                        parentCategory = await FindTableCategoryById(table.TableCategoryId);
                        if (parentCategory == null)
                        {
                            return null;
                        }
                    }

                    res.TableTitle = table.TableTitle;
                    res.TableCategoryId = parentCategory.Id;
                    await _repository.UpdateAsync<Table>(d=>d.Id == table.Id,res);
                    return res;
                }
            }
            return null;
        }
        public async Task<List<Table>> GetChildTableListByTableCategoryId(string tableCategoryId)
        {
            var list = new List<Table>();
            if (tableCategoryId != null)
            {
                var res = await _repository.GetItemsAsync<Table>(d => d.TableCategoryId == tableCategoryId);
                list = res?.ToList();
            }

            return list;
        }
        public async Task<List<TableCategory>> GetBaseCategory(string resturantId)
        {
            var list = new List<TableCategory>();
            if (resturantId != null)
            {
                var res = await _repository.GetItemsAsync<TableCategory>(d =>  d.ResturantId == resturantId && d.ParentId == null);
                list = res?.ToList();
            }

            return list;
        }
        public async Task<List<TableCategory>> GetChildTableCategoryListByTableCategoryId(string ParentTableCategoryId)
        {
            var list = new List<TableCategory>();
            if (ParentTableCategoryId != null)
            {
                var res = await _repository.GetItemsAsync<TableCategory>(d => d.ParentId == ParentTableCategoryId  );
                list = res?.ToList();
            }

            return list;
        }


        public async Task<TableCategory> FindTableCategoryById(string Id)
        {
            return await _repository.GetItemAsync<TableCategory>(d => d.Id == Id);
        }
        public async Task<Table> FindTableById(string Id)
        {
            return await _repository.GetItemAsync<Table>(d => d.Id == Id);
        }

        public async Task DeleteTable(string Id)
        {
            await _repository.DeleteAsync<Table>(d => d.Id == Id);
        }
        public async Task DeleteTableCategory(string Id)
        {
            await _repository.DeleteAsync<TableCategory>(d => d.Id == Id);
        }

        public async Task DeleteCategory(string id)
        {
            var Category = await FindTableCategoryById(id);
            if (Category != null)
            {
                string parent;
                if (Category.ParentId == null)
                {
                    parent = "tablebase";
                }
                else
                {
                    parent = Category.ParentId;
                }
                // List Theke out
                if (parent != null)
                {
                    var sort = await _sortService.FindSortUsingParentId(parent);
                    if (sort != null)
                    {
                        if (sort.SortList != null)
                        {
                            sort.SortList.Remove(Category.Id);
                        }

                        var UpSort = await _sortService.EditSort(sort);
                    }
                }

                await Delete(Category.Id);
                var delsort = await _sortService.FindSortUsingParentId(Category.Id);
                if (delsort != null)
                {
                    await DeleteRec(delsort.ParentId);
                }

            }

        }
        public async Task DeleteTableWithAll(string id)
        {
            var table = await FindTableById(id);
            if (table != null)
            {
                // List Theke out
                if (table.TableCategoryId != null)
                {
                    var sort = await _sortService.FindSortUsingParentId(table.TableCategoryId);
                    if (sort != null)
                    {
                        if (sort.SortList != null)
                        {
                            sort.SortList.Remove(table.Id);
                        }

                        var UpSort = await _sortService.EditSort(sort);
                    }
                }

                await Delete(table.Id);
            }
        }
        public async Task DeleteRec(string sortid)
        {
            var sorts = await _sortService.FindSortUsingParentId(sortid);
            if (sorts != null)
            {

                foreach (var child in sorts.SortList)
                {
                    var unit = await _sortService.FindSortUsingParentId(child);
                    if (unit != null)
                    {
                        await DeleteRec(child);
                    }
                    else
                    {
                        await Delete(child);
                    }

                }

                await Delete(sortid);
                await _sortService.DeleteSort(sorts);

            }
        }

        private async Task Delete(string id)
        {
            var table = await FindTableById(id);
            if (table != null)
            {
                await DeleteTable(id);
            }
            else
            {
                var cat = await FindTableCategoryById(id);
                if (cat != null)
                {
                    await DeleteTableCategory(id);
                }
            }
        }

        public async Task<TableCategory> CategoryStatus(string id)
        {
            var category = await FindTableCategoryById(id);
            if (category != null)
            {
                category.available = !category.available;
                await _repository.UpdateAsync<TableCategory>(d => d.Id == id, category);
                return category;
            }
            return null;
        }

        public async Task<Table> TableStatus(string id)
        {
            var table = await FindTableById(id);
            if (table != null)
            {
                table.available = !table.available;
                await _repository.UpdateAsync<Table>(d => d.Id == id, table);
                return table;
            }
            return null;
        }
    }
}
