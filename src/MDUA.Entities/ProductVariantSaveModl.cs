using System.Collections.Generic;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    public class ProductOptionDTO
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } // e.g., "Color"
        public AttributeValueList Values { get; set; } // e.g., ["Black", "Red", "Blue"]
    }
    public class ProductVariantViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; }
        public string SKU { get; set; }
        public decimal VariantPrice { get; set; }
        public bool IsActive { get; set; }

        // Flattened fields from VariantPriceStock
        public int StockQty { get; set; }
        public bool TrackInventory { get; set; }
        public bool AllowBackorder { get; set; }
        public int WeightGrams { get; set; }
    }
    public class ProductVariantSaveModl
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; }
        public string SKU { get; set; }
        public decimal VariantPrice { get; set; }
        public bool IsActive { get; set; }
        public List<int> SelectedAttributeValueIds { get; set; }

        // VariantPriceStock fields
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int StockQty { get; set; }
        public bool TrackInventory { get; set; } = true;
        public bool AllowBackorder { get; set; } = false;
        public int? WeightGrams { get; set; }
    }
   
}