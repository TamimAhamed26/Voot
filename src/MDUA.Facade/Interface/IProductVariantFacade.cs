using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;

namespace MDUA.Facade.Interface
{
    public interface IProductVariantFacade : ICommonFacade<ProductVariant, ProductVariantList, ProductVariantBase>
    {
        ProductVariantList GetByProductId(int _ProductId);
    }
}