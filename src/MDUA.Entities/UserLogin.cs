using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    public partial class UserLogin
    {

    }

    public class UserLoginResult
    {
        public UserLogin UserLogin { get; set; }
        //User Permitted Ids
        public List<int> ids { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsAdmin { get; set; }
        public string Role { get; set; }
        public string ErrorMessage { get; set; }
    }
}