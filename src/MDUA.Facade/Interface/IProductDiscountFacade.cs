using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework; // Kept for PagedRequest if needed
using System.Collections.Generic;

namespace MDUA.Facade.Interface
{
    public interface IProductDiscountFacade
    {
        ProductDiscount Get(int id);
        ProductDiscountList GetAll();
        ProductDiscountList GetByQuery(string query);
        ProductDiscountList GetPaged(PagedRequest request);
        long Insert(ProductDiscountBase entity);
        long Update(ProductDiscountBase entity);
        long Delete(int id);

        ProductDiscountList GetByProductId(int productId);


        void ApplyDiscount(ProductDiscountBase discount, string user);

        void DeleteDiscount(int discountId, string user);

      
        decimal CalculateNewPrice(decimal original, string type, decimal value);

        ProductDiscount GetBestDiscount(int productId, decimal basePrice);
    }
}