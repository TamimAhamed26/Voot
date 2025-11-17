using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
namespace MDUA.Facade.Interface
{
    public interface IAttributeValueFacade : ICommonFacade<AttributeValue, AttributeValueList, AttributeValueBase>
    {
        AttributeValueList GetByAttributeId(int _AttributeId);
    }
}