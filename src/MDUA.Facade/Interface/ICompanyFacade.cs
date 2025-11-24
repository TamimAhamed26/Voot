// ICompanyFacade.cs
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;

namespace MDUA.Facade.Interface
{
    public interface ICompanyFacade
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
