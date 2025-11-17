using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Facade.Interface
{
    public interface IProductFacade : ICommonFacade<Product, ProductList, ProductBase>
    {
        ProductDetailsModel GetProductDetails(string slug);
    }
}