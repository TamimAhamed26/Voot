using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "LanguageBase", Namespace = "http://www.piistech.com//entities")]
	public class LanguageBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			Name = 1,
			Code = 2
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_Name = "Name";		            
		public const string Property_Code = "Code";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _Name;	            
		private String _Code;	            
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
		public String Name
		{	
			get{ return _Name; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Name, value, _Name);
				if (PropertyChanging(args))
				{
					_Name = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Code
		{	
			get{ return _Code; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Code, value, _Code);
				if (PropertyChanging(args))
				{
					_Code = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  LanguageBase Clone()
		{
			LanguageBase newObj = new  LanguageBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.Name = this.Name;						
			newObj.Code = this.Code;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(LanguageBase.Property_Id, Id);				
			info.AddValue(LanguageBase.Property_Name, Name);				
			info.AddValue(LanguageBase.Property_Code, Code);				
		}
		#endregion

		
	}
}
