using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CustomerBase", Namespace = "http://www.piistech.com//entities")]
	public class CustomerBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerName = 1,
			Email = 2,
			Phone = 3,
			IsActive = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerName = "CustomerName";		            
		public const string Property_Email = "Email";		            
		public const string Property_Phone = "Phone";		            
		public const string Property_IsActive = "IsActive";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _CustomerName;	            
		private String _Email;	            
		private String _Phone;	            
		private Boolean _IsActive;	            
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
		public String CustomerName
		{	
			get{ return _CustomerName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerName, value, _CustomerName);
				if (PropertyChanging(args))
				{
					_CustomerName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Email
		{	
			get{ return _Email; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Email, value, _Email);
				if (PropertyChanging(args))
				{
					_Email = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Phone
		{	
			get{ return _Phone; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Phone, value, _Phone);
				if (PropertyChanging(args))
				{
					_Phone = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsActive
		{	
			get{ return _IsActive; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsActive, value, _IsActive);
				if (PropertyChanging(args))
				{
					_IsActive = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  CustomerBase Clone()
		{
			CustomerBase newObj = new  CustomerBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerName = this.CustomerName;						
			newObj.Email = this.Email;						
			newObj.Phone = this.Phone;						
			newObj.IsActive = this.IsActive;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CustomerBase.Property_Id, Id);				
			info.AddValue(CustomerBase.Property_CustomerName, CustomerName);				
			info.AddValue(CustomerBase.Property_Email, Email);				
			info.AddValue(CustomerBase.Property_Phone, Phone);				
			info.AddValue(CustomerBase.Property_IsActive, IsActive);				
		}
		#endregion

		
	}
}
