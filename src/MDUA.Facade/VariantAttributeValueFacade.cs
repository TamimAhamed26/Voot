using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
namespace MDUA.Facade
{
    public class VariantAttributeValueFacade : IVariantAttributeValueFacade
    {
        private readonly IVariantAttributeValueDataAccess _dataAccess;
        public VariantAttributeValueFacade(IVariantAttributeValueDataAccess dataAccess) { _dataAccess = dataAccess; }
        public VariantAttributeValue Get(int _Id) => _dataAccess.Get(_Id);
        public VariantAttributeValueList GetByVariantId(int _VariantId)
        {
            return _dataAccess.GetByVariantId(_VariantId);
        }
        public long Delete(int _Id) => _dataAccess.Delete(_Id);
        public VariantAttributeValueList GetAll() => _dataAccess.GetAll();
        public VariantAttributeValueList GetByQuery(string query) => _dataAccess.GetByQuery(query);
        public VariantAttributeValueList GetPaged(PagedRequest request) => _dataAccess.GetPaged(request);
        public long Insert(VariantAttributeValueBase Object) => _dataAccess.Insert(Object);
        public long Update(VariantAttributeValueBase Object) => _dataAccess.Update(Object);
    }
}