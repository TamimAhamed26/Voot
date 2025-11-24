using MDUA.DataAccess.Interface;
using MDUA.Framework;
using MDUA.Framework.DataAccess;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    // This partial class declaration is all we need.
    // It automatically combines with the /Bases/ProductCategoryDataAccess.cs file,
    // which already contains the constructors and the 'GetAll' method.
    public partial class ProductCategoryDataAccess : BaseDataAccess, IProductCategoryDataAccess
    {
        // Add constructors ONLY IF they are NOT in the base class.
        // Since they are in the base class, we leave this file empty.
        // We can add custom (non-generated) methods here in the future.
    }
}