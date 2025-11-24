using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;

namespace MDUA.Facade
{
    public class VariantPriceStockFacade : IVariantPriceStockFacade
    {
        private readonly IVariantPriceStockDataAccess _variantPriceStockDataAccess;

        public VariantPriceStockFacade(IVariantPriceStockDataAccess variantPriceStockDataAccess)
        {
            _variantPriceStockDataAccess = variantPriceStockDataAccess;
        }

        public long Delete(int _Id)
        {
            return _variantPriceStockDataAccess.Delete(_Id);
        }

        public VariantPriceStock Get(int _Id)
        {
            return _variantPriceStockDataAccess.Get(_Id);
        }

        public VariantPriceStockList GetAll()
        {
            return _variantPriceStockDataAccess.GetAll();
        }

        public VariantPriceStockList GetByQuery(string query)
        {
            return _variantPriceStockDataAccess.GetByQuery(query);
        }

        public VariantPriceStockList GetPaged(PagedRequest request)
        {
            return _variantPriceStockDataAccess.GetPaged(request);
        }

        public long Insert(VariantPriceStockBase Object)
        {
            return _variantPriceStockDataAccess.Insert(Object);
        }

        public long Update(VariantPriceStockBase Object)
        {
            return _variantPriceStockDataAccess.Update(Object);
        }
    }
}