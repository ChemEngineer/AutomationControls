using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

namespace AutomationControls.Serialization
{
    public class JsonContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if (typeof(ISerializable).IsAssignableFrom(objectType))
                return CreateISerializableContract(objectType);

            return base.CreateContract(objectType);
        }
    }
}
