using MDUA.Entities;

namespace MDUA.Facade.Interface
{
    public interface IOrderFacade
    {
        CustomerLookupModel CheckCustomer(string phone); // Added
        long PlaceOnlineOrder(ProductPageOrderModel model);
    }
}