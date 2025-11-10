using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "AddressBase", Namespace = "http://www.piistech.com//entities")]
	public class AddressBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerId = 1,
			Street = 2,
			City = 3,
			Divison = 4,
			PostalCode = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerId = "CustomerId";		            
		public const string Property_Street = "Street";		            
		public const string Property_City = "City";		            
		public const string Property_Divison = "Divison";		            
		public const string Property_PostalCode = "PostalCode";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CustomerId;	            
		private String _Street;	            
		private String _City;	            
		private String _Divison;	            
		private String _PostalCode;	            
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
		public String Street
		{	
			get{ return _Street; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Street, value, _Street);
				if (PropertyChanging(args))
				{
					_Street = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String City
		{	
			get{ return _City; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_City, value, _City);
				if (PropertyChanging(args))
				{
					_City = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Divison
		{	
			get{ return _Divison; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Divison, value, _Divison);
				if (PropertyChanging(args))
				{
					_Divison = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PostalCode
		{	
			get{ return _PostalCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PostalCode, value, _PostalCode);
				if (PropertyChanging(args))
				{
					_PostalCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  AddressBase Clone()
		{
			AddressBase newObj = new  AddressBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerId = this.CustomerId;						
			newObj.Street = this.Street;						
			newObj.City = this.City;						
			newObj.Divison = this.Divison;						
			newObj.PostalCode = this.PostalCode;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(AddressBase.Property_Id, Id);				
			info.AddValue(AddressBase.Property_CustomerId, CustomerId);				
			info.AddValue(AddressBase.Property_Street, Street);				
			info.AddValue(AddressBase.Property_City, City);				
			info.AddValue(AddressBase.Property_Divison, Divison);				
			info.AddValue(AddressBase.Property_PostalCode, PostalCode);				
		}
		#endregion

		
	}
}
