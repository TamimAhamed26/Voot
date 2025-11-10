using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PaymentAllocationBase", Namespace = "http://www.piistech.com//entities")]
	public class PaymentAllocationBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerPaymentId = 1,
			SalesOrderId = 2,
			AllocatedAmount = 3,
			AllocatedDate = 4,
			Notes = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerPaymentId = "CustomerPaymentId";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_AllocatedAmount = "AllocatedAmount";		            
		public const string Property_AllocatedDate = "AllocatedDate";		            
		public const string Property_Notes = "Notes";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CustomerPaymentId;	            
		private Nullable<Int32> _SalesOrderId;	            
		private Decimal _AllocatedAmount;	            
		private DateTime _AllocatedDate;	            
		private String _Notes;	            
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
		public Int32 CustomerPaymentId
		{	
			get{ return _CustomerPaymentId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerPaymentId, value, _CustomerPaymentId);
				if (PropertyChanging(args))
				{
					_CustomerPaymentId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> SalesOrderId
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
		public Decimal AllocatedAmount
		{	
			get{ return _AllocatedAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AllocatedAmount, value, _AllocatedAmount);
				if (PropertyChanging(args))
				{
					_AllocatedAmount = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime AllocatedDate
		{	
			get{ return _AllocatedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AllocatedDate, value, _AllocatedDate);
				if (PropertyChanging(args))
				{
					_AllocatedDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Notes
		{	
			get{ return _Notes; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Notes, value, _Notes);
				if (PropertyChanging(args))
				{
					_Notes = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PaymentAllocationBase Clone()
		{
			PaymentAllocationBase newObj = new  PaymentAllocationBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerPaymentId = this.CustomerPaymentId;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.AllocatedAmount = this.AllocatedAmount;						
			newObj.AllocatedDate = this.AllocatedDate;						
			newObj.Notes = this.Notes;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PaymentAllocationBase.Property_Id, Id);				
			info.AddValue(PaymentAllocationBase.Property_CustomerPaymentId, CustomerPaymentId);				
			info.AddValue(PaymentAllocationBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(PaymentAllocationBase.Property_AllocatedAmount, AllocatedAmount);				
			info.AddValue(PaymentAllocationBase.Property_AllocatedDate, AllocatedDate);				
			info.AddValue(PaymentAllocationBase.Property_Notes, Notes);				
		}
		#endregion

		
	}
}
