using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CustomerPaymentBase", Namespace = "http://www.piistech.com//entities")]
	public class CustomerPaymentBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerId = 1,
			PaymentMethodId = 2,
			InventoryTransactionId = 3,
			ReferenceNumber = 4,
			PaymentType = 5,
			Amount = 6,
			PaymentDate = 7,
			Status = 8,
			Notes = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerId = "CustomerId";		            
		public const string Property_PaymentMethodId = "PaymentMethodId";		            
		public const string Property_InventoryTransactionId = "InventoryTransactionId";		            
		public const string Property_ReferenceNumber = "ReferenceNumber";		            
		public const string Property_PaymentType = "PaymentType";		            
		public const string Property_Amount = "Amount";		            
		public const string Property_PaymentDate = "PaymentDate";		            
		public const string Property_Status = "Status";		            
		public const string Property_Notes = "Notes";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CustomerId;	            
		private Int32 _PaymentMethodId;	            
		private Nullable<Int32> _InventoryTransactionId;	            
		private String _ReferenceNumber;	            
		private String _PaymentType;	            
		private Decimal _Amount;	            
		private DateTime _PaymentDate;	            
		private String _Status;	            
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
		public Int32 CustomerId
		{	
			get{ return _CustomerId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerId, value, _CustomerId);
				if (PropertyChanging(args))
				{
					_CustomerId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 PaymentMethodId
		{	
			get{ return _PaymentMethodId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentMethodId, value, _PaymentMethodId);
				if (PropertyChanging(args))
				{
					_PaymentMethodId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> InventoryTransactionId
		{	
			get{ return _InventoryTransactionId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_InventoryTransactionId, value, _InventoryTransactionId);
				if (PropertyChanging(args))
				{
					_InventoryTransactionId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ReferenceNumber
		{	
			get{ return _ReferenceNumber; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReferenceNumber, value, _ReferenceNumber);
				if (PropertyChanging(args))
				{
					_ReferenceNumber = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PaymentType
		{	
			get{ return _PaymentType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentType, value, _PaymentType);
				if (PropertyChanging(args))
				{
					_PaymentType = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal Amount
		{	
			get{ return _Amount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Amount, value, _Amount);
				if (PropertyChanging(args))
				{
					_Amount = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime PaymentDate
		{	
			get{ return _PaymentDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentDate, value, _PaymentDate);
				if (PropertyChanging(args))
				{
					_PaymentDate = value;
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
		public  CustomerPaymentBase Clone()
		{
			CustomerPaymentBase newObj = new  CustomerPaymentBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerId = this.CustomerId;						
			newObj.PaymentMethodId = this.PaymentMethodId;						
			newObj.InventoryTransactionId = this.InventoryTransactionId;						
			newObj.ReferenceNumber = this.ReferenceNumber;						
			newObj.PaymentType = this.PaymentType;						
			newObj.Amount = this.Amount;						
			newObj.PaymentDate = this.PaymentDate;						
			newObj.Status = this.Status;						
			newObj.Notes = this.Notes;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CustomerPaymentBase.Property_Id, Id);				
			info.AddValue(CustomerPaymentBase.Property_CustomerId, CustomerId);				
			info.AddValue(CustomerPaymentBase.Property_PaymentMethodId, PaymentMethodId);				
			info.AddValue(CustomerPaymentBase.Property_InventoryTransactionId, InventoryTransactionId);				
			info.AddValue(CustomerPaymentBase.Property_ReferenceNumber, ReferenceNumber);				
			info.AddValue(CustomerPaymentBase.Property_PaymentType, PaymentType);				
			info.AddValue(CustomerPaymentBase.Property_Amount, Amount);				
			info.AddValue(CustomerPaymentBase.Property_PaymentDate, PaymentDate);				
			info.AddValue(CustomerPaymentBase.Property_Status, Status);				
			info.AddValue(CustomerPaymentBase.Property_Notes, Notes);				
		}
		#endregion

		
	}
}
