using System;
using System.Collections.Generic;
using System.Text;
using EF.Core.ShoppersStore.ShoppersStoreDB.Models;
using ServiceLib.ShoppersStore.DTO;

namespace ServiceLib.ShoppersStore.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Category> GetCategories();
        Product AddProduct(Product product);        
        ProductFileAddResponse ProductFileAdd(ProductFile productFile);
        /*
        IEnumerable<ProductDTO> GetAllProducts();
        ProductDTO GetProduct(int productId);
        ProductDTO EditProduct(ProductDTO product);
        ProductFileEditResponse ProductFileEdit(ProductFileEditResponse _productFile);        
        IEnumerable<ProductDTO> SearchProducts(string searchValue, string categoryId);
        ProductDiscountDTO SetProductDiscount(ProductDiscountDTO discount);
        */
    }
}
