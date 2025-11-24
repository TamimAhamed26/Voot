using MDUA.DataAccess.Interface;
using MDUA.Entities.List;
using MDUA.Entities;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDUA.Entities.Bases;

namespace MDUA.Facade
{
    public class UserLoginFacade : IUserLoginFacade
    {

        IUserLoginDataAccess _UserLoginDataAccess;
        IPermissionGroupMapDataAccess _IPermissionGroupMapDataAccess;

        public UserLoginFacade(IUserLoginDataAccess userLoginDataAccess,
            IPermissionGroupMapDataAccess _iPermissionGroupMapDataAccess)
        {
            _UserLoginDataAccess = userLoginDataAccess;
            _IPermissionGroupMapDataAccess = _iPermissionGroupMapDataAccess;
        }

        #region common implementation 

        public long Delete(int _Id)
        {
            return _UserLoginDataAccess.Delete(_Id);
        }

        public UserLogin Get(int _Id)
        {
            return _UserLoginDataAccess.Get(_Id);
        }

        public UserLoginList GetAll()
        {
            return _UserLoginDataAccess.GetAll();
        }

        public UserLoginList GetByQuery(string query)
        {
            return _UserLoginDataAccess.GetByQuery(query);
        }

        public long Insert(UserLoginBase Object)
        {
            return _UserLoginDataAccess.Insert(Object);
        }
        public long Update(UserLoginBase Object)
        {
            return _UserLoginDataAccess.Update(Object);
        }

        #endregion

        #region extented implementation
        public UserLoginResult GetUserLoginBy(string email, string password)
        {
            UserLoginResult result = new UserLoginResult();

            UserLogin obUser = _UserLoginDataAccess.GetUserLogin(email, password);
            if (obUser != null)
            {
                result.IsSuccess = true;
                result.UserLogin = obUser;
                result.ids = GetUserPermissionByUserId(obUser.Id);
                //User type 
                //Role Setup 

                result.IsAdmin = true;
            }
            else
            {
                result.ErrorMessage = "Wrong user/password";
            }


            return result;
        }
        public List<int> GetUserPermissionByUserId(int Id)
        {
            return _IPermissionGroupMapDataAccess.GetUserPermissionbyUserId(Id);
        }
        #endregion
    }
}