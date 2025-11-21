using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Facade.Interface
{
    public interface IProductFacade : ICommonFacade<Product, ProductList, ProductBase>
    {
        Product GetProductWithPrice(int productId);
        ProductDiscount GetBestDiscountForProduct(int productId, decimal basePrice);

        ProductDetailsModel GetProductDetails(string slug);
    }
}