using System;
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    [Serializable]
    [DataContract(Name = "VariantAttributeValue", Namespace = "http://www.piistech.com//entities")]
    public partial class VariantAttributeValue : VariantAttributeValueBase
    {
        #region Exernal Properties
        private ProductVariant _VariantIdObject = null;
        private AttributeName _AttributeIdObject = null;
        private AttributeValue _AttributeValueIdObject = null;

        [DataMember]
        public ProductVariant VariantIdObject
        {
            get { return this._VariantIdObject; }
            set { this._VariantIdObject = value; }
        }

        [DataMember]
        public AttributeName AttributeIdObject
        {
            get { return this._AttributeIdObject; }
            set { this._AttributeIdObject = value; }
        }

        [DataMember]
        public AttributeValue AttributeValueIdObject
        {
            get { return this._AttributeValueIdObject; }
            set { this._AttributeValueIdObject = value; }
        }
        #endregion

        #region Orverride Equals
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(VariantAttributeValue))
            {
                return false;
            }

            VariantAttributeValue _paramObj = obj as VariantAttributeValue;
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