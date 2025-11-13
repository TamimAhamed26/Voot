using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System.Net.NetworkInformation;

namespace MDUA.Entities
{
	public partial class Permission 
	{
		public  class Product
		{
			public const int View = 1001;
			public const int Create = 1002;
			public const int Edit = 1003;
			public const int Delete = 1004;




        }
		
	}
}
