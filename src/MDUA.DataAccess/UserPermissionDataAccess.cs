using MDUA.DataAccess.Interface;
using MDUA.Entities.List;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq; 

namespace MDUA.DataAccess
{

    public partial class UserPermissionDataAccess : IUserPermissionDataAccess
    {
        public List<int> GetUserPermissionIds(int userId)
        {
            
            UserPermissionList permissionsList = this.GetByUserId(userId);

            List<int> idList = permissionsList
                .Select(p => p.PermissionId)
                .Distinct()
                .ToList();

            return idList;
        }
    }
}