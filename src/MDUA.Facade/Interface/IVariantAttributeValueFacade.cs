using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
namespace MDUA.Facade.Interface
{
    public interface IVariantAttributeValueFacade : ICommonFacade<VariantAttributeValue, VariantAttributeValueList, VariantAttributeValueBase> { VariantAttributeValueList GetByVariantId(int _VariantId); }

}