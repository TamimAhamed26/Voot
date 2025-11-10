using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "DeliveryBase", Namespace = "http://www.piistech.com//entities")]
	public class DeliveryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SalesOrderId = 1,
			DeliveryDate = 2,
			TrackingNumber = 3,
			Status = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_DeliveryDate = "DeliveryDate";		            
		public const string Property_TrackingNumber = "TrackingNumber";		            
		public const string Property_Status = "Status";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _SalesOrderId;	            
		private DateTime _DeliveryDate;	            
		private String _TrackingNumber;	            
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
		public Int32 SalesOrderId
		{	
			get{ return _SalesOrderId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SalesOrderId, value, _SalesOrderId);
				if (PropertyChanging(args))
				{
					_SalesOrderId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime DeliveryDate
		{	
			get{ return _DeliveryDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DeliveryDate, value, _DeliveryDate);
				if (PropertyChanging(args))
				{
					_DeliveryDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String TrackingNumber
		{	
			get{ return _TrackingNumber; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TrackingNumber, value, _TrackingNumber);
				if (PropertyChanging(args))
				{
					_TrackingNumber = value;
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
		public  DeliveryBase Clone()
		{
			DeliveryBase newObj = new  DeliveryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.DeliveryDate = this.DeliveryDate;						
			newObj.TrackingNumber = this.TrackingNumber;						
			newObj.Status = this.Status;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(DeliveryBase.Property_Id, Id);				
			info.AddValue(DeliveryBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(DeliveryBase.Property_DeliveryDate, DeliveryDate);				
			info.AddValue(DeliveryBase.Property_TrackingNumber, TrackingNumber);				
			info.AddValue(DeliveryBase.Property_Status, Status);				
		}
		#endregion

		
	}
}
