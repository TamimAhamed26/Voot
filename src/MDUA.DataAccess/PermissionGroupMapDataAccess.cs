using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
	public partial class PermissionGroupMapDataAccess : IPermissionGroupMapDataAccess
    {

        public List<int> GetUserPermissionbyUserId(int id)
        {
            List<int> ids = new List<int>();
            string SqlQuery = @"DECLARE @UserId INT = @Id -- Replace with your actual UserId

                                SELECT DISTINCT PermissionId Id
                                FROM (
                                    -- Direct permissions
                                    SELECT PermissionId
                                    FROM UserPermission
                                    WHERE UserId = @UserId

                                    UNION

                                    -- Group-based permissions
                                    SELECT pgm.PermissionId
                                    FROM UserPermission up
                                    INNER JOIN PermissionGroupMap pgm ON up.GroupId = pgm.GroupId
                                    WHERE up.UserId = @UserId
                                ) AS AllPermissions
                                WHERE PermissionId IS NOT NULL";
            try
            {
                using (SqlCommand cmd = GetSQLCommand(SqlQuery))
                {
                    AddParameter(cmd, pInt32("Id", id));
                    DataSet dsResult = GetDataSet(cmd);
                    if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                    {

                    }
                    ids = dsResult.Tables[0].AsEnumerable()
                                    .Where(row => !row.IsNull("Id"))
                                    .Select(row => Convert.ToInt32(row["Id"]))
                                    .ToList();

                }
            }
            catch (Exception ex)
            {

            }

            return ids;
        }
    }	
}
