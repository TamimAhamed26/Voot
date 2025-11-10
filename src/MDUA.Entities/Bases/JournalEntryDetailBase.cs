using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "JournalEntryDetailBase", Namespace = "http://www.piistech.com//entities")]
	public class JournalEntryDetailBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			JournalEntryId = 1,
			AccountId = 2,
			Debit = 3,
			Credit = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_JournalEntryId = "JournalEntryId";		            
		public const string Property_AccountId = "AccountId";		            
		public const string Property_Debit = "Debit";		            
		public const string Property_Credit = "Credit";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _JournalEntryId;	            
		private Int32 _AccountId;	            
		private Decimal _Debit;	            
		private Decimal _Credit;	            
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
		public Int32 JournalEntryId
		{	
			get{ return _JournalEntryId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_JournalEntryId, value, _JournalEntryId);
				if (PropertyChanging(args))
				{
					_JournalEntryId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 AccountId
		{	
			get{ return _AccountId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountId, value, _AccountId);
				if (PropertyChanging(args))
				{
					_AccountId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal Debit
		{	
			get{ return _Debit; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Debit, value, _Debit);
				if (PropertyChanging(args))
				{
					_Debit = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal Credit
		{	
			get{ return _Credit; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Credit, value, _Credit);
				if (PropertyChanging(args))
				{
					_Credit = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  JournalEntryDetailBase Clone()
		{
			JournalEntryDetailBase newObj = new  JournalEntryDetailBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.JournalEntryId = this.JournalEntryId;						
			newObj.AccountId = this.AccountId;						
			newObj.Debit = this.Debit;						
			newObj.Credit = this.Credit;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(JournalEntryDetailBase.Property_Id, Id);				
			info.AddValue(JournalEntryDetailBase.Property_JournalEntryId, JournalEntryId);				
			info.AddValue(JournalEntryDetailBase.Property_AccountId, AccountId);				
			info.AddValue(JournalEntryDetailBase.Property_Debit, Debit);				
			info.AddValue(JournalEntryDetailBase.Property_Credit, Credit);				
		}
		#endregion

		
	}
}
