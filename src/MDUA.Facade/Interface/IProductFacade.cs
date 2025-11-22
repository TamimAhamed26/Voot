using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Facade.Interface
{
    public interface IProductFacade : ICommonFacade<Product, ProductList, ProductBase>
    {
        Product GetProductWithPrice(int productId);
        ProductDiscount GetBestDiscountForProduct(int productId, decimal basePrice);
        long AddImage(ProductImage image);
        long DeleteImage(int imageId);
        List<ProductImage> GetImages(int productId);
        ProductDetailsModel GetProductDetails(string slug);
    }
}