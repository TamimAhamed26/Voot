// ICompanyDataAccess.cs
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.DataAccess.Interface
{
    public interface ICompanyDataAccess
    {
        Company Get(int id);
        long Delete(int id);
        CompanyList GetAll();
        CompanyList GetByQuery(string query);
        CompanyList GetPaged(PagedRequest request);
        long Insert(CompanyBase obj);
        long Update(CompanyBase obj);
    }
}