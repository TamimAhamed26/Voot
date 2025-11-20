using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;

namespace MDUA.Facade.Interface
{
    public interface IProductDiscountFacade : ICommonFacade<ProductDiscount, ProductDiscountList, ProductDiscountBase>
    {
        ProductDiscountList GetByProductId(int productId);

        void DeleteDiscount(int discountId, string user);
        void ApplyDiscount(ProductDiscountBase discount, string user);
    }
}