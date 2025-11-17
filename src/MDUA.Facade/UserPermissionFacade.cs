using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System.Collections.Generic; 

namespace MDUA.Facade
{
    public class UserPermissionFacade : IUserPermissionFacade
    {
        private readonly IUserPermissionDataAccess _userPermissionDataAccess;

        public UserPermissionFacade(IUserPermissionDataAccess userPermissionDataAccess)
        {
            _userPermissionDataAccess = userPermissionDataAccess;
        }

        public List<int> GetUserPermissionIds(int userId)
        {
            return _userPermissionDataAccess.GetUserPermissionIds(userId);
        }

        public UserPermissionList GetByUserId(int _UserId)
        {
            return _userPermissionDataAccess.GetByUserId(_UserId);
        }

        public UserPermissionList GetByPermissionId(int _PermissionId)
        {
            return _userPermissionDataAccess.GetByPermissionId(_PermissionId);
        }

        public UserPermissionList GetByPermissionGroupId(int _PermissionGroupId)
        {
            return _userPermissionDataAccess.GetByPermissionGroupId(_PermissionGroupId);
        }

        public long Delete(int _Id)
        {
            return _userPermissionDataAccess.Delete(_Id);
        }

        public UserPermission Get(int _Id)
        {
            return _userPermissionDataAccess.Get(_Id);
        }

        public UserPermissionList GetAll()
        {
            return _userPermissionDataAccess.GetAll();
        }

        public UserPermissionList GetByQuery(string query)
        {
            return _userPermissionDataAccess.GetByQuery(query);
        }

        public UserPermissionList GetPaged(PagedRequest request)
        {
            return _userPermissionDataAccess.GetPaged(request);
        }

        public long Insert(UserPermissionBase Object)
        {
            return _userPermissionDataAccess.Insert(Object);
        }

        public long Update(UserPermissionBase Object)
        {
            return _userPermissionDataAccess.Update(Object);
        }
    }
}