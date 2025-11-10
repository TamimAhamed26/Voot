using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IUserLoginFacade: ICommonFacade<UserLogin, UserLoginList, UserLoginBase>
    { 
        public UserLoginResult GetUserLoginBy(string email, string password);
    }
}
