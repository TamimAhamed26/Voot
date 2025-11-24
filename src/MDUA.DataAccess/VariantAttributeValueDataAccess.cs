#nullable disable
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class VariantAttributeValueDataAccess : BaseDataAccess, IVariantAttributeValueDataAccess
    {
        #region Constants
        private const string INSERTVARIANTATTRIBUTEVALUE = "InsertVariantAttributeValue";
        private const string UPDATEVARIANTATTRIBUTEVALUE = "UpdateVariantAttributeValue";
        private const string DELETEVARIANTATTRIBUTEVALUE = "DeleteVariantAttributeValue";
        private const string GETVARIANTATTRIBUTEVALUEBYID = "GetVariantAttributeValueById";
        private const string GETALLVARIANTATTRIBUTEVALUE = "GetAllVariantAttributeValue";
        private const string GETPAGEDVARIANTATTRIBUTEVALUE = "GetPagedVariantAttributeValue";
        private const string GETVARIANTATTRIBUTEVALUEMAXIMUMID = "GetVariantAttributeValueMaximumId";
        private const string GETVARIANTATTRIBUTEVALUEROWCOUNT = "GetVariantAttributeValueRowCount";
        private const string GETVARIANTATTRIBUTEVALUEBYQUERY = "GetVariantAttributeValueByQuery";

        // New SP Constants
        private const string GETVARIANTATTRIBUTEVALUEBYVARIANTID = "GetVariantAttributeValueByVariantId";
        private const string GETVARIANTATTRIBUTEVALUEBYATTRIBUTEID = "GetVariantAttributeValueByAttributeId";
        private const string GETVARIANTATTRIBUTEVALUEBYATTRIBUTEVALUEID = "GetVariantAttributeValueByAttributeValueId";
        #endregion

        #region Constructors
        public VariantAttributeValueDataAccess(IConfiguration configuration) : base(configuration) { }
        public VariantAttributeValueDataAccess(ClientContext context) : base(context) { }
        public VariantAttributeValueDataAccess(SqlTransaction transaction) : base(transaction) { }
        public VariantAttributeValueDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams
        private void AddCommonParams(SqlCommand cmd, VariantAttributeValueBase obj)
        {
            AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_VariantId, obj.VariantId));
            AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_AttributeId, obj.AttributeId));
            AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_AttributeValueId, obj.AttributeValueId));
            AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_DisplayOrder, obj.DisplayOrder));
        }
        #endregion

        #region Insert
        public long Insert(VariantAttributeValueBase obj)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTVARIANTATTRIBUTEVALUE);
                AddParameter(cmd, pInt32Out(VariantAttributeValueBase.Property_Id));
                AddCommonParams(cmd, obj);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    obj.Id = (int)GetOutParameter(cmd, VariantAttributeValueBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(obj, x);
            }
        }
        #endregion

        #region Update
        public long Update(VariantAttributeValueBase obj)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEVARIANTATTRIBUTEVALUE);
                AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_Id, obj.Id));
                AddCommonParams(cmd, obj);
                return UpdateRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(obj, x);
            }
        }
        #endregion

        #region Delete
        public long Delete(int _Id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEVARIANTATTRIBUTEVALUE);
                AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_Id, _Id));
                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(VariantAttributeValue), _Id, x);
            }
        }
        #endregion

        #region Get By Id
        public VariantAttributeValue Get(int _Id)
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYID))
            {
                AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_Id, _Id));
                return GetObject(cmd);
            }
        }
        #endregion

        #region GetAll
        public VariantAttributeValueList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLVARIANTATTRIBUTEVALUE))
            {
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public VariantAttributeValueList GetByVariantId(int _VariantId)
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYVARIANTID))
            {
                AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_VariantId, _VariantId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public VariantAttributeValueList GetByAttributeId(int _AttributeId)
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYATTRIBUTEID))
            {
                AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_AttributeId, _AttributeId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public VariantAttributeValueList GetByAttributeValueId(int _AttributeValueId)
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYATTRIBUTEVALUEID))
            {
                AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_AttributeValueId, _AttributeValueId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        #endregion

        #region GetPaged
        public VariantAttributeValueList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDVARIANTATTRIBUTEVALUE))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                var list = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = (int)GetOutParameter(cmd, "TotalRows");
                return list;
            }
        }
        #endregion

        #region GetByQuery
        public VariantAttributeValueList GetByQuery(string query)
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        #endregion

        #region GetMaxId
        public int GetMaxId()
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEMAXIMUMID))
            {
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                int maxId = 0;
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    maxId = reader.GetInt32(0);
                }
                reader.Close();
                reader.Dispose();
                return maxId;
            }
        }
        #endregion

        #region GetRowCount
        public int GetRowCount()
        {
            using (SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEROWCOUNT))
            {
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                int rowCount = 0;
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    rowCount = reader.GetInt32(0);
                }
                reader.Close();
                reader.Dispose();
                return rowCount;
            }
        }
        #endregion

        #region Fill Methods
        protected void FillObject(VariantAttributeValueBase obj, SqlDataReader reader, int start)
        {
            obj.Id = reader.GetInt32(start + 0);
            obj.VariantId = reader.GetInt32(start + 1);
            obj.AttributeId = reader.GetInt32(start + 2);
            obj.AttributeValueId = reader.GetInt32(start + 3);
            obj.DisplayOrder = reader.GetInt32(start + 4);
            // Assuming FillBaseObject exists in your BaseDataAccess
            FillBaseObject(obj, reader, (start + 5));
            obj.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        protected void FillObject(VariantAttributeValueBase obj, SqlDataReader reader)
        {
            FillObject(obj, reader, 0);
        }

        private VariantAttributeValue GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read())
                {
                    VariantAttributeValue obj = new VariantAttributeValue();
                    FillObject(obj, reader);
                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        private VariantAttributeValueList GetList(SqlCommand cmd, long rows)
        {
            SqlDataReader reader;
            SelectRecords(cmd, out reader);
            VariantAttributeValueList list = new VariantAttributeValueList();

            using (reader)
            {
                while (reader.Read() && rows-- != 0)
                {
                    VariantAttributeValue obj = new VariantAttributeValue();
                    FillObject(obj, reader);
                    list.Add(obj);
                }
                reader.Close();
            }
            return list;
        }
        #endregion
    }
}