using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class ProductFacade : IProductFacade
    {
        private readonly IProductDataAccess _productDataAccess;
        private readonly IProductDiscountFacade _productDiscountFacade;
        private readonly IProductImageDataAccess _productImageDataAccess;

        public ProductFacade(
            IProductDataAccess productDataAccess,
            IProductDiscountFacade productDiscountFacade,
            IProductImageDataAccess productImageDataAccess
        )
        {
            _productDataAccess = productDataAccess;
            _productDiscountFacade = productDiscountFacade;
            _productImageDataAccess = productImageDataAccess;
        }

        // =============================================================
        // GET PRODUCT + DISCOUNT LOGIC
        // =============================================================
        public Product GetProductWithPrice(int productId)
        {
            var product = _productDataAccess.Get(productId);
            if (product == null) return null;

            decimal basePrice = product.BasePrice ?? 0;
            var bestDiscount = _productDiscountFacade.GetBestDiscount(productId, basePrice);

            decimal sellingPrice = basePrice;

            if (bestDiscount != null)
            {
                sellingPrice = _productDiscountFacade.CalculateNewPrice(
                    basePrice,
                    bestDiscount.DiscountType,
                    bestDiscount.DiscountValue
                );
            }

            product.SellingPrice = sellingPrice;
            product.ActiveDiscount = bestDiscount;

            return product;
        }

        // Helper for variants (controller-friendly)
        public ProductDiscount GetBestDiscountForProduct(int productId, decimal basePrice)
        {
            return _productDiscountFacade.GetBestDiscount(productId, basePrice);
        }

        // =============================================================
        // STANDARD CRUD + PAGINATION SECTION
        // =============================================================
        public ProductDetailsModel GetProductDetails(string slug)
        {
            return _productDataAccess.GetProductDetails(slug);
        }

        public Product Get(int id)
        {
            return _productDataAccess.Get(id);
        }

        public ProductList GetAll()
        {
            return _productDataAccess.GetAll();
        }

        public ProductList GetByQuery(string query)
        {
            return _productDataAccess.GetByQuery(query);
        }

        public ProductList GetPaged(PagedRequest request)
        {
            return _productDataAccess.GetPaged(request);
        }

        public long Insert(ProductBase entity)
        {
            return _productDataAccess.Insert(entity);
        }

        public long Update(ProductBase entity)
        {
            return _productDataAccess.Update(entity);
        }

        public long Delete(int id)
        {
            return _productDataAccess.Delete(id);
        }

        // =============================================================
        // IMAGE MANAGEMENT
        // =============================================================
        public long AddImage(ProductImage image)
        {
            return _productImageDataAccess.Insert(image);
        }

        public long DeleteImage(int imageId)
        {
            return _productImageDataAccess.Delete(imageId);
        }
        public ProductImage GetImage(int imageId)
        {
            return _productImageDataAccess.Get(imageId);
        }

        public long UpdateImage(ProductImage image)
        {
            return _productImageDataAccess.Update(image);
        }
        public List<ProductImage> GetImages(int productId)
        {
            // Returns ProductImageList which inherits from List<ProductImage>
            return _productImageDataAccess.GetByProductId(productId);
        }
    }
}