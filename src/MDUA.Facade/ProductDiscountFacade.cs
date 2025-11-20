using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using MDUA.Framework.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MDUA.Facade
{
    public class ProductDiscountFacade : IProductDiscountFacade
    {
        private readonly IProductDiscountDataAccess _productDiscountDataAccess;
        private readonly IProductDataAccess _productDataAccess;
        private readonly IProductVariantDataAccess _variantDataAccess;
        private readonly IVariantPriceStockDataAccess _priceStockDataAccess;

        public ProductDiscountFacade(
            IProductDiscountDataAccess productDiscountDataAccess,
            IProductDataAccess productDataAccess,
            IProductVariantDataAccess variantDataAccess,
            IVariantPriceStockDataAccess priceStockDataAccess)
        {
            _productDiscountDataAccess = productDiscountDataAccess;
            _productDataAccess = productDataAccess;
            _variantDataAccess = variantDataAccess;
            _priceStockDataAccess = priceStockDataAccess;
        }

        public ProductDiscountList GetByProductId(int productId)
        {
            return _productDiscountDataAccess.GetByProductId(productId);
        }

        public void ApplyDiscount(ProductDiscountBase discount, string user)
        {
            // 1. Insert new discount
            discount.CreatedBy = user;
            discount.CreatedAt = DateTime.UtcNow;
            _productDiscountDataAccess.Insert(discount);

            // 2. Recalculate
            RecalculatePrices(discount.ProductId, user);
        }

        public void DeleteDiscount(int discountId, string user)
        {
            var discount = _productDiscountDataAccess.Get(discountId);
            if (discount != null)
            {
                _productDiscountDataAccess.Delete(discountId);
                // 3. Recalculate (This restores original price if no other discounts exist)
                RecalculatePrices(discount.ProductId, user);
            }
        }

        private void RecalculatePrices(int productId, string user)
        {
            // FIXED: Use injected dependencies instead of creating new instances
            // 1. Fetch ALL Active Discounts
            var allDiscounts = _productDiscountDataAccess.GetByProductId(productId);
            var now = DateTime.UtcNow;

            var activeDiscounts = allDiscounts.Where(d =>
                d.IsActive &&
                d.EffectiveFrom <= now &&
                (d.EffectiveTo == null || d.EffectiveTo >= now)
            ).ToList();

            var product = _productDataAccess.Get(productId);
            if (product == null) return;

            // 2. Handle Variants (We CAN store Original/Selling separately here)
            if (product.IsVariantBased == true)
            {
                var variants = _variantDataAccess.GetByProductId(productId);
                foreach (var variant in variants)
                {
                    var priceStock = _priceStockDataAccess.Get(variant.Id);

                    // A. Determine Original Price
                    // If CompareAt exists and is higher, IT is the original. Otherwise current Price is original.
                    decimal originalPrice = 0;
                    if (priceStock != null)
                    {
                        originalPrice = (priceStock.CompareAtPrice > 0 && priceStock.CompareAtPrice > priceStock.Price)
                                        ? priceStock.CompareAtPrice.Value
                                        : priceStock.Price;
                    }
                    else
                    {
                        originalPrice = variant.VariantPrice ?? 0;
                    }

                    // B. Find Best Discount
                    decimal bestPrice = originalPrice;
                    if (activeDiscounts.Any())
                    {
                        foreach (var d in activeDiscounts)
                        {
                            decimal calc = CalculateNewPrice(originalPrice, d.DiscountType, d.DiscountValue);
                            if (calc < bestPrice) bestPrice = calc;
                        }
                    }

                    // C. Update Stock Record
                    if (priceStock == null)
                    {
                        priceStock = new VariantPriceStock
                        {
                            Id = variant.Id,
                            StockQty = 0,
                            TrackInventory = true,
                            Price = originalPrice
                        };
                    }

                    if (bestPrice < originalPrice)
                    {
                        // Apply Discount
                        priceStock.Price = bestPrice;
                        priceStock.CompareAtPrice = originalPrice;
                    }
                    else
                    {
                        // RESTORE ORIGINAL (No discount active)
                        priceStock.Price = originalPrice;
                        priceStock.CompareAtPrice = null;
                    }

                    // Update or Insert
                    if (priceStock.Id > 0 && _priceStockDataAccess.Get(priceStock.Id) != null)
                    {
                        _priceStockDataAccess.Update(priceStock);
                    }
                    else
                    {
                        _priceStockDataAccess.Insert(priceStock);
                    }

                    // D. Update Variant Display Price
                    variant.VariantPrice = priceStock.Price;
                    variant.UpdatedBy = user;
                    variant.UpdatedAt = DateTime.UtcNow;
                    _variantDataAccess.Update(variant);
                }
            }

            // 3. Simple Product
            // We DO NOT update Product.BasePrice in DB to the lower price, 
            // otherwise we lose the original price forever.
            // The UI will handle the calculation via DTO.
        }

        private decimal CalculateNewPrice(decimal original, string type, decimal value)
        {
            decimal result = original;
            if (type.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                result = original - (original * (value / 100));
            else
                result = original - value;

            return result < 0 ? 0 : result;
        }

        // --- Pass Through ---
        public long Insert(ProductDiscountBase obj) => _productDiscountDataAccess.Insert(obj);
        public long Update(ProductDiscountBase obj) => _productDiscountDataAccess.Update(obj);
        public long Delete(int id) => _productDiscountDataAccess.Delete(id);
        public ProductDiscount Get(int id) => _productDiscountDataAccess.Get(id);
        public ProductDiscountList GetAll() => _productDiscountDataAccess.GetAll();
        public ProductDiscountList GetByQuery(string query) => _productDiscountDataAccess.GetByQuery(query);
        public ProductDiscountList GetPaged(PagedRequest request) => _productDiscountDataAccess.GetPaged(request);
    }
}