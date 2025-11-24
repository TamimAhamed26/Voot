using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
namespace MDUA.Facade
{
    public class AttributeNameFacade : IAttributeNameFacade
    {
        private readonly IAttributeNameDataAccess _dataAccess;
        public AttributeNameFacade(IAttributeNameDataAccess dataAccess) { _dataAccess = dataAccess; }
        public AttributeName Get(int _Id) => _dataAccess.Get(_Id);
        public long Delete(int _Id) => _dataAccess.Delete(_Id);
        public AttributeNameList GetAll() => _dataAccess.GetAll();
        public AttributeNameList GetByQuery(string query) => _dataAccess.GetByQuery(query);
        public AttributeNameList GetPaged(PagedRequest request) => _dataAccess.GetPaged(request);
        public long Insert(AttributeNameBase Object) => _dataAccess.Insert(Object);
        public long Update(AttributeNameBase Object) => _dataAccess.Update(Object);
    }
}