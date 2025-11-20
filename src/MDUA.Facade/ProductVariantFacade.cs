using MDUA.DataAccess; // Needed for concrete DAOs inside transaction
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using MDUA.Framework.DataAccess; // Needed for BaseDataAccess transactions
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MDUA.Facade
{
    public class ProductVariantFacade : IProductVariantFacade
    {
        private readonly IProductVariantDataAccess _productVariantDataAccess;
        private readonly IVariantAttributeValueDataAccess _variantAttributeValueDataAccess;

        public ProductVariantFacade(
            IProductVariantDataAccess productVariantDataAccess,
            IVariantAttributeValueDataAccess variantAttributeValueDataAccess)
        {
            _productVariantDataAccess = productVariantDataAccess;
            _variantAttributeValueDataAccess = variantAttributeValueDataAccess;
        }

        public ProductVariantList GetByProductId(int _ProductId)
        {
            return _productVariantDataAccess.GetByProductId(_ProductId);
        }

        #region Business Logic (Transactions)

        /// <summary>
        /// Creates a Variant, its Price/Stock info, and Attribute Links in a single Transaction.
        /// </summary>
        public ProductVariantViewModel CreateVariantWithAttributes(ProductVariantSaveModl model, string user)
        {
            if (CheckVariantExists(model.ProductId, model.SelectedAttributeValueIds))
                throw new Exception("A variant with this combination of attributes already exists.");

            using (SqlTransaction transaction = BaseDataAccess.BeginTransaction())
            {
                try
                {
                    var variantDao = new ProductVariantDataAccess(transaction);
                    var priceStockDao = new VariantPriceStockDataAccess(transaction);
                    var attrLinkDao = new VariantAttributeValueDataAccess(transaction);
                    var attrValueDao = new AttributeValueDataAccess(transaction);

                    var variant = new ProductVariant
                    {
                        ProductId = model.ProductId,
                        VariantName = model.VariantName,
                        SKU = model.SKU,
                        VariantPrice = model.VariantPrice,
                        IsActive = model.IsActive,
                        CreatedBy = user,
                        CreatedAt = DateTime.UtcNow
                    };
                    variantDao.Insert(variant); // variant.Id is populated

                    var priceStock = new VariantPriceStock
                    {
                        Id = variant.Id,
                        Price = model.Price,
                        CompareAtPrice = model.CompareAtPrice,
                        CostPrice = model.CostPrice,
                        StockQty = model.StockQty,
                        TrackInventory = model.TrackInventory,
                        AllowBackorder = model.AllowBackorder,
                        WeightGrams = model.WeightGrams
                    };
                    priceStockDao.Insert(priceStock);

                    if (model.SelectedAttributeValueIds != null)
                    {
                        int displayOrder = 0;
                        foreach (var valueId in model.SelectedAttributeValueIds)
                        {
                            var attrValue = attrValueDao.Get(valueId);
                            if (attrValue == null) throw new Exception($"Invalid Attribute ID: {valueId}");

                            attrLinkDao.Insert(new VariantAttributeValue
                            {
                                VariantId = variant.Id,
                                AttributeId = attrValue.AttributeId,
                                AttributeValueId = valueId,
                                DisplayOrder = displayOrder++
                            });
                        }
                    }

                    BaseDataAccess.CloseTransaction(true, transaction);

                    // Construct Return Object
                    return new ProductVariantViewModel
                    {
                        Id = variant.Id,
                        ProductId = variant.ProductId,
                        VariantName = variant.VariantName,
                        SKU = variant.SKU,
                        VariantPrice = (decimal)variant.VariantPrice,
                        IsActive = variant.IsActive,
                        StockQty = priceStock.StockQty,
                        TrackInventory = priceStock.TrackInventory,
                        AllowBackorder = priceStock.AllowBackorder,
                        WeightGrams = priceStock.WeightGrams ?? 0
                    };
                }
                catch
                {
                    BaseDataAccess.CloseTransaction(false, transaction);
                    throw;
                }
            }
        }

        public ProductVariantViewModel UpdateVariantWithAttributes(ProductVariantSaveModl model, string user)
        {
            var oldLinks = _variantAttributeValueDataAccess.GetByVariantId(model.Id);
            var oldIds = oldLinks.Select(x => x.AttributeValueId).OrderBy(x => x).ToList();
            var newIds = model.SelectedAttributeValueIds.OrderBy(x => x).ToList();

            bool attributesChanged = !oldIds.SequenceEqual(newIds);

            if (attributesChanged)
            {
                if (CheckVariantExists(model.ProductId, model.SelectedAttributeValueIds, model.Id))
                    throw new Exception("A variant with this combination of attributes already exists.");
            }

            using (SqlTransaction transaction = BaseDataAccess.BeginTransaction())
            {
                try
                {
                    var variantDao = new ProductVariantDataAccess(transaction);
                    var priceStockDao = new VariantPriceStockDataAccess(transaction);
                    var attrLinkDao = new VariantAttributeValueDataAccess(transaction);
                    var attrValueDao = new AttributeValueDataAccess(transaction);

                    // 1. Update Variant
                    var variant = variantDao.Get(model.Id);
                    if (variant == null) throw new Exception("Variant not found.");

                    variant.VariantName = model.VariantName;
                    variant.SKU = model.SKU;
                    variant.VariantPrice = model.VariantPrice;
                    variant.IsActive = model.IsActive;
                    variant.UpdatedBy = user;
                    variant.UpdatedAt = DateTime.UtcNow;
                    variantDao.Update(variant);

                    // 2. Update PriceStock
                    var priceStock = new VariantPriceStock
                    {
                        Id = model.Id,
                        Price = model.Price,
                        CompareAtPrice = model.CompareAtPrice,
                        CostPrice = model.CostPrice,
                        StockQty = model.StockQty,
                        TrackInventory = model.TrackInventory,
                        AllowBackorder = model.AllowBackorder,
                        WeightGrams = model.WeightGrams
                    };

                    // Check existence inside transaction to determine update vs insert
                    var existingStock = priceStockDao.Get(model.Id);
                    if (existingStock != null)
                        priceStockDao.Update(priceStock);
                    else
                        priceStockDao.Insert(priceStock);

                    // 3. Update Attributes
                    if (attributesChanged)
                    {
                        foreach (var link in oldLinks) attrLinkDao.Delete(link.Id);

                        int displayOrder = 0;
                        foreach (var valueId in model.SelectedAttributeValueIds)
                        {
                            var attrValue = attrValueDao.Get(valueId);
                            if (attrValue == null) throw new Exception($"Invalid Attribute ID: {valueId}");

                            attrLinkDao.Insert(new VariantAttributeValue
                            {
                                VariantId = model.Id,
                                AttributeId = attrValue.AttributeId,
                                AttributeValueId = valueId,
                                DisplayOrder = displayOrder++
                            });
                        }
                    }

                    BaseDataAccess.CloseTransaction(true, transaction);

                    // Construct Return Object matching the JS requirements
                    return new ProductVariantViewModel
                    {
                        Id = variant.Id,
                        ProductId = variant.ProductId,
                        VariantName = variant.VariantName,
                        SKU = variant.SKU,
                        VariantPrice = (decimal)variant.VariantPrice,
                        IsActive = variant.IsActive,
                        StockQty = priceStock.StockQty,
                        TrackInventory = priceStock.TrackInventory,
                        AllowBackorder = priceStock.AllowBackorder,
                        WeightGrams = priceStock.WeightGrams ?? 0
                    };
                }
                catch
                {
                    BaseDataAccess.CloseTransaction(false, transaction);
                    throw;
                }
            }
        }

        /// <summary>
        /// Checks if a specific combination of attributes already exists for a product.
        /// </summary>
        private bool CheckVariantExists(int productId, List<int> attributeValueIds, int? excludeVariantId = null)
        {
            // This is a read-only check, so we use the standard injected DAO
            var variants = _productVariantDataAccess.GetByProductId(productId);

            foreach (var v in variants)
            {
                if (excludeVariantId.HasValue && v.Id == excludeVariantId.Value) continue;

                var attrs = _variantAttributeValueDataAccess.GetByVariantId(v.Id);
                var existingIds = attrs.Select(a => a.AttributeValueId).OrderBy(x => x).ToList();
                var newIds = attributeValueIds.OrderBy(x => x).ToList();

                if (existingIds.SequenceEqual(newIds)) return true;
            }
            return false;
        }

        #endregion

        #region Standard ICommonFacade Pass-Through (With Transactional Delete)

        public long Delete(int _Id)
        {
            // Wrap Delete in transaction to clean up children manually if Cascade Delete is not set in DB
            using (SqlTransaction transaction = BaseDataAccess.BeginTransaction())
            {
                try
                {
                    // 1. Setup DAOs
                    var variantDao = new ProductVariantDataAccess(transaction);
                    var priceDao = new VariantPriceStockDataAccess(transaction);
                    var attrLinkDao = new VariantAttributeValueDataAccess(transaction);

                    // 2. Delete Children (Attributes)
                    // Optimization: In real world, you'd add DeleteByVariantId to DAO. 
                    // Here we iterate (less efficient but safe without changing DAO interface).
                    var links = _variantAttributeValueDataAccess.GetByVariantId(_Id);
                    foreach (var link in links) attrLinkDao.Delete(link.Id);

                    // 3. Delete Price/Stock
                    priceDao.Delete(_Id);

                    // 4. Delete Variant
                    long result = variantDao.Delete(_Id);

                    BaseDataAccess.CloseTransaction(true, transaction);
                    return result;
                }
                catch
                {
                    BaseDataAccess.CloseTransaction(false, transaction);
                    throw;
                }
            }
        }

        public ProductVariant Get(int _Id) => _productVariantDataAccess.Get(_Id);
        public ProductVariantList GetAll() => _productVariantDataAccess.GetAll();
        public ProductVariantList GetByQuery(string query) => _productVariantDataAccess.GetByQuery(query);
        public ProductVariantList GetPaged(PagedRequest request) => _productVariantDataAccess.GetPaged(request);
        public long Insert(ProductVariantBase Object) => _productVariantDataAccess.Insert(Object);
        public long Update(ProductVariantBase Object) => _productVariantDataAccess.Update(Object);

        #endregion
    }
}