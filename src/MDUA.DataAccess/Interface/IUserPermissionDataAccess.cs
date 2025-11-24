using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System.Collections.Generic; // Import this

namespace MDUA.DataAccess.Interface
{
    public interface IUserPermissionDataAccess : ICommonDataAccess<UserPermission, UserPermissionList, UserPermissionBase>
    {
        UserPermissionList GetByUserId(int _UserId);
        UserPermissionList GetByPermissionId(int? _PermissionId);
        UserPermissionList GetByPermissionGroupId(int? _PermissionGroupId);



        List<int> GetUserPermissionIds(int userId);
    }
}