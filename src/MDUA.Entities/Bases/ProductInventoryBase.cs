using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductInventoryBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductInventoryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			CurrentStock = 2,
			AverageCost = 3,
			UpdatedAt = 4,
			SuggestedSellingPrice = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_CurrentStock = "CurrentStock";		            
		public const string Property_AverageCost = "AverageCost";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_SuggestedSellingPrice = "SuggestedSellingPrice";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private Int32 _CurrentStock;	            
		private Decimal _AverageCost;	            
		private DateTime _UpdatedAt;	            
		private Decimal _SuggestedSellingPrice;	            
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
		public Int32 CurrentStock
		{	
			get{ return _CurrentStock; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CurrentStock, value, _CurrentStock);
				if (PropertyChanging(args))
				{
					_CurrentStock = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal AverageCost
		{	
			get{ return _AverageCost; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AverageCost, value, _AverageCost);
				if (PropertyChanging(args))
				{
					_AverageCost = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime UpdatedAt
		{	
			get{ return _UpdatedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedAt, value, _UpdatedAt);
				if (PropertyChanging(args))
				{
					_UpdatedAt = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal SuggestedSellingPrice
		{	
			get{ return _SuggestedSellingPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SuggestedSellingPrice, value, _SuggestedSellingPrice);
				if (PropertyChanging(args))
				{
					_SuggestedSellingPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  ProductInventoryBase Clone()
		{
			ProductInventoryBase newObj = new  ProductInventoryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.CurrentStock = this.CurrentStock;						
			newObj.AverageCost = this.AverageCost;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.SuggestedSellingPrice = this.SuggestedSellingPrice;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ProductInventoryBase.Property_Id, Id);				
			info.AddValue(ProductInventoryBase.Property_ProductId, ProductId);				
			info.AddValue(ProductInventoryBase.Property_CurrentStock, CurrentStock);				
			info.AddValue(ProductInventoryBase.Property_AverageCost, AverageCost);				
			info.AddValue(ProductInventoryBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(ProductInventoryBase.Property_SuggestedSellingPrice, SuggestedSellingPrice);				
		}
		#endregion

		
	}
}
