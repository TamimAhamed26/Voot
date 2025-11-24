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
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsAdmin { get; set; }

        public UserLogin UserLogin { get; set; }

        public List<int> ids { get; set; } = new List<int>();
    }
}