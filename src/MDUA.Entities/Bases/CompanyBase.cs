using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanyBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanyBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyName = 1
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyName = "CompanyName";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _CompanyName;	            
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
		public String CompanyName
		{	
			get{ return _CompanyName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyName, value, _CompanyName);
				if (PropertyChanging(args))
				{
					_CompanyName = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  CompanyBase Clone()
		{
			CompanyBase newObj = new  CompanyBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyName = this.CompanyName;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CompanyBase.Property_Id, Id);				
			info.AddValue(CompanyBase.Property_CompanyName, CompanyName);				
		}
		#endregion

		
	}
}
