//using MDUA.DataAccess.Interface; // <-- Must include this
//using MDUA.Entities;
//using MDUA.Entities.List;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;

//namespace MDUA.DataAccess
//{
//    // This is the OTHER "part" of the partial class.
//    // Notice it does NOT inherit from anything.
//    // It has access to the protected methods from the Base class
//    // (like GetSPCommand, AddParameter) because it's one class.
//    public partial class ProductDataAccess
//    {
//        public ProductDetailsModel GetProductDetails(int productId)
//        {
//            var model = new ProductDetailsModel();

//            // GetSPCommand() is accessible because it's a protected method
//            // in BaseDataAccess, which our OTHER partial file inherits from.
//            using (SqlCommand cmd = GetSPCommand("sp_GetProductDetails"))
//            {
//                AddParameter(cmd, pInt32("ProductId", productId));

//                SqlDataReader reader;
//                SelectRecords(cmd, out reader);

//                using (reader)
//                {
//                    //// Result 1: Product
//                    if (reader.Read())
//                    {
//                        // FillProduct() is our private helper below
//                        model.Product = FillProduct(reader);
//                    }
//                    else
//                    {
//                        return model; // Not found
//                    }

//                    // Result 2: Product Images
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        model.ProductImages.Add(FillProductImage(reader));
//                    }

//                    // Result 3: Product Variants
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        model.Variants.Add(FillProductVariant(reader));
//                    }

//                    // Result 4: Variant Price/Stock
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        var vps = FillVariantPriceStock(reader);
//                        model.VariantPriceStocks[vps.Id] = vps;
//                    }

//                    // Result 5: Variant Images
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        var vi = FillVariantImage(reader);
//                        if (!model.VariantImages.ContainsKey(vi.VariantId))
//                        {
//                            model.VariantImages[vi.VariantId] = new VariantImageList();
//                        }
//                        model.VariantImages[vi.VariantId].Add(vi);
//                    }

//                    // Result 6: Product Options (Attribute Names)
//                    var optionValuesMap = new Dictionary<int, AttributeValueList>();
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        var option = new ProductAttributeOptionModel
//                        {
//                            AttributeId = (int)reader["AttributeId"],
//                            AttributeName = (string)reader["AttributeName"]
//                        };
//                        model.Options.Add(option);
//                        optionValuesMap[option.AttributeId] = new AttributeValueList();
//                    }

//                    // Result 7: Attribute Values
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        var val = FillAttributeValue(reader);
//                        if (optionValuesMap.ContainsKey(val.AttributeId))
//                        {
//                            optionValuesMap[val.AttributeId].Add(val);
//                        }
//                    }

//                    AGAIN
//                    foreach (var opt in model.Options)
//                    {
//                        opt.Values = optionValuesMap[opt.AttributeId];
//                    }

//                    // Result 8: Variant-to-Value Mappings
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        model.VariantAttributeValues.Add(FillVariantAttributeValue(reader));
//                    }
//                }
//            }

//            return model;
//        }

//        // --- Private Helper Fill Methods for the DTO ---
//        // These are ONLY defined here and will not conflict with the Base.

//        private Product FillProduct(SqlDataReader reader)
//        {
//            var p = new Product();
//            // FillObject is accessible because it's a protected method
//            // in our OTHER partial file (ProductDataAccess.cs in Bases)
//            FillObject(p, reader);
//            return p;
//        }

//        private ProductImage FillProductImage(SqlDataReader reader)
//        {
//            var pi = new ProductImage();
//            pi.Id = (int)reader["Id"];
//            pi.ProductId = (int)reader["ProductId"];
//            pi.ImageUrl = (string)reader["ImageUrl"];
//            pi.IsPrimary = (bool)reader["IsPrimary"];
//            pi.SortOrder = (int)reader["SortOrder"];
//            if (reader["AltText"] != DBNull.Value) pi.AltText = (string)reader["AltText"];
//            if (reader["CreatedBy"] != DBNull.Value) pi.CreatedBy = (string)reader["CreatedBy"];
//            pi.CreatedAt = (DateTime)reader["CreatedAt"];
//            if (reader["UpdatedBy"] != DBNull.Value) pi.UpdatedBy = (string)reader["UpdatedBy"];
//            if (reader["UpdatedAt"] != DBNull.Value) pi.UpdatedAt = (DateTime)reader["UpdatedAt"];
//            return pi;
//        }

