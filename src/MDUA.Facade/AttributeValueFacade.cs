using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
namespace MDUA.Facade
{
    public class AttributeValueFacade : IAttributeValueFacade
    {
        private readonly IAttributeValueDataAccess _dataAccess;
        public AttributeValueFacade(IAttributeValueDataAccess dataAccess) { _dataAccess = dataAccess; }
        public AttributeValueList GetByAttributeId(int _AttributeId) => _dataAccess.GetByAttributeId(_AttributeId);
        public AttributeValue Get(int _Id) => _dataAccess.Get(_Id);
        public long Delete(int _Id) => _dataAccess.Delete(_Id);
        public AttributeValueList GetAll() => _dataAccess.GetAll();
        public AttributeValueList GetByQuery(string query) => _dataAccess.GetByQuery(query);
        public AttributeValueList GetPaged(PagedRequest request) => _dataAccess.GetPaged(request);
        public long Insert(AttributeValueBase Object) => _dataAccess.Insert(Object);
        public long Update(AttributeValueBase Object) => _dataAccess.Update(Object);
    }
}