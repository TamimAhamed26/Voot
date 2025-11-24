using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using MDUA.Framework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDUA.Facade
{
    public class ProductDiscountFacade : IProductDiscountFacade
    {
        private readonly IProductDiscountDataAccess _productDiscountDataAccess;

        public ProductDiscountFacade(IProductDiscountDataAccess productDiscountDataAccess)
        {
            _productDiscountDataAccess = productDiscountDataAccess;
        }

        

        /// <summary>
        /// Finds the single best discount for a product based on which one gives the lowest price.
        /// </summary>
        public ProductDiscount GetBestDiscount(int productId, decimal basePrice)
        {
            var allDiscounts = _productDiscountDataAccess.GetByProductId(productId);

            var now = DateTime.Now; 
            var validDiscounts = allDiscounts
                .Where(d => d.IsActive
                         && d.EffectiveFrom <= now
                         && (d.EffectiveTo == null || d.EffectiveTo >= now))
                .ToList();

            if (!validDiscounts.Any()) return null; 

            ProductDiscount bestDiscount = null;
            decimal lowestPriceFound = basePrice;

            foreach (var d in validDiscounts)
            {
                decimal calculatedPrice = CalculateNewPrice(basePrice, d.DiscountType, d.DiscountValue);

                if (calculatedPrice < lowestPriceFound)
                {
                    lowestPriceFound = calculatedPrice;
                    bestDiscount = d;
                }
            }

            return bestDiscount; 
        }

        /// <summary>
        /// Math helper to calculate Flat vs Percentage logic.
        /// </summary>
        public decimal CalculateNewPrice(decimal original, string type, decimal value)
        {
            if (string.Equals(type, "Percentage", StringComparison.OrdinalIgnoreCase))
            {
                return original * (1 - value / 100m);
            }

            if (string.Equals(type, "Flat", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(type, "Fixed", StringComparison.OrdinalIgnoreCase)) // ADD THIS
            {
                return Math.Max(0, original - value);
            }

            return original; // fallback
        }
        // ==========================================================================
        // CRUD Operations
        // ==========================================================================

        public ProductDiscountList GetByProductId(int productId)
        {
            return _productDiscountDataAccess.GetByProductId(productId);
        }

        public void ApplyDiscount(ProductDiscountBase discount, string user)
        {
           
            discount.CreatedBy = user;
            discount.CreatedAt = DateTime.Now; 
            _productDiscountDataAccess.Insert(discount);
        }

        public void DeleteDiscount(int discountId, string user)
        {
            _productDiscountDataAccess.Delete(discountId);
        }

        public long Insert(ProductDiscountBase obj) => _productDiscountDataAccess.Insert(obj);
        public long Update(ProductDiscountBase obj) => _productDiscountDataAccess.Update(obj);
        public long Delete(int id) => _productDiscountDataAccess.Delete(id);
        public ProductDiscount Get(int id) => _productDiscountDataAccess.Get(id);
        public ProductDiscountList GetAll() => _productDiscountDataAccess.GetAll();
        public ProductDiscountList GetByQuery(string query) => _productDiscountDataAccess.GetByQuery(query);
        public ProductDiscountList GetPaged(PagedRequest request) => _productDiscountDataAccess.GetPaged(request);
    }
}