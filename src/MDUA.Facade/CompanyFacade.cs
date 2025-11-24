using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;

namespace MDUA.Facade
{
    public class CompanyFacade : ICompanyFacade
    {
        private readonly ICompanyDataAccess _dataAccess;

        public CompanyFacade(ICompanyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Company Get(int id) => _dataAccess.Get(id);
        public long Delete(int id) => _dataAccess.Delete(id);
        public CompanyList GetAll() => _dataAccess.GetAll();
        public CompanyList GetByQuery(string query) => _dataAccess.GetByQuery(query);
        public CompanyList GetPaged(PagedRequest request) => _dataAccess.GetPaged(request);
        public long Insert(CompanyBase obj) => _dataAccess.Insert(obj);
        public long Update(CompanyBase obj) => _dataAccess.Update(obj);
    }
}