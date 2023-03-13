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
                // throw new Exception();

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
        public ProductFileAddResponse ProductFileAdd(AddProductFile addProductFile)
        {
            ProductFileAddResponse response = new ProductFileAddResponse();
            using var transaction = appDbContext.Database.BeginTransaction();
            try
            {
                // throw new Exception();

                // 1)
                ProductFile productFile = new ProductFile()
                {
                    ProductFileId = addProductFile.ProductFileId,
                    FileName = addProductFile.FileName,
                    FilePath = addProductFile.FilePath
                };
                var productFileSaved = appDbContext.ProductFiles.Add(productFile);
                appDbContext.SaveChanges();

                // 2)
                var product = appDbContext.Products
                                    .Where(x => x.ProductId == addProductFile.ProductId).FirstOrDefault();
                if (product != null)
                {
                    product.ProductFileId = productFileSaved.Entity.ProductFileId;
                    appDbContext.SaveChanges();
                }
                else
                {
                    throw new Exception();
                }

                // commit 1 & 2
                transaction.Commit();


                response.ResponseCode = 0;
                response.ResponseMessage = "Product File Saved Successfully !";
                response.ProductFileId = productFileSaved.Entity.ProductFileId;
                response.ProductImage = productFileSaved.Entity.FileName;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                response.ResponseCode = -1;
                response.ResponseMessage = "Product File Saved Fail !";
                response.ProductFileId = 0;
                response.ProductImage = null;
            }
            return response;
        }

        public IEnumerable<ProductDTO> GetAllProducts()
        {
            List<ProductDTO> products = new List<ProductDTO>();

            var _products = appDbContext.Products;

            if (_products != null && _products.Count() > 0)
            {
                foreach (var _product in _products)
                {
                    var _productImage = appDbContext.ProductFiles
                                        .Where(x => x.ProductFileId == _product.ProductFileId).FirstOrDefault();
                    if (_productImage != null)
                    {
                        // products with image
                        products.Add(new ProductDTO()
                        {
                            CategoryId = _product.CategoryId,
                            Price = _product.Price,
                            ProductDesc = _product.ProductDesc,
                            ProductFileId = _product.ProductFileId,
                            ProductId = _product.ProductId,
                            ProductImage = _productImage.FileName,
                            ProductName = _product.ProductName,
                            CurrentPrice = _product.DiscountPrice > 0 ? _product.DiscountPrice : _product.Price,
                            CurrentDiscountPercentage = _product.DiscountPercentage
                        });
                    }
                    else
                    {
                        // products without image
                        products.Add(new ProductDTO()
                        {
                            CategoryId = _product.CategoryId,
                            Price = _product.Price,
                            ProductDesc = _product.ProductDesc,
                            ProductId = _product.ProductId,
                            ProductImage = null,
                            ProductName = _product.ProductName,
                            CurrentPrice = _product.DiscountPrice > 0 ? _product.DiscountPrice : _product.Price,
                            CurrentDiscountPercentage = _product.DiscountPercentage
                        });
                    }
                }
            }
            return products;
        }

        public IEnumerable<ProductDTO> SearchProducts(string searchValue, string categoryId)
        {
            List<ProductDTO> products = new List<ProductDTO>();
            IQueryable<Product> _products = appDbContext.Products;
            List<Product> _productsByNameDesc = new List<Product>();
            List<Product> _productsByCategory = new List<Product>();


            if (searchValue != null)
            {
                _productsByNameDesc = _products
                          .Where(x => x.ProductName.Contains(searchValue) || x.ProductDesc.Contains(searchValue)).ToList();
            }
            if (categoryId != null)
            {
                try
                {
                    int catId = Int32.Parse(categoryId);

                    _productsByCategory = _products
                             .Where(x => x.CategoryId == catId).ToList();
                }
                catch (FormatException e)
                {
                    throw new Exception();
                }              
            }

            // add
            _products = (_productsByNameDesc.Concat(_productsByCategory)).AsQueryable<Product>();

            // remove duplicate
            _products = _products.Distinct(new DistinctProductComparer()).AsQueryable<Product>();


            if (_products != null && _products.Count() > 0)
            {
                foreach (var _product in _products)
                {
                    var _productImage = appDbContext.ProductFiles
                                        .Where(x => x.ProductFileId == _product.ProductFileId).FirstOrDefault();
                    if (_productImage != null)
                    {
                        // products with image
                        products.Add(new ProductDTO()
                        {
                            CategoryId = _product.CategoryId,
                            Price = _product.Price,
                            ProductDesc = _product.ProductDesc,
                            ProductFileId = _product.ProductFileId,
                            ProductId = _product.ProductId,
                            ProductImage = _productImage.FileName,
                            ProductName = _product.ProductName,
                            CurrentPrice = _product.DiscountPrice > 0 ? _product.DiscountPrice : _product.Price
                        });
                    }
                    else
                    {
                        // products without image
                        products.Add(new ProductDTO()
                        {
                            CategoryId = _product.CategoryId,
                            Price = _product.Price,
                            ProductDesc = _product.ProductDesc,
                            ProductId = _product.ProductId,
                            ProductImage = null,
                            ProductName = _product.ProductName,
                            CurrentPrice = _product.DiscountPrice > 0 ? _product.DiscountPrice : _product.Price
                        });
                    }
                }
            }
            return products;
        }

        public ProductDTO GetProduct(int productId)
        {
            ProductDTO product = new ProductDTO();

            var _product = appDbContext.Products
                                .Where(x => x.ProductId == productId).FirstOrDefault();
            if (_product != null)
            {
                var _productFile = appDbContext.ProductFiles
                                        .Where(x => x.ProductFileId == _product.ProductFileId).FirstOrDefault();

                // product with image
                if (_productFile != null)
                {
                    product.CategoryId = _product.CategoryId;
                    product.Price = _product.Price;
                    product.ProductDesc = _product.ProductDesc;
                    product.ProductFileId = _product.ProductFileId;
                    product.ProductId = _product.ProductId;
                    product.ProductImage = _productFile.FileName;
                    product.ProductName = _product.ProductName;
                }
                // product without image
                else
                {
                    product.CategoryId = _product.CategoryId;
                    product.Price = _product.Price;
                    product.ProductDesc = _product.ProductDesc;
                    product.ProductId = _product.ProductId;
                    product.ProductImage = null;
                    product.ProductName = _product.ProductName;
                }
            }
            return product;
        }
    }
}
