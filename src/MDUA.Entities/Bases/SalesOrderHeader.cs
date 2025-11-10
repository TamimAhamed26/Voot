using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "SalesOrderHeader", Namespace = "http://www.piistech.com//entities")]
	public partial class SalesOrderHeader : SalesOrderHeaderBase
	{
		#region Exernal Properties
		private Address _AddressIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Address"/>.
		/// </summary>
		/// <value>The source Address for _AddressIdObject.</value>
		[DataMember]
		public Address AddressIdObject
      	{
            get { return this._AddressIdObject; }
            set { this._AddressIdObject = value; }
      	}
		
		private SalesChannel _SalesChannelIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SalesChannel"/>.
		/// </summary>
		/// <value>The source SalesChannel for _SalesChannelIdObject.</value>
		[DataMember]
		public SalesChannel SalesChannelIdObject
      	{
            get { return this._SalesChannelIdObject; }
            set { this._SalesChannelIdObject = value; }
      	}
		
		private Customer _CustomerIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Customer"/>.
		/// </summary>
		/// <value>The source Customer for _CustomerIdObject.</value>
		[DataMember]
		public Customer CustomerIdObject
      	{
            get { return this._CustomerIdObject; }
            set { this._CustomerIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(SalesOrderHeader))
            {
                return false;
            }			
			
			 SalesOrderHeader _paramObj = obj as SalesOrderHeader;
            if (_paramObj != null)
            {			
                return (_paramObj.Id == this.Id && _paramObj.CustomPropertyMatch(this));
            }
            else
            {
                return base.Equals(obj);
            }
		}
		#endregion
		
		#region Orverride HashCode
		 public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
		#endregion		
	}
}
