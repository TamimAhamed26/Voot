using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
namespace MDUA.Facade
{
    public class ProductAttributeFacade : IProductAttributeFacade
    {
        private readonly IProductAttributeDataAccess _dataAccess;
        public ProductAttributeFacade(IProductAttributeDataAccess dataAccess) { _dataAccess = dataAccess; }
        public ProductAttributeList GetByProductId(int _ProductId) => _dataAccess.GetByProductId(_ProductId);
        public ProductAttribute Get(int _Id) => _dataAccess.Get(_Id);
        public long Delete(int _Id) => _dataAccess.Delete(_Id);
        public ProductAttributeList GetAll() => _dataAccess.GetAll();
        public ProductAttributeList GetByQuery(string query) => _dataAccess.GetByQuery(query);
        public ProductAttributeList GetPaged(PagedRequest request) => _dataAccess.GetPaged(request);
        public long Insert(ProductAttributeBase Object) => _dataAccess.Insert(Object);
        public long Update(ProductAttributeBase Object) => _dataAccess.Update(Object);
    }
}