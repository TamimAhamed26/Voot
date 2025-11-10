using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "VendorBase", Namespace = "http://www.piistech.com//entities")]
	public class VendorBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VendorName = 1,
			Email = 2,
			Phone = 3
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VendorName = "VendorName";		            
		public const string Property_Email = "Email";		            
		public const string Property_Phone = "Phone";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _VendorName;	            
		private String _Email;	            
		private String _Phone;	            
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
		public String VendorName
		{	
			get{ return _VendorName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VendorName, value, _VendorName);
				if (PropertyChanging(args))
				{
					_VendorName = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  VendorBase Clone()
		{
			VendorBase newObj = new  VendorBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VendorName = this.VendorName;						
			newObj.Email = this.Email;						
			newObj.Phone = this.Phone;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(VendorBase.Property_Id, Id);				
			info.AddValue(VendorBase.Property_VendorName, VendorName);				
			info.AddValue(VendorBase.Property_Email, Email);				
			info.AddValue(VendorBase.Property_Phone, Phone);				
		}
		#endregion

		
	}
}
