using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MDUA.DataAccess.Interface;
using MDUA.Framework;
using MDUA.Framework.DataAccess; // Ensure this base class namespace is correct

namespace MDUA.DataAccess
{
    public partial class PermissionGroupMapDataAccess : BaseDataAccess, IPermissionGroupMapDataAccess
    {
        public List<int> GetUserPermissionbyUserId(int id)
        {
            List<int> ids = new List<int>();

            // --- CORRECTION HERE ---
            // 1. The outer SELECT aliases PermissionId as 'Id' for the C# code.
            // 2. The WHERE clause now correctly uses 'PermissionId' (the actual column name).
            string SqlQuery = @"
                                SELECT DISTINCT PermissionId AS Id
                                FROM (
                                    -- 1. Direct permissions assigned to user
                                    SELECT PermissionId
                                    FROM UserPermission
                                    WHERE UserId = @UserId AND PermissionId IS NOT NULL

                                    UNION

                                    -- 2. Group-based permissions
                                    SELECT pgm.PermissionId
                                    FROM UserPermission up
                                    INNER JOIN PermissionGroupMap pgm ON up.PermissionGroupId = pgm.PermissionGroupId
                                    WHERE up.UserId = @UserId
                                ) AS AllPermissions
                                WHERE PermissionId IS NOT NULL";
            try
            {
                using (SqlCommand cmd = GetSQLCommand(SqlQuery))
                {
                    AddParameter(cmd, pInt32("UserId", id));

                    DataSet dsResult = GetDataSet(cmd);

                    if (dsResult != null && dsResult.Tables.Count > 0)
                    {
                        // Now we can safely look for the "Id" column because 
                        // the SELECT clause created it for the result set.
                        ids = dsResult.Tables[0].AsEnumerable()
                                        .Where(row => !row.IsNull("Id"))
                                        .Select(row => Convert.ToInt32(row["Id"]))
                                        .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // In a real app, log this error
                throw;
            }

            return ids;
        }
    }
}