using System;
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    [Serializable]
    [DataContract(Name = "ProductAttribute", Namespace = "http://www.piistech.com//entities")]
    public partial class ProductAttribute : ProductAttributeBase
    {
        #region Exernal Properties
        private Product? _ProductIdObject = null; 
        private AttributeName? _AttributeIdObject = null;

        [DataMember]
        public Product? ProductIdObject 
        {
            get { return this._ProductIdObject; }
            set { this._ProductIdObject = value; }
        }

        [DataMember]
        public AttributeName? AttributeIdObject 
        {
            get { return this._AttributeIdObject; }
            set { this._AttributeIdObject = value; }
        }
        #endregion

        #region Orverride Equals
        public override bool Equals(object? obj) // <-- FIX: Made object nullable
        {
            if (obj == null || obj.GetType() != typeof(ProductAttribute))
            {
                return false;
            }

            ProductAttribute? _paramObj = obj as ProductAttribute; // <-- FIX: Made nullable
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