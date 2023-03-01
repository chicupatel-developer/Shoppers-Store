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
using ServiceLib.ShoppersStore.DTO;
using System.IO;

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

        // check for file type
        private string[] permittedExtensions = { ".gif", ".jpeg", ".jpg", ".png" };

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


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequestSizeLimit(40000000)]
        [Route("productFileUpload")]
        #region file upload with extra parameter,,, custom type AddProductFile_
        public async Task<IActionResult> ProductFileUpload([FromForm] AddProductFile_ addProductFile_)
        {
            ProductFileAddResponse response = new ProductFileAddResponse();
            try
            {
                // throw new Exception();

                // var postedFile = Request.Form.Files[0];
                var postedFile = addProductFile_.ProductFile;


                // check for file type
                var ext = Path.GetExtension(postedFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return BadRequest("Invalid File Type !");                   
                }


                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                if (postedFile.Length > 0)
                {
                    var fileName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + postedFile.FileName;
                    var finalPath = Path.Combine(uploadFolder, fileName);
                    using (var fileStream = new FileStream(finalPath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fileStream);
                    }
                    // product file info save to db
                    AddProductFile addProductFile = new AddProductFile()
                    {
                        FilePath = finalPath,
                        FileName = fileName,
                        ProductId = Convert.ToInt32(addProductFile_.ProductId),
                    };
                    response = _productRepo.ProductFileAdd(addProductFile);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("The File is not received !");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error !");
            }
        }
        #endregion


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequestSizeLimit(40000000)]
        [Route("productFileUpload_01")]
        #region file upload without any extra parameter,,, only file parameter
        public IActionResult ProductFileUpload_01()
        {
            ProductFileAddResponse response = new ProductFileAddResponse();
            try
            {
                // throw new Exception();

                var postedFile = Request.Form.Files[0];
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                if (postedFile.Length > 0)
                {
                    var fileName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + postedFile.FileName;
                    var finalPath = Path.Combine(uploadFolder, fileName);
                    using (var fileStream = new FileStream(finalPath, FileMode.Create))
                    {
                        postedFile.CopyTo(fileStream);
                    }
                    // product file info save to db
                    AddProductFile addProductFile = new AddProductFile()
                    {
                        FilePath = finalPath,
                        FileName = fileName
                    };
                    response = _productRepo.ProductFileAdd(addProductFile);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("The File is not received.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Some Error Occcured while uploading File {ex.Message}");
            }
        }
        #endregion


        [Authorize(Roles = "Shopper,Admin,Manager")]
        [HttpGet]
        [Route("allProducts")]
        public IActionResult GetAllProducts(string searchValue, string categoryId)
        {
            try
            {              
                if (searchValue == null && categoryId == null)
                {
                    var allProducts = _productRepo.GetAllProducts();
                    return Ok(allProducts);
                }
                else
                {
                    var allProducts = _productRepo.SearchProducts(searchValue, categoryId);
                    return Ok(allProducts);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ResponseCode = 500, ResponseMessage = "Server Error!" });
            }
        }


    }
}
