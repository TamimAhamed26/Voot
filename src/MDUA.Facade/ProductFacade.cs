using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;

namespace MDUA.Facade
{
    public class ProductFacade : IProductFacade
    {
        private readonly IProductDataAccess _productDataAccess;

        // Constructor for Dependency Injection
        public ProductFacade(IProductDataAccess productDataAccess)
        {
            _productDataAccess = productDataAccess;
        }

        // 
        // Custom method implementation
        //
        public ProductDetailsModel GetProductDetails(string slug)
        {
            return _productDataAccess.GetProductDetails(slug);
           // return _productDataAccess.GetByQuery(string.Format(" UrlSlug='{0}'", slug)).FirstOrDefault()

        }


        //
        // Generic ICommonFacade Implementation
        //
        public long Delete(int _Id)
        {
            return _productDataAccess.Delete(_Id);
        }

        public Product Get(int _Id)
        {
            return _productDataAccess.Get(_Id);
        }

        public ProductList GetAll()
        {
            return _productDataAccess.GetAll();
        }

        public ProductList GetByQuery(string query)
        {
            return _productDataAccess.GetByQuery(query);
        }

        public ProductList GetPaged(PagedRequest request)
        {
            return _productDataAccess.GetPaged(request);
        }

        public long Insert(ProductBase Object)
        {
            return _productDataAccess.Insert(Object);
        }

        public long Update(ProductBase Object)
        {
            return _productDataAccess.Update(Object);
        }
    }
}