//        private ProductVariant FillProductVariant(SqlDataReader reader)
//        {
//            var pv = new ProductVariant();
//            pv.Id = (int)reader["Id"];
//            pv.ProductId = (int)reader["ProductId"];
//            pv.VariantName = (string)reader["VariantName"];
//            if (reader["SKU"] != DBNull.Value) pv.SKU = (string)reader["SKU"];
//            if (reader["Barcode"] != DBNull.Value) pv.Barcode = (string)reader["Barcode"];
//            if (reader["VariantPrice"] != DBNull.Value) pv.VariantPrice = (decimal)reader["VariantPrice"];
//            pv.IsActive = (bool)reader["IsActive"];
//            if (reader["CreatedBy"] != DBNull.Value) pv.CreatedBy = (string)reader["CreatedBy"];
//            pv.CreatedAt = (DateTime)reader["CreatedAt"];
//            if (reader["UpdatedBy"] != DBNull.Value) pv.UpdatedBy = (string)reader["UpdatedBy"];
//            if (reader["UpdatedAt"] != DBNull.Value) pv.UpdatedAt = (DateTime)reader["UpdatedAt"];
//            return pv;
//        }

//        private VariantPriceStock FillVariantPriceStock(SqlDataReader reader)
//        {
//            var vps = new VariantPriceStock();
//            vps.Id = (int)reader["Id"];
//            vps.Price = (decimal)reader["Price"];
//            if (reader["CompareAtPrice"] != DBNull.Value) vps.CompareAtPrice = (decimal)reader["CompareAtPrice"];
//            if (reader["CostPrice"] != DBNull.Value) vps.CostPrice = (decimal)reader["CostPrice"];
//            vps.StockQty = (int)reader["StockQty"];
//            vps.TrackInventory = (bool)reader["TrackInventory"];
//            vps.AllowBackorder = (bool)reader["AllowBackorder"];
//            if (reader["WeightGrams"] != DBNull.Value) vps.WeightGrams = (int)reader["WeightGrams"];
//            return vps;
//        }

//        private VariantImage FillVariantImage(SqlDataReader reader)
//        {
//            var vi = new VariantImage();
//            vi.Id = (int)reader["Id"];
//            vi.VariantId = (int)reader["VariantId"];
//            vi.ImageUrl = (string)reader["ImageUrl"];
//            if (reader["AltText"] != DBNull.Value) vi.AltText = (string)reader["AltText"];
//            vi.DisplayOrder = (int)reader["DisplayOrder"];
//            return vi;
//        }

//        private AttributeValue FillAttributeValue(SqlDataReader reader)
//        {
//            var av = new AttributeValue();
//            av.Id = (int)reader["Id"];
//            av.AttributeId = (int)reader["AttributeId"];
//            av.Value = (string)reader["Value"];
//            av.DisplayOrder = (int)reader["DisplayOrder"];
//            return av;
//        }

//        private VariantAttributeValue FillVariantAttributeValue(SqlDataReader reader)
//        {
//            var vav = new VariantAttributeValue();
//            vav.Id = (int)reader["Id"];
//            vav.VariantId = (int)reader["VariantId"];
//            vav.AttributeId = (int)reader["AttributeId"];
//            vav.AttributeValueId = (int)reader["AttributeValueId"];
//            vav.DisplayOrder = (int)reader["DisplayOrder"];
//            return vav;
//        }
//    }
//}