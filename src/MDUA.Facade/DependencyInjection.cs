using Microsoft.Extensions.DependencyInjection;
using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Facade.Interface;

namespace MDUA.Facade;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<IUserLoginDataAccess, UserLoginDataAccess>();
        services.AddScoped<IPermissionGroupMapDataAccess, PermissionGroupMapDataAccess>();
        services.AddScoped<IProductDataAccess, ProductDataAccess>();
        services.AddScoped<IProductVariantDataAccess, ProductVariantDataAccess>();
        services.AddScoped<IUserPermissionDataAccess, UserPermissionDataAccess>();
        services.AddScoped<IUserPermissionFacade, UserPermissionFacade>();
        services.AddScoped<IProductCategoryDataAccess, ProductCategoryDataAccess>();
        services.AddScoped<IProductDiscountDataAccess, ProductDiscountDataAccess>();
        services.AddScoped<IVariantPriceStockDataAccess, VariantPriceStockDataAccess>();
        services.AddScoped<IProductImageDataAccess, ProductImageDataAccess>();
        services.AddScoped<IVariantImageDataAccess, VariantImageDataAccess>();
        services.AddScoped<IAttributeNameDataAccess, AttributeNameDataAccess>();
        services.AddScoped<IAttributeValueDataAccess, AttributeValueDataAccess>();
        services.AddScoped<IProductAttributeDataAccess, ProductAttributeDataAccess>();
        services.AddScoped<IVariantAttributeValueDataAccess, VariantAttributeValueDataAccess>();
        services.AddScoped<ICompanyDataAccess, CompanyDataAccess>();

        services.AddServiceFacade();

        return services;
    }

    private static void AddServiceFacade(this IServiceCollection services)
    {
        services.AddScoped<IUserLoginFacade, UserLoginFacade>();
        services.AddScoped<IProductFacade, ProductFacade>();
        services.AddScoped<IProductCategoryFacade, ProductCategoryFacade>();
        services.AddScoped<IProductVariantFacade, ProductVariantFacade>();
        services.AddScoped<IVariantPriceStockFacade, VariantPriceStockFacade>();
        services.AddScoped<IProductDiscountFacade, ProductDiscountFacade>();
        services.AddScoped<IAttributeNameFacade, AttributeNameFacade>();
        services.AddScoped<IAttributeValueFacade, AttributeValueFacade>();
        services.AddScoped<IProductAttributeFacade, ProductAttributeFacade>();
        services.AddScoped<IVariantAttributeValueFacade, VariantAttributeValueFacade>();
        services.AddScoped<IOrderFacade, OrderFacade>();
        services.AddScoped<ICompanyFacade, CompanyFacade>();
    }
}