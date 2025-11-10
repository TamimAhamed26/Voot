using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PoReceivedBase", Namespace = "http://www.piistech.com//entities")]
	public class PoReceivedBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			PoRequestedId = 1,
			ReceivedQuantity = 2,
			BuyingPrice = 3,
			ReceivedDate = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_PoRequestedId = "PoRequestedId";		            
		public const string Property_ReceivedQuantity = "ReceivedQuantity";		            
		public const string Property_BuyingPrice = "BuyingPrice";		            
		public const string Property_ReceivedDate = "ReceivedDate";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _PoRequestedId;	            
		private Int32 _ReceivedQuantity;	            
		private Decimal _BuyingPrice;	            
		private DateTime _ReceivedDate;	            
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
		public Int32 PoRequestedId
		{	
			get{ return _PoRequestedId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PoRequestedId, value, _PoRequestedId);
				if (PropertyChanging(args))
				{
					_PoRequestedId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ReceivedQuantity
		{	
			get{ return _ReceivedQuantity; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReceivedQuantity, value, _ReceivedQuantity);
				if (PropertyChanging(args))
				{
					_ReceivedQuantity = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal BuyingPrice
		{	
			get{ return _BuyingPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BuyingPrice, value, _BuyingPrice);
				if (PropertyChanging(args))
				{
					_BuyingPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime ReceivedDate
		{	
			get{ return _ReceivedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReceivedDate, value, _ReceivedDate);
				if (PropertyChanging(args))
				{
					_ReceivedDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PoReceivedBase Clone()
		{
			PoReceivedBase newObj = new  PoReceivedBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.PoRequestedId = this.PoRequestedId;						
			newObj.ReceivedQuantity = this.ReceivedQuantity;						
			newObj.BuyingPrice = this.BuyingPrice;						
			newObj.ReceivedDate = this.ReceivedDate;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PoReceivedBase.Property_Id, Id);				
			info.AddValue(PoReceivedBase.Property_PoRequestedId, PoRequestedId);				
			info.AddValue(PoReceivedBase.Property_ReceivedQuantity, ReceivedQuantity);				
			info.AddValue(PoReceivedBase.Property_BuyingPrice, BuyingPrice);				
			info.AddValue(PoReceivedBase.Property_ReceivedDate, ReceivedDate);				
		}
		#endregion

		
	}
}
