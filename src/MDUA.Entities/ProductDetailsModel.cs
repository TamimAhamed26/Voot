using MDUA.Entities.List;
using System.Collections.Generic;

namespace MDUA.Entities
{
    // This is the "ViewModel" for your product page.
    public class ProductDetailsModel
    {
        public Product? Product { get; set; }
        public ProductImageList ProductImages { get; set; } = new ProductImageList();
        public ProductVariantList Variants { get; set; } = new ProductVariantList();

        // This dictionary will hold Price/Stock info, keyed by Variant.Id
        public Dictionary<int, VariantPriceStock> VariantPriceStocks { get; set; } = new Dictionary<int, VariantPriceStock>();

        // This dictionary will hold images, keyed by Variant.Id
        public Dictionary<int, VariantImageList> VariantImages { get; set; } = new Dictionary<int, VariantImageList>();

        // Holds the *options* for the product, e.g., "Color", "Size"
        public List<ProductAttributeOptionModel> Options { get; set; } = new List<ProductAttributeOptionModel>();

        // Holds the mappings for the UI to use
        public VariantAttributeValueList VariantAttributeValues { get; set; } = new VariantAttributeValueList();
    }

    // A helper model to group attributes and their values
    public class ProductAttributeOptionModel
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;

        // A list of values for this attribute, e.g., "Silver", "Black"
        public AttributeValueList Values { get; set; } = new AttributeValueList();
    }
}