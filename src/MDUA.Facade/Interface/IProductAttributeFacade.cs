using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
namespace MDUA.Facade.Interface
{
    public interface IProductAttributeFacade : ICommonFacade<ProductAttribute, ProductAttributeList, ProductAttributeBase>
    {
        ProductAttributeList GetByProductId(int _ProductId);
    }
}