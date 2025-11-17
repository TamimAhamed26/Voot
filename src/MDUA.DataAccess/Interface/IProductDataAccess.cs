using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    // 1. Inherit from the generic ICommonDataAccess
    // 2. Add your custom method signature for GetProductDetails
    public interface IProductDataAccess : ICommonDataAccess<Product, ProductList, ProductBase>
    {
        ProductDetailsModel GetProductDetails(string slug);
    }

}