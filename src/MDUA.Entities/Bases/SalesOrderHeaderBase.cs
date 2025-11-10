using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "SalesOrderHeaderBase", Namespace = "http://www.piistech.com//entities")]
	public class SalesOrderHeaderBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerId = 1,
			SalesChannelId = 2,
			AddressId = 3,
			OrderDate = 4,
			TotalAmount = 5,
			DiscountAmount = 6,
			NetAmount = 7,
			Status = 8,
			RegisterNumber = 9,
			SessionId = 10,
			IPAddress = 11,
			PaymentGateway = 12,
			GatewayTxnId = 13,
			SalesOrderId = 14,
			OnlineOrderId = 15,
			DirectOrderId = 16
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerId = "CustomerId";		            
		public const string Property_SalesChannelId = "SalesChannelId";		            
		public const string Property_AddressId = "AddressId";		            
		public const string Property_OrderDate = "OrderDate";		            
		public const string Property_TotalAmount = "TotalAmount";		            
		public const string Property_DiscountAmount = "DiscountAmount";		            
		public const string Property_NetAmount = "NetAmount";		            
		public const string Property_Status = "Status";		            
		public const string Property_RegisterNumber = "RegisterNumber";		            
		public const string Property_SessionId = "SessionId";		            
		public const string Property_IPAddress = "IPAddress";		            
		public const string Property_PaymentGateway = "PaymentGateway";		            
		public const string Property_GatewayTxnId = "GatewayTxnId";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_OnlineOrderId = "OnlineOrderId";		            
		public const string Property_DirectOrderId = "DirectOrderId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CustomerId;	            
		private Int32 _SalesChannelId;	            
		private Int32 _AddressId;	            
		private DateTime _OrderDate;	            
		private Decimal _TotalAmount;	            
		private Decimal _DiscountAmount;	            
		private Nullable<Decimal> _NetAmount;	            
		private String _Status;	            
		private String _RegisterNumber;	            
		private String _SessionId;	            
		private String _IPAddress;	            
		private String _PaymentGateway;	            
		private String _GatewayTxnId;	            
		private String _SalesOrderId;	            
		private String _OnlineOrderId;	            
		private String _DirectOrderId;	            
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
		public Int32 SalesChannelId
		{	
			get{ return _SalesChannelId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SalesChannelId, value, _SalesChannelId);
				if (PropertyChanging(args))
				{
					_SalesChannelId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 AddressId
		{	
			get{ return _AddressId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AddressId, value, _AddressId);
				if (PropertyChanging(args))
				{
					_AddressId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime OrderDate
		{	
			get{ return _OrderDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OrderDate, value, _OrderDate);
				if (PropertyChanging(args))
				{
					_OrderDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal TotalAmount
		{	
			get{ return _TotalAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TotalAmount, value, _TotalAmount);
				if (PropertyChanging(args))
				{
					_TotalAmount = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal DiscountAmount
		{	
			get{ return _DiscountAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DiscountAmount, value, _DiscountAmount);
				if (PropertyChanging(args))
				{
					_DiscountAmount = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> NetAmount
		{	
			get{ return _NetAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NetAmount, value, _NetAmount);
				if (PropertyChanging(args))
				{
					_NetAmount = value;
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
		public String RegisterNumber
		{	
			get{ return _RegisterNumber; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RegisterNumber, value, _RegisterNumber);
				if (PropertyChanging(args))
				{
					_RegisterNumber = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SessionId
		{	
			get{ return _SessionId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SessionId, value, _SessionId);
				if (PropertyChanging(args))
				{
					_SessionId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String IPAddress
		{	
			get{ return _IPAddress; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IPAddress, value, _IPAddress);
				if (PropertyChanging(args))
				{
					_IPAddress = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PaymentGateway
		{	
			get{ return _PaymentGateway; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentGateway, value, _PaymentGateway);
				if (PropertyChanging(args))
				{
					_PaymentGateway = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String GatewayTxnId
		{	
			get{ return _GatewayTxnId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_GatewayTxnId, value, _GatewayTxnId);
				if (PropertyChanging(args))
				{
					_GatewayTxnId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SalesOrderId
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
		public String OnlineOrderId
		{	
			get{ return _OnlineOrderId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OnlineOrderId, value, _OnlineOrderId);
				if (PropertyChanging(args))
				{
					_OnlineOrderId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DirectOrderId
		{	
			get{ return _DirectOrderId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DirectOrderId, value, _DirectOrderId);
				if (PropertyChanging(args))
				{
					_DirectOrderId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  SalesOrderHeaderBase Clone()
		{
			SalesOrderHeaderBase newObj = new  SalesOrderHeaderBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerId = this.CustomerId;						
			newObj.SalesChannelId = this.SalesChannelId;						
			newObj.AddressId = this.AddressId;						
			newObj.OrderDate = this.OrderDate;						
			newObj.TotalAmount = this.TotalAmount;						
			newObj.DiscountAmount = this.DiscountAmount;						
			newObj.NetAmount = this.NetAmount;						
			newObj.Status = this.Status;						
			newObj.RegisterNumber = this.RegisterNumber;						
			newObj.SessionId = this.SessionId;						
			newObj.IPAddress = this.IPAddress;						
			newObj.PaymentGateway = this.PaymentGateway;						
			newObj.GatewayTxnId = this.GatewayTxnId;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.OnlineOrderId = this.OnlineOrderId;						
			newObj.DirectOrderId = this.DirectOrderId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SalesOrderHeaderBase.Property_Id, Id);				
			info.AddValue(SalesOrderHeaderBase.Property_CustomerId, CustomerId);				
			info.AddValue(SalesOrderHeaderBase.Property_SalesChannelId, SalesChannelId);				
			info.AddValue(SalesOrderHeaderBase.Property_AddressId, AddressId);				
			info.AddValue(SalesOrderHeaderBase.Property_OrderDate, OrderDate);				
			info.AddValue(SalesOrderHeaderBase.Property_TotalAmount, TotalAmount);				
			info.AddValue(SalesOrderHeaderBase.Property_DiscountAmount, DiscountAmount);				
			info.AddValue(SalesOrderHeaderBase.Property_NetAmount, NetAmount);				
			info.AddValue(SalesOrderHeaderBase.Property_Status, Status);				
			info.AddValue(SalesOrderHeaderBase.Property_RegisterNumber, RegisterNumber);				
			info.AddValue(SalesOrderHeaderBase.Property_SessionId, SessionId);				
			info.AddValue(SalesOrderHeaderBase.Property_IPAddress, IPAddress);				
			info.AddValue(SalesOrderHeaderBase.Property_PaymentGateway, PaymentGateway);				
			info.AddValue(SalesOrderHeaderBase.Property_GatewayTxnId, GatewayTxnId);				
			info.AddValue(SalesOrderHeaderBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(SalesOrderHeaderBase.Property_OnlineOrderId, OnlineOrderId);				
			info.AddValue(SalesOrderHeaderBase.Property_DirectOrderId, DirectOrderId);				
		}
		#endregion

		
	}
}
