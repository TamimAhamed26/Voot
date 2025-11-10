using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "UserLoginBase", Namespace = "http://www.piistech.com//entities")]
	public class UserLoginBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			UserName = 1,
			Email = 2,
			Phone = 3,
			Password = 4,
			CompanyId = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_UserName = "UserName";		            
		public const string Property_Email = "Email";		            
		public const string Property_Phone = "Phone";		            
		public const string Property_Password = "Password";		            
		public const string Property_CompanyId = "CompanyId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _UserName;	            
		private String _Email;	            
		private String _Phone;	            
		private String _Password;	            
		private Int32 _CompanyId;	            
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
		public String UserName
		{	
			get{ return _UserName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UserName, value, _UserName);
				if (PropertyChanging(args))
				{
					_UserName = value;
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
		public String Password
		{	
			get{ return _Password; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Password, value, _Password);
				if (PropertyChanging(args))
				{
					_Password = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  UserLoginBase Clone()
		{
			UserLoginBase newObj = new  UserLoginBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.UserName = this.UserName;						
			newObj.Email = this.Email;						
			newObj.Phone = this.Phone;						
			newObj.Password = this.Password;						
			newObj.CompanyId = this.CompanyId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(UserLoginBase.Property_Id, Id);				
			info.AddValue(UserLoginBase.Property_UserName, UserName);				
			info.AddValue(UserLoginBase.Property_Email, Email);				
			info.AddValue(UserLoginBase.Property_Phone, Phone);				
			info.AddValue(UserLoginBase.Property_Password, Password);				
			info.AddValue(UserLoginBase.Property_CompanyId, CompanyId);				
		}
		#endregion

		
	}
}
