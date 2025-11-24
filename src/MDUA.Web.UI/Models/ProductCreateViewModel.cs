using System.ComponentModel.DataAnnotations;

namespace MDUA.Web.UI.Models
{
    public class ProductEditViewModel
    {
        public int Id { get; set; } // The ID of the product to edit

        [Required]
        [Display(Name = "Product Name")]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(400)]
        public string Slug { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Base Price")]
        [DataType(DataType.Currency)]
        public decimal? BasePrice { get; set; }

        [StringLength(100)]
        public string Barcode { get; set; }

        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        [Display(Name = "Is this product variant-based?")]
        public bool IsVariantBased { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
    }
    public class ProductCreateViewModel
    {
        [Required]
        [Display(Name = "Product Name")]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(400)]
        public string Slug { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Base Price")]
        [DataType(DataType.Currency)]
        public decimal? BasePrice { get; set; }

        [StringLength(100)]
        public string Barcode { get; set; }

        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; } = 0;

        [Display(Name = "Is this product variant-based?")]
        public bool IsVariantBased { get; set; } = false;

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        public List<int> SelectedAttributeIds { get; set; }
    }
}