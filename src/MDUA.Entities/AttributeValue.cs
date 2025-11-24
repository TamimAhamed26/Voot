using System;
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    [Serializable]
    [DataContract(Name = "AttributeValue", Namespace = "http://www.piistech.com//entities")]
    public partial class AttributeValue : AttributeValueBase
    {
        #region Exernal Properties
        private AttributeName? _AttributeIdObject = null; // <-- FIX: Made nullable

        [DataMember]
        public AttributeName? AttributeIdObject // <-- FIX: Made nullable
        {
            get { return this._AttributeIdObject; }
            set { this._AttributeIdObject = value; }
        }
        #endregion

        #region Orverride Equals
        public override bool Equals(object? obj) // <-- FIX: Made object nullable
        {
            if (obj == null || obj.GetType() != typeof(AttributeValue))
            {
                return false;
            }

            AttributeValue? _paramObj = obj as AttributeValue; // <-- FIX: Made nullable
            if (_paramObj != null)
            {
                return (_paramObj.Id == this.Id && _paramObj.CustomPropertyMatch(this));
            }
            else
            {
                return base.Equals(obj);
            }
        }
        #endregion

        #region Orverride HashCode
        public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
        #endregion
    }
}