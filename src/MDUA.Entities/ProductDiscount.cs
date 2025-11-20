using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MDUA.Entities
{
	public partial class ProductDiscount
    {
        public bool IsValid()
        {
            var now = DateTime.UtcNow;
            return IsActive
                   && EffectiveFrom <= now
                   && (EffectiveTo == null || EffectiveTo >= now);
        }
        public class ProductListViewModel
        {
            public int Id { get; set; }
            public string ProductName { get; set; }
            public string Slug { get; set; }
            public bool IsVariantBased { get; set; }
            public bool IsActive { get; set; }

            // Pricing
            public decimal OriginalPrice { get; set; }
            public decimal SellingPrice { get; set; }
            public bool HasDiscount { get; set; }
        }
        public class ProductDiscountViewModel
        {
            [Required]
            public int ProductId { get; set; }

            [Required]
            public string DiscountType { get; set; }

            [Required]
            [Range(0.01, 100000)]
            public decimal DiscountValue { get; set; }

            [Range(1, int.MaxValue)]
            public int MinQuantity { get; set; } = 1;

            [Required]
            public DateTime EffectiveFrom { get; set; }

            public DateTime? EffectiveTo { get; set; }

            public bool IsValid()
            {
                if (EffectiveTo.HasValue && EffectiveTo.Value < EffectiveFrom)
                    return false;
                if (DiscountType == "Percentage" && DiscountValue > 100)
                    return false;
                return true;
            }
        }
    }
}
