using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;

namespace MDUA.Facade.Interface
{
    public interface IVariantPriceStockFacade
    {
        long Delete(int _Id);
        VariantPriceStock Get(int _Id);
        VariantPriceStockList GetAll();
        VariantPriceStockList GetByQuery(string query);
        VariantPriceStockList GetPaged(PagedRequest request);
        long Insert(VariantPriceStockBase Object);
        long Update(VariantPriceStockBase Object);
    }
}