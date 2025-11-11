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


        services.AddServiceFacade();

        return services;
    }

    private static void AddServiceFacade(this IServiceCollection services)
    {
        services.AddScoped<IUserLoginFacade, UserLoginFacade>();

        services.AddScoped<IProductFacade, ProductFacade>();
    }
}