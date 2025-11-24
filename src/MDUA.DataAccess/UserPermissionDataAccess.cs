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
            UserPermissionList list = GetByUserId(userId);

            return list
                .Where(p => p.PermissionId.HasValue)
                .Select(p => p.PermissionId.Value)
                .Distinct()
                .ToList();
        }
    }
}