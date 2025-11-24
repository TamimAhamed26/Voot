using System;

namespace MDUA.Entities
{
    public class ProductPageOrderModel
    {
        public CustomerDto Customer { get; set; }
        public AddressDto Address { get; set; }
        public OrderItemDto Order { get; set; }
    }

    public class CustomerDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class CustomerLookupModel
    {
        public bool Found { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public AddressDto LastAddress { get; set; }
    }
    public class AddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Divison { get; set; } // Matches your DB spelling
        public string PostalCode { get; set; }

        public string ZipCode { get; set; } // NEW PROPERTY
        public string Country { get; set; }
        public string AddressType { get; set; }
    }

    public class OrderItemDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}