using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLib.ShoppersStore.Interfaces;
using ServiceLib.ShoppersStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using EF.Core.ShoppersStore.ShoppersStoreDB.Models;

namespace API.SS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;

        public ProductController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        [Authorize(Roles = "Shopper,Admin,Manager")]
        [HttpGet]
        [Route("getCategories")]
        public IActionResult GetCategories()
        {
            try
            {
                // throw new Exception();
                var _categories = _productRepo.GetCategories();
                return Ok(_categories);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize("Admin")]
        [HttpPost]
        [Route("addProduct")]
        public IActionResult AddProduct(Product product)
        {
            try
            {
                // throw new Exception();

                if (ModelState.IsValid)
                {
                    var newProduct = _productRepo.AddProduct(product);
                    if (newProduct!=null)
                        return Ok(new { ResponseCode = 200, ResponseMessage = "Product Created Successfully!" ,NewProduct = newProduct });
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, new { ResponseCode = 500, ResponseMessage = "Server Error!" });
                }
                else
                {
                    return BadRequest(ModelState);
                    /*
                    return BadRequest(ModelState.ToDictionary(
                         kvp => kvp.Key,
                         kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                         )
                    );
                    */
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ResponseCode = 500, ResponseMessage = "Server Error!" });
            }
        }
    }
}
