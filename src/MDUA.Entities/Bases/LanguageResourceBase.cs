using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "LanguageResourceBase", Namespace = "http://www.piistech.com//entities")]
	public class LanguageResourceBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			LKey = 1,
			LValue = 2,
			LanguageId = 3,
			CompanyId = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_LKey = "LKey";		            
		public const string Property_LValue = "LValue";		            
		public const string Property_LanguageId = "LanguageId";		            
		public const string Property_CompanyId = "CompanyId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _LKey;	            
		private String _LValue;	            
		private Int32 _LanguageId;	            
		private Nullable<Int32> _CompanyId;	            
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
		public String LKey
		{	
			get{ return _LKey; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LKey, value, _LKey);
				if (PropertyChanging(args))
				{
					_LKey = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String LValue
		{	
			get{ return _LValue; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LValue, value, _LValue);
				if (PropertyChanging(args))
				{
					_LValue = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 LanguageId
		{	
			get{ return _LanguageId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LanguageId, value, _LanguageId);
				if (PropertyChanging(args))
				{
					_LanguageId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> CompanyId
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
		public  LanguageResourceBase Clone()
		{
			LanguageResourceBase newObj = new  LanguageResourceBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.LKey = this.LKey;						
			newObj.LValue = this.LValue;						
			newObj.LanguageId = this.LanguageId;						
			newObj.CompanyId = this.CompanyId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(LanguageResourceBase.Property_Id, Id);				
			info.AddValue(LanguageResourceBase.Property_LKey, LKey);				
			info.AddValue(LanguageResourceBase.Property_LValue, LValue);				
			info.AddValue(LanguageResourceBase.Property_LanguageId, LanguageId);				
			info.AddValue(LanguageResourceBase.Property_CompanyId, CompanyId);				
		}
		#endregion

		
	}
}
