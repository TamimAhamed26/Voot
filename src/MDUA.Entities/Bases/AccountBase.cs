using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "AccountBase", Namespace = "http://www.piistech.com//entities")]
	public class AccountBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			AccountCode = 1,
			AccountName = 2,
			AccountTypeId = 3,
			IsActive = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_AccountCode = "AccountCode";		            
		public const string Property_AccountName = "AccountName";		            
		public const string Property_AccountTypeId = "AccountTypeId";		            
		public const string Property_IsActive = "IsActive";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _AccountCode;	            
		private String _AccountName;	            
		private Int32 _AccountTypeId;	            
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
		public String AccountCode
		{	
			get{ return _AccountCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountCode, value, _AccountCode);
				if (PropertyChanging(args))
				{
					_AccountCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String AccountName
		{	
			get{ return _AccountName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountName, value, _AccountName);
				if (PropertyChanging(args))
				{
					_AccountName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 AccountTypeId
		{	
			get{ return _AccountTypeId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountTypeId, value, _AccountTypeId);
				if (PropertyChanging(args))
				{
					_AccountTypeId = value;
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
		public  AccountBase Clone()
		{
			AccountBase newObj = new  AccountBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.AccountCode = this.AccountCode;						
			newObj.AccountName = this.AccountName;						
			newObj.AccountTypeId = this.AccountTypeId;						
			newObj.IsActive = this.IsActive;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(AccountBase.Property_Id, Id);				
			info.AddValue(AccountBase.Property_AccountCode, AccountCode);				
			info.AddValue(AccountBase.Property_AccountName, AccountName);				
			info.AddValue(AccountBase.Property_AccountTypeId, AccountTypeId);				
			info.AddValue(AccountBase.Property_IsActive, IsActive);				
		}
		#endregion

		
	}
}
