using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Repository;

namespace Services.TableServices
{
    public class TableService : ITableService
    {
        private IMongoRepository _repository;
        public TableService(IMongoRepository repository)
        {
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

                    tableCategory.ParentId = TableCategory.Id;
                }

                await _repository.SaveAsync<TableCategory>(tableCategory);
                return tableCategory;
            }
            return null;
        }

        public async Task<Table> AddTable(Table table)
        {
            if (table.TableTitle != null)
            {
                if (table.TableCategoryId != null)
                {
                    var TableCategory = await FindTableCategoryById(table.TableCategoryId);
                    if (TableCategory == null)
                    {
                        return null;
                    }

                    table.Id = Guid.NewGuid().ToString();
                    await _repository.SaveAsync<Table>(table);
                    return table;
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

        
    }
}
