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
using Services.TableServices;

namespace Resturant_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.User)]
    public class TableController : ControllerBase
    {
        private ITableService _tableService;
        private IExceptionModelGenerator _exceptionModelGenerator;
        public TableController(ITableService tableService, IExceptionModelGenerator exceptionModelGenerator)
        {
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

                var result = _exceptionModelGenerator.setData<Table>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<TableCategory>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }


        [HttpGet("{ResturantId}/baseTableCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> BaseCategory(string ResturantId)
        {
            try
            {
                var tableRes = await _tableService.GetBaseCategory(ResturantId);
                if (tableRes != null)
                {

                    var resul = _exceptionModelGenerator.setData<List<TableCategory>>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, "Ok", null);
                return StatusCode(500, result);
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

                    var resul = _exceptionModelGenerator.setData<List<TableCategory>>(false, "Ok", tableRes);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<List<TableCategory>>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpGet("Tables/{categoryId}")]
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
                return StatusCode(500, result);
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

                var result = _exceptionModelGenerator.setData<Table>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {

                var result = _exceptionModelGenerator.setData<Table>(true, e.Message, null);
                return StatusCode(500, result);
            }

        }
        
    }
}
