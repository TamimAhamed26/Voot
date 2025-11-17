using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductDataAccess
    {
        public ProductDetailsModel GetProductDetails(string slug)
        {
            var model = new ProductDetailsModel();

            using (SqlCommand cmd = GetSPCommand("sp_GetProductDetails"))
            {
                AddParameter(cmd, pNVarChar("Slug", 500, slug));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    if (reader.Read())
                        model.Product = FillProduct(reader);
                    else
                        return model;

                    // Product Images
                    reader.NextResult();
                    while (reader.Read())
                        model.ProductImages.Add(FillProductImage(reader));

                    // Variants
                    reader.NextResult();
                    while (reader.Read())
                        model.Variants.Add(FillProductVariant(reader));

                    // Variant Price/Stock
                    reader.NextResult();
                    while (reader.Read())
                    {
                        var vps = FillVariantPriceStock(reader);
                        model.VariantPriceStocks[vps.Id] = vps;
                    }

                    // Variant Images
                    reader.NextResult();
                    while (reader.Read())
                    {
                        var vi = FillVariantImage(reader);
                        if (!model.VariantImages.ContainsKey(vi.VariantId))
                            model.VariantImages[vi.VariantId] = new VariantImageList();
                        model.VariantImages[vi.VariantId].Add(vi);
                    }

                    // Options (Attributes)
                    var optionValuesMap = new Dictionary<int, AttributeValueList>();
                    reader.NextResult();
                    while (reader.Read())
                    {
                        var option = new ProductAttributeOptionModel
                        {
                            AttributeId = (int)reader["AttributeId"],
                            AttributeName = (string)reader["AttributeName"]
                        };
                        model.Options.Add(option);
                        optionValuesMap[option.AttributeId] = new AttributeValueList();
                    }

                    // Attribute Values
                    reader.NextResult();
                    while (reader.Read())
                    {
                        var val = FillAttributeValue(reader);
                        if (optionValuesMap.ContainsKey(val.AttributeId))
                            optionValuesMap[val.AttributeId].Add(val);
                    }

                    foreach (var opt in model.Options)
                        opt.Values = optionValuesMap[opt.AttributeId];

                    // Variant-to-Value Mappings
                    reader.NextResult();
                    while (reader.Read())
                        model.VariantAttributeValues.Add(FillVariantAttributeValue(reader));
                }
            }

            return model;
        }

        // --- Private Helper Fill Methods ---
        private Product FillProduct(SqlDataReader reader)
        {
            var p = new Product();
            FillObject(p, reader);
            return p;
        }

        private ProductImage FillProductImage(SqlDataReader reader)
        {
            var pi = new ProductImage();
            pi.Id = (int)reader["Id"];
            pi.ProductId = (int)reader["ProductId"];
            pi.ImageUrl = (string)reader["ImageUrl"];
            pi.IsPrimary = (bool)reader["IsPrimary"];
            pi.SortOrder = (int)reader["SortOrder"];
            if (reader["AltText"] != DBNull.Value) pi.AltText = (string)reader["AltText"];
            return pi;
        }

        private ProductVariant FillProductVariant(SqlDataReader reader)
        {
            var pv = new ProductVariant();
            pv.Id = (int)reader["Id"];
            pv.ProductId = (int)reader["ProductId"];
            pv.VariantName = (string)reader["VariantName"];
            if (reader["SKU"] != DBNull.Value) pv.SKU = (string)reader["SKU"];
            if (reader["Barcode"] != DBNull.Value) pv.Barcode = (string)reader["Barcode"];
            if (reader["VariantPrice"] != DBNull.Value) pv.VariantPrice = (decimal)reader["VariantPrice"];
            pv.IsActive = (bool)reader["IsActive"];
            return pv;
        }

        private VariantPriceStock FillVariantPriceStock(SqlDataReader reader)
        {
            var vps = new VariantPriceStock();
            vps.Id = (int)reader["Id"];
            vps.Price = (decimal)reader["Price"];
            if (reader["CompareAtPrice"] != DBNull.Value) vps.CompareAtPrice = (decimal)reader["CompareAtPrice"];
            if (reader["CostPrice"] != DBNull.Value) vps.CostPrice = (decimal)reader["CostPrice"];
            vps.StockQty = (int)reader["StockQty"];
            vps.TrackInventory = (bool)reader["TrackInventory"];
            vps.AllowBackorder = (bool)reader["AllowBackorder"];
            if (reader["WeightGrams"] != DBNull.Value) vps.WeightGrams = (int)reader["WeightGrams"];
            return vps;
        }

        private VariantImage FillVariantImage(SqlDataReader reader)
        {
            var vi = new VariantImage();
            vi.Id = (int)reader["Id"];
            vi.VariantId = (int)reader["VariantId"];
            vi.ImageUrl = (string)reader["ImageUrl"];
            if (reader["AltText"] != DBNull.Value) vi.AltText = (string)reader["AltText"];
            vi.DisplayOrder = (int)reader["DisplayOrder"];
            return vi;
        }

        private AttributeValue FillAttributeValue(SqlDataReader reader)
        {
            var av = new AttributeValue();
            av.Id = (int)reader["Id"];
            av.AttributeId = (int)reader["AttributeId"];
            av.Value = (string)reader["Value"];
            av.DisplayOrder = (int)reader["DisplayOrder"];
            return av;
        }

        private VariantAttributeValue FillVariantAttributeValue(SqlDataReader reader)
        {
            var vav = new VariantAttributeValue();
            vav.Id = (int)reader["Id"];
            vav.VariantId = (int)reader["VariantId"];
            vav.AttributeId = (int)reader["AttributeId"];
            vav.AttributeValueId = (int)reader["AttributeValueId"];
            vav.DisplayOrder = (int)reader["DisplayOrder"];
            return vav;
        }
    }
}
