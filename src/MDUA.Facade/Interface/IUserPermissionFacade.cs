using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using System.Collections.Generic; 

namespace MDUA.Facade.Interface
{
    public interface IUserPermissionFacade : ICommonFacade<UserPermission, UserPermissionList, UserPermissionBase>
    {
   
        UserPermissionList GetByUserId(int _UserId);
        UserPermissionList GetByPermissionId(int _PermissionId);
        UserPermissionList GetByPermissionGroupId(int _PermissionGroupId);

    
        List<int> GetUserPermissionIds(int userId);
    }
}