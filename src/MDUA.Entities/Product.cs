using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class Product
    {  // Not mapped to DB – runtime only
        public decimal? SellingPrice { get; set; }
        public ProductDiscount? ActiveDiscount { get; set; }

    }
}
