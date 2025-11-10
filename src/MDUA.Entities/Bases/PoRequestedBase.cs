using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PoRequestedBase", Namespace = "http://www.piistech.com//entities")]
	public class PoRequestedBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VendorId = 1,
			ProductId = 2,
			Quantity = 3,
			RequestDate = 4,
			Status = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VendorId = "VendorId";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_Quantity = "Quantity";		            
		public const string Property_RequestDate = "RequestDate";		            
		public const string Property_Status = "Status";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _VendorId;	            
		private Int32 _ProductId;	            
		private Int32 _Quantity;	            
		private Nullable<DateTime> _RequestDate;	            
		private String _Status;	            
		#endregion
		
		#region Properties		
		[DataMember]
		public Int32 Id
		{	
			get{ return _Id; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Id, value, _Id);
				if (PropertyChanging(args))
				{
					_Id = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 VendorId
		{	
			get{ return _VendorId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VendorId, value, _VendorId);
				if (PropertyChanging(args))
				{
					_VendorId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ProductId
		{	
			get{ return _ProductId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductId, value, _ProductId);
				if (PropertyChanging(args))
				{
					_ProductId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 Quantity
		{	
			get{ return _Quantity; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Quantity, value, _Quantity);
				if (PropertyChanging(args))
				{
					_Quantity = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> RequestDate
		{	
			get{ return _RequestDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RequestDate, value, _RequestDate);
				if (PropertyChanging(args))
				{
					_RequestDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Status
		{	
			get{ return _Status; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Status, value, _Status);
				if (PropertyChanging(args))
				{
					_Status = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PoRequestedBase Clone()
		{
			PoRequestedBase newObj = new  PoRequestedBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VendorId = this.VendorId;						
			newObj.ProductId = this.ProductId;						
			newObj.Quantity = this.Quantity;						
			newObj.RequestDate = this.RequestDate;						
			newObj.Status = this.Status;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PoRequestedBase.Property_Id, Id);				
			info.AddValue(PoRequestedBase.Property_VendorId, VendorId);				
			info.AddValue(PoRequestedBase.Property_ProductId, ProductId);				
			info.AddValue(PoRequestedBase.Property_Quantity, Quantity);				
			info.AddValue(PoRequestedBase.Property_RequestDate, RequestDate);				
			info.AddValue(PoRequestedBase.Property_Status, Status);				
		}
		#endregion

		
	}
}
