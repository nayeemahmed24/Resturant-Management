using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Services.TableServices
{
    public interface ITableService
    {
        public Task<TableCategory> AddTableCategory(TableCategory tableCategory);
        public Task<TableCategory> EditTableCategory(TableCategory tableCategory);
        public Task<Table> AddTable(Table table);
        public Task<TableCategory> CategoryStatus(string id);
        public Task<Table> TableStatus(string id);
        public Task<Table> EditTable(Table table);
        public Task<List<Table>> GetChildTableListByTableCategoryId(string tableCategoryId);
        public  Task<List<TableCategory>> GetBaseCategory( string resturantId);
        public Task<List<TableCategory>> GetChildTableCategoryListByTableCategoryId(string ParentTableCategoryId);
        public Task DeleteCategory(string id);
        public  Task DeleteTableWithAll(string id);

    }
}
