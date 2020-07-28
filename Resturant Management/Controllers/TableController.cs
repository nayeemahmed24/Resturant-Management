using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Entities;
using Model.Error_Handler;
using Repository;
using Services.Sort_Service;
using Services.TableServices;

namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.User)]
    public class TableController : ControllerBase
    {
        private ISortService _sortService;
        private ITableService _tableService;
        private IExceptionModelGenerator _exceptionModelGenerator;
        public TableController(ISortService sortService,ITableService tableService, IExceptionModelGenerator exceptionModelGenerator)
        {
            _sortService = sortService;
            _tableService = tableService;
            _exceptionModelGenerator = exceptionModelGenerator;
        }

        [HttpPost("createTableCategory")]
        public async Task<IActionResult> AddTableCategory(TableCategory table)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                table.ResturantId = userId;
                var tableRes = await _tableService.AddTableCategory(table);
                if (tableRes != null)
                {
                    var resul = _exceptionModelGenerator.setData<TableCategory>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Table>(true, "Bad request", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<TableCategory>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpPut("editTableCategory")]
        public async Task<IActionResult> EditTableCategory(TableCategory table)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                table.ResturantId = userId;
                var tableRes = await _tableService.EditTableCategory(table);
                if (tableRes != null)
                {
                    var resul = _exceptionModelGenerator.setData<TableCategory>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Table>(true, "Bad request", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<TableCategory>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }
        [HttpGet("baseTableCategory/{resturantid}")]
        [AllowAnonymous]
        public async Task<IActionResult> BaseCategory(string resturantid)
        {
            
            try
            {
                var tableRes = await _tableService.GetBaseCategory(resturantid);
                if (tableRes != null || tableRes.Count!=0)
                {
                    var sort = await _sortService.FindSortUsingParentId("tablebase");
                    tableRes = _sortService.SortTableCategories(sort, tableRes);
                    var resul = _exceptionModelGenerator.setData<List<TableCategory>>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, "No data", null);
                return StatusCode(404, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpGet("childCategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ChildCategory(string categoryId)
        {

            try
            {
                var tableRes = await _tableService.GetChildTableCategoryListByTableCategoryId(categoryId);
                if (tableRes != null)
                {
                    var sort = await _sortService.FindSortUsingParentId(categoryId);
                    tableRes = _sortService.SortTableCategories(sort, tableRes);
                    var resul = _exceptionModelGenerator.setData<List<TableCategory>>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, "No data", null);
                return StatusCode(404, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpGet("tables/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Tables(string categoryId)
        {

            try
            {
                var tableRes = await _tableService.GetChildTableListByTableCategoryId(categoryId);
                if (tableRes != null)
                {

                    var resul = _exceptionModelGenerator.setData<List<Table>>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<List<Table>>(true, "Ok", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<List<Table>>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpPost("Addtable")]
        public async Task<IActionResult> CreateTable(Table table)
        {
            try
            {
                var tableRes = await _tableService.AddTable(table);
                if (tableRes != null)
                {

                    var resul = _exceptionModelGenerator.setData<Table>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Table>(true, "Bad request", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<Table>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpPut("edittable")]
        public async Task<IActionResult> EditTable(Table table)
        {
            try
            {
                var tableRes = await _tableService.EditTable(table);
                if (tableRes != null)
                {

                    var resul = _exceptionModelGenerator.setData<Table>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Table>(true, "Bad request", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<Table>(true, e.Message, null);
                return StatusCode(500, result);
            }

        }

        [HttpPost("sortedit")]
        public async Task<IActionResult> EditSort(SortOrder sort)
        {
            try
            {
                if (sort.ParentId != null)
                {
                    if (sort.SortList != null)
                    {
                        var res = await _sortService.EditSort(sort);
                        if (res != null)
                        {
                            var resul = _exceptionModelGenerator.setData<SortOrder>(false, "Ok", res);
                            return StatusCode(201, resul);
                        }
                    }
                }
                var result = _exceptionModelGenerator.setData<SortOrder>(true, "Ok", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<SortOrder>(true, e.Message, null);
                return StatusCode(500, reslt);
            }
        }

        [HttpGet("deletecategory/{categoryid}")]
        public async Task<IActionResult> Delete(string categoryid)
        {
            try
            {
                await _tableService.DeleteCategory(categoryid);
                var result = _exceptionModelGenerator.setData<string>(false, "Sucess", null);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<SortOrder>(true, e.Message, null);
                return StatusCode(500, reslt);
            }
        }

        [HttpGet("delete/{tableid}")]
        public async Task<IActionResult> DeleteTable(string tableid)
        {
            try
            {
                await _tableService.DeleteTableWithAll(tableid);
                var result = _exceptionModelGenerator.setData<string>(false, "Sucess", null);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<SortOrder>(true, e.Message, null);
                return StatusCode(500, reslt);
            }
        }
        private async Task<SortOrder> GetSortOrder(string parentId)
        {
            return await _sortService.FindSortUsingParentId(parentId);
        }
    }
}
