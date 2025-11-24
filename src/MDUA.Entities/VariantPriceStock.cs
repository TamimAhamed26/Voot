using System;
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    [Serializable]
    [DataContract(Name = "VariantPriceStock", Namespace = "http://www.piistech.com//entities")]
    public partial class VariantPriceStock : VariantPriceStockBase
    {
        #region Exernal Properties
        private ProductVariant _IdObject = null;

        [DataMember]
        public ProductVariant IdObject
        {
            get { return this._IdObject; }
            set { this._IdObject = value; }
        }
        #endregion

        #region Orverride Equals
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(VariantPriceStock))
            {
                return false;
            }

            VariantPriceStock _paramObj = obj as VariantPriceStock;
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