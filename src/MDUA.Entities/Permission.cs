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
			public const int View = 1;
			public const int Create = 2;
			public const int Edit = 3;
			public const int Delete = 4;

			public const int Category_View = 20;
			public const int Category_Create = 21;
			public const int Category_Edit = 22;
			public const int Category_Delete = 23;




        }
		
	}
}
