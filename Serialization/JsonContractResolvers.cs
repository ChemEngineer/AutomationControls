using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace AutomationControls.Serialization
{
    public class JsonContractResolvers
    {
        public class AutomationControlsContractResolver : DefaultContractResolver
        {
            public AutomationControlsContractResolver()
            {
                this.IgnoreSerializableAttribute = false;
                this.IgnoreSerializableInterface = false;
            }

            protected override JsonContract CreateContract(Type objectType)
            {
                if (typeof(ISerializable).IsAssignableFrom(objectType))
                    return CreateISerializableContract(objectType);

                return base.CreateContract(objectType);
            }
        }

        public class ISerializableCollectionContractResolver : DefaultContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                var contract = base.CreateContract(objectType);
                var underlyingType = Nullable.GetUnderlyingType(objectType) ?? objectType;

                if (!IgnoreSerializableInterface
                    && typeof(ISerializable).IsAssignableFrom(underlyingType)
                    && contract is JsonArrayContract
                    && !underlyingType.GetCustomAttributes(typeof(JsonContainerAttribute), true).Any())
                {
                    contract = CreateISerializableContract(objectType);
                }

                return contract;
            }
        }
    }
}
