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
                    _productRepo.AddProduct(product);
                    return Ok("Product Created Successfully !");
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
                return BadRequest();
            }
        }
    }
}
