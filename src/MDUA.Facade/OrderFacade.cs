using MDUA.DataAccess;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework.DataAccess;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MDUA.Facade
{
    public class OrderFacade : IOrderFacade
    {
        public OrderFacade() { }

        public long PlaceOnlineOrder(ProductPageOrderModel model)
        {
            Debug.WriteLine($"=== PROCESSING ORDER FOR: {model.Customer.Phone} ===");

            // Start Transaction
            using (SqlTransaction transaction = BaseDataAccess.BeginTransaction())
            {
                try
                {
                    // 1. Initialize DataAccess with the active transaction
                    var customerDA = new CustomerDataAccess(transaction);
                    var addressDA = new AddressDataAccess(transaction);
                    var companyCustomerDA = new CompanyCustomerDataAccess(transaction);
                    var orderHeaderDA = new SalesOrderHeaderDataAccess(transaction);
                    var orderDetailDA = new SalesOrderDetailDataAccess(transaction);
                    var variantDA = new ProductVariantDataAccess(transaction);
                    var productDA = new ProductDataAccess(transaction);

                    // 2. Resolve Product & Company Context
                    var variant = variantDA.Get(model.Order.VariantId);
                    if (variant == null) throw new Exception($"Product Variant {model.Order.VariantId} not found.");

                    var product = productDA.Get(variant.ProductId);
                    if (product == null) throw new Exception($"Product {variant.ProductId} not found.");

                    int companyId = product.CompanyId;

                    // 3. Customer Strategy (Idempotent: Find OR Create OR Recover)
                    int customerId = 0;
                    string safeEmail = !string.IsNullOrEmpty(model.Customer.Email)
                        ? model.Customer.Email
                        : $"{model.Customer.Phone}@guest.voot.com";

                    var newCust = new Customer
                    {
                        CustomerName = model.Customer.Name,
                        Phone = model.Customer.Phone,
                        Email = safeEmail,
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Try insert (The updated SP handles duplicates and returns ID)
                    long custIdResult = customerDA.Insert(newCust);

                    // Resolve ID
                    customerId = (custIdResult > 0) ? (int)custIdResult : newCust.Id;

                    if (customerId <= 0)
                    {
                        // Fallback Check (If SP logic failed or older SP used)
                        var existing = customerDA.GetByQuery($"Phone = '{model.Customer.Phone}'");
                        if (existing != null && existing.Count > 0)
                            customerId = existing[0].Id;
                        else
                            throw new Exception("Failed to resolve Customer ID. Please check 'InsertCustomer' SP logic.");
                    }

                    // 4. Address (Idempotent)
                    var newAddr = new Address
                    {
                        CustomerId = customerId,
                        Street = model.Address.Street,
                        City = model.Address.City,
                        Divison = model.Address.Divison,
                        PostalCode = model.Address.PostalCode,
                        // NEW: Handle ZipCode separately if provided, or fallback to PostalCode
                        ZipCode = (model.Address.ZipCode ?? model.Address.PostalCode ?? "").ToCharArray(),
                        Country = model.Address.Country ?? "Bangladesh",
                        AddressType = model.Address.AddressType,
                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    long addrResult = addressDA.Insert(newAddr);
                    int addressId = (addrResult > 0) ? (int)addrResult : newAddr.Id;

                    if (addressId <= 0)
                    {
                        // Fallback Check
                        var existingAddr = addressDA.GetByQuery($"CustomerId={customerId} AND Street='{model.Address.Street}'");
                        if (existingAddr != null && existingAddr.Count > 0)
                            addressId = existingAddr[0].Id;
                        else
                            throw new Exception("Failed to resolve Address ID. Please check 'InsertAddress' SP.");
                    }

                    // 5. Link Company
                    var link = new CompanyCustomer { CompanyId = companyId, CustomerId = customerId };
                    long linkResult = companyCustomerDA.Insert(link);

                    int companyCustomerId = (linkResult > 0) ? (int)linkResult : link.Id;
                    if (companyCustomerId <= 0)
                    {
                        var existingLink = companyCustomerDA.GetByQuery($"CompanyId={companyId} AND CustomerId={customerId}");
                        if (existingLink != null && existingLink.Count > 0)
                            companyCustomerId = existingLink[0].Id;
                        else
                            throw new Exception("Failed to resolve CompanyCustomer Link ID.");
                    }

                    // 6. Order Header
                    var header = new SalesOrderHeader
                    {
                        CompanyCustomerId = companyCustomerId,
                        AddressId = addressId,
                        SalesChannelId = 1, // 1 = Online
                        // NOTE: SalesOrderId, OnlineOrderId, etc. are computed in DB. DO NOT SET HERE.
                        OrderDate = DateTime.Now,
                        TotalAmount = model.Order.TotalAmount,
                        DiscountAmount = 0,
                        Status = "Draft",
                        IsActive = true,
                        Confirmed = false,
                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    long headerResult = orderHeaderDA.Insert(header);
                    int headerId = (headerResult > 0) ? (int)headerResult : header.Id;

                    if (headerId <= 0) throw new Exception("Order Header Insertion Failed (ID returned 0). Check 'InsertSalesOrderHeader' SP.");

                    // 7. Order Detail
                    var detail = new SalesOrderDetail
                    {
                        SalesOrderId = headerId,
                        ProductId = variant.ProductId,
                        Quantity = model.Order.Quantity,
                        UnitPrice = model.Order.TotalAmount / (model.Order.Quantity > 0 ? model.Order.Quantity : 1),
                        // ProfitAmount: calculated by DB trigger or Logic
                        ProfitAmount = 0,
                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    orderDetailDA.Insert(detail);

                    BaseDataAccess.CloseTransaction(true, transaction);
                    Debug.WriteLine($"SUCCESS: Order {headerId} created.");
                    return headerId;
                }
                catch (Exception)
                {
                    BaseDataAccess.CloseTransaction(false, transaction);
                    throw; // CRITICAL: Re-throw ORIGINAL exception to see the SQL error in Controller/UI
                }
            }
        }
        public CustomerLookupModel CheckCustomer(string phone)
        {
            // We use a transaction scope for consistency, even for reads
            using (SqlTransaction transaction = BaseDataAccess.BeginTransaction())
            {
                try
                {
                    var customerDA = new CustomerDataAccess(transaction);
                    var addressDA = new AddressDataAccess(transaction);

                    // 1. Find Customer
                    var customers = customerDA.GetByQuery($"Phone = '{phone}'");

                    if (customers != null && customers.Count > 0)
                    {
                        var customer = customers[0];

                        // 2. Find Latest Address
                        var addresses = addressDA.GetByQuery($"CustomerId = {customer.Id}");
                        AddressBase lastAddress = null;

                        if (addresses != null && addresses.Count > 0)
                        {
                            // Get the one with the highest ID (most recently added)
                            lastAddress = addresses.OrderByDescending(a => a.Id).FirstOrDefault();
                        }

                        // 3. Map to DTO
                        var result = new CustomerLookupModel
                        {
                            Found = true,
                            Name = customer.CustomerName,
                            Email = customer.Email,
                            LastAddress = lastAddress != null ? new AddressDto
                            {
                                Street = lastAddress.Street,
                                City = lastAddress.City,
                                Divison = lastAddress.Divison,
                                PostalCode = lastAddress.PostalCode,
                                // Handle Char[] to String conversion safely
                                ZipCode = lastAddress.ZipCode != null ? new string(lastAddress.ZipCode).Trim() : "",
                                Country = lastAddress.Country,
                                AddressType = lastAddress.AddressType
                            } : null
                        };

                        BaseDataAccess.CloseTransaction(true, transaction);
                        return result;
                    }

                    BaseDataAccess.CloseTransaction(true, transaction);
                    return new CustomerLookupModel { Found = false };
                }
                catch (Exception)
                {
                    BaseDataAccess.CloseTransaction(false, transaction);
                    throw;
                }
            }
        }
    }
}