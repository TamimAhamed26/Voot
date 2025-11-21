using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;

namespace MDUA.Facade
{
    public class ProductFacade : IProductFacade
    {
        private readonly IProductDataAccess _productDataAccess;
        private readonly IProductDiscountFacade _productDiscountFacade;

        public ProductFacade(
            IProductDataAccess productDataAccess,
            IProductDiscountFacade productDiscountFacade
        )
        {
            _productDataAccess = productDataAccess;
            _productDiscountFacade = productDiscountFacade;
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
    }
}
