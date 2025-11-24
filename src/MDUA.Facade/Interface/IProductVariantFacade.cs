using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using System.Collections.Generic;

namespace MDUA.Facade.Interface
{
    public interface IProductVariantFacade : ICommonFacade<ProductVariant, ProductVariantList, ProductVariantBase>
    {
        ProductVariantList GetByProductId(int _ProductId);
        VariantImage GetImage(int imageId);
        long UpdateImage(VariantImage image);
        long AddImage(VariantImage image);
        long DeleteImage(int imageId);
        List<VariantImage> GetImages(int variantId);
        ProductVariantViewModel CreateVariantWithAttributes(ProductVariantSaveModl model, string user);
        ProductVariantViewModel UpdateVariantWithAttributes(ProductVariantSaveModl model, string user);

    }
}