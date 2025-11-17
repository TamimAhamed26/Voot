using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;

namespace MDUA.Facade
{
    public class ProductCategoryFacade : IProductCategoryFacade
    {
        private readonly IProductCategoryDataAccess _productCategoryDataAccess;

        public ProductCategoryFacade(IProductCategoryDataAccess productCategoryDataAccess)
        {
            _productCategoryDataAccess = productCategoryDataAccess;
        }

        // --- Pass-through implementations ---

        public long Delete(int _Id)
        {
            return _productCategoryDataAccess.Delete(_Id);
        }

        public ProductCategory Get(int _Id)
        {
            return _productCategoryDataAccess.Get(_Id);
        }

        public ProductCategoryList GetAll()
        {
            return _productCategoryDataAccess.GetAll();
        }

        public ProductCategoryList GetByQuery(string query)
        {
            return _productCategoryDataAccess.GetByQuery(query);
        }

        public ProductCategoryList GetPaged(PagedRequest request)
        {
            return _productCategoryDataAccess.GetPaged(request);
        }

        public long Insert(ProductCategoryBase Object)
        {
            return _productCategoryDataAccess.Insert(Object);
        }

        public long Update(ProductCategoryBase Object)
        {
            return _productCategoryDataAccess.Update(Object);
        }
    }
}