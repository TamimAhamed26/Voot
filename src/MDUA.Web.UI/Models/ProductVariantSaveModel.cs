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

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(150)]
        public string VariantName { get; set; }

        [StringLength(50)]
        public string SKU { get; set; }

        public decimal? VariantPrice { get; set; }
        public List<int> SelectedAttributeValueIds { get; set; }
        public bool IsActive { get; set; }
    }
}