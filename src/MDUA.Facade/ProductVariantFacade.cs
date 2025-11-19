using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;

namespace MDUA.Facade
{
    public class ProductVariantFacade : IProductVariantFacade
    {
        private readonly IProductVariantDataAccess _productVariantDataAccess;

        public ProductVariantFacade(IProductVariantDataAccess productVariantDataAccess)
        {
            _productVariantDataAccess = productVariantDataAccess;
        }

        public ProductVariantList GetByProductId(int _ProductId)
        {
            return _productVariantDataAccess.GetByProductId(_ProductId);
        }

        #region Standard ICommonFacade Pass-Through

        public long Delete(int _Id)
        {
            return _productVariantDataAccess.Delete(_Id);
        }

        public ProductVariant Get(int _Id)
        {
            return _productVariantDataAccess.Get(_Id);
        }

        public ProductVariantList GetAll()
        {
            return _productVariantDataAccess.GetAll();
        }

        public ProductVariantList GetByQuery(string query)
        {
            return _productVariantDataAccess.GetByQuery(query);
        }

        public ProductVariantList GetPaged(PagedRequest request)
        {
            return _productVariantDataAccess.GetPaged(request);
        }

        public long Insert(ProductVariantBase Object)
        {
            return _productVariantDataAccess.Insert(Object);
        }

        public long Update(ProductVariantBase Object)
        {
            return _productVariantDataAccess.Update(Object);
        }

        #endregion
    }
}