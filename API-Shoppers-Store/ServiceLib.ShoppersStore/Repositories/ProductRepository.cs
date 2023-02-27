using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceLib.ShoppersStore.Interfaces;
using EF.Core.ShoppersStore.ShoppersStoreDB;
using EF.Core.ShoppersStore.ShoppersStoreDB.Models;
using ServiceLib.ShoppersStore.DTO;


namespace ServiceLib.ShoppersStore.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShoppersStoreContext appDbContext;
        public ProductRepository(ShoppersStoreContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IEnumerable<Category> GetCategories()
        {
            return appDbContext.Categories.ToList();
        }


        public Product AddProduct(Product product)
        {
            try
            {
                var result = appDbContext.Products.Add(product);
                appDbContext.SaveChanges();
                return result.Entity;
            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }

        // product image info save to db
        public ProductFileAddResponse ProductFileAdd(ProductFile productFile)
        {
            ProductFileAddResponse response = new ProductFileAddResponse();
            try
            {
                // throw new Exception();

                var productFileSaved = appDbContext.ProductFiles.Add(productFile);
                appDbContext.SaveChanges();
                response.ResponseCode = 0;
                response.ResponseMessage = "Product File Saved Successfully !";
                response.ProductFileId = productFileSaved.Entity.ProductFileId;
                response.ProductImage = productFileSaved.Entity.FileName;
            }
            catch (Exception ex)
            {
                response.ResponseCode = -1;
                response.ResponseMessage = "Product File Saved Fail !";
                response.ProductFileId = 0;
                response.ProductImage = null;
            }
            return response;
        }
    }
}
