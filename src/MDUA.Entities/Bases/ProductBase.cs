using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductName = 1,
			ReorderLevel = 2,
			CompanyId = 3,
			PriceMultiplier = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductName = "ProductName";		            
		public const string Property_ReorderLevel = "ReorderLevel";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_PriceMultiplier = "PriceMultiplier";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _ProductName;	            
		private Int32 _ReorderLevel;	            
		private Int32 _CompanyId;	            
		private Decimal _PriceMultiplier;	            
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
		public String ProductName
		{	
			get{ return _ProductName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductName, value, _ProductName);
				if (PropertyChanging(args))
				{
					_ProductName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ReorderLevel
		{	
			get{ return _ReorderLevel; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReorderLevel, value, _ReorderLevel);
				if (PropertyChanging(args))
				{
					_ReorderLevel = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 CompanyId
		{	
			get{ return _CompanyId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyId, value, _CompanyId);
				if (PropertyChanging(args))
				{
					_CompanyId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal PriceMultiplier
		{	
			get{ return _PriceMultiplier; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PriceMultiplier, value, _PriceMultiplier);
				if (PropertyChanging(args))
				{
					_PriceMultiplier = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  ProductBase Clone()
		{
			ProductBase newObj = new  ProductBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductName = this.ProductName;						
			newObj.ReorderLevel = this.ReorderLevel;						
			newObj.CompanyId = this.CompanyId;						
			newObj.PriceMultiplier = this.PriceMultiplier;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ProductBase.Property_Id, Id);				
			info.AddValue(ProductBase.Property_ProductName, ProductName);				
			info.AddValue(ProductBase.Property_ReorderLevel, ReorderLevel);				
			info.AddValue(ProductBase.Property_CompanyId, CompanyId);				
			info.AddValue(ProductBase.Property_PriceMultiplier, PriceMultiplier);				
		}
		#endregion

		
	}
}
