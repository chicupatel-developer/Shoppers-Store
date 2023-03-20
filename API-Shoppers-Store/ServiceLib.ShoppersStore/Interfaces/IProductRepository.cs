﻿using System;
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
        ProductFileAddResponse ProductFileAdd(AddProductFile addProductFile);        
        IEnumerable<ProductDTO> GetAllProducts();
        IEnumerable<ProductDTO> SearchProducts(string searchValue, string categoryId);        
        ProductDTO GetProduct(int productId);        
        ProductDTO EditProduct(ProductDTO product);        
        ProductFileEditResponse ProductFileEdit(ProductFileEditResponse _productFile);        
        ProductDiscountDTO SetProductDiscount(ProductDiscountDTO discount);
        bool ResetProductDiscount(int productId);
    }
}
