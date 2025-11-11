using System;
using System.Runtime.Serialization;
using MDUA.Framework;

namespace MDUA.Entities.List
{
    [Serializable]
    [DataContract(Name = "VariantAttributeValueList", Namespace = "http://www.piistech.com//entities")]
    public class VariantAttributeValueList : BaseCollection<VariantAttributeValue>
    {
    }
}