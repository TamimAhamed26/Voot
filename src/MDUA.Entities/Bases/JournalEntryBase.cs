using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "JournalEntryBase", Namespace = "http://www.piistech.com//entities")]
	public class JournalEntryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ReferenceType = 1,
			ReferenceId = 2,
			EntryDate = 3,
			Description = 4,
			CreatedBy = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ReferenceType = "ReferenceType";		            
		public const string Property_ReferenceId = "ReferenceId";		            
		public const string Property_EntryDate = "EntryDate";		            
		public const string Property_Description = "Description";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _ReferenceType;	            
		private Nullable<Int32> _ReferenceId;	            
		private DateTime _EntryDate;	            
		private String _Description;	            
		private String _CreatedBy;	            
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
		public String ReferenceType
		{	
			get{ return _ReferenceType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReferenceType, value, _ReferenceType);
				if (PropertyChanging(args))
				{
					_ReferenceType = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> ReferenceId
		{	
			get{ return _ReferenceId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReferenceId, value, _ReferenceId);
				if (PropertyChanging(args))
				{
					_ReferenceId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime EntryDate
		{	
			get{ return _EntryDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EntryDate, value, _EntryDate);
				if (PropertyChanging(args))
				{
					_EntryDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Description
		{	
			get{ return _Description; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Description, value, _Description);
				if (PropertyChanging(args))
				{
					_Description = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CreatedBy
		{	
			get{ return _CreatedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedBy, value, _CreatedBy);
				if (PropertyChanging(args))
				{
					_CreatedBy = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  JournalEntryBase Clone()
		{
			JournalEntryBase newObj = new  JournalEntryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ReferenceType = this.ReferenceType;						
			newObj.ReferenceId = this.ReferenceId;						
			newObj.EntryDate = this.EntryDate;						
			newObj.Description = this.Description;						
			newObj.CreatedBy = this.CreatedBy;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(JournalEntryBase.Property_Id, Id);				
			info.AddValue(JournalEntryBase.Property_ReferenceType, ReferenceType);				
			info.AddValue(JournalEntryBase.Property_ReferenceId, ReferenceId);				
			info.AddValue(JournalEntryBase.Property_EntryDate, EntryDate);				
			info.AddValue(JournalEntryBase.Property_Description, Description);				
			info.AddValue(JournalEntryBase.Property_CreatedBy, CreatedBy);				
		}
		#endregion

		
	}
}
