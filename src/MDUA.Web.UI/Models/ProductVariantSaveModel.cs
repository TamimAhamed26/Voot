using MDUA.Entities.List;
using System.ComponentModel.DataAnnotations;

namespace MDUA.Web.UI.Models
{
    public class ProductOptionDTO
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } // e.g., "Color"
        public AttributeValueList Values { get; set; } // e.g., ["Black", "Red", "Blue"]
    }
    public class ProductVariantSaveModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; }
        public string SKU { get; set; }
        public decimal VariantPrice { get; set; }
        public bool IsActive { get; set; }
        public List<int> SelectedAttributeValueIds { get; set; }

        // NEW: VariantPriceStock fields
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int StockQty { get; set; }
        public bool TrackInventory { get; set; } = true;
        public bool AllowBackorder { get; set; } = false;
        public int? WeightGrams { get; set; }
    }
}