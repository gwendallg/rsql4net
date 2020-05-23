using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RSql4Net.Tests.Models
{
    public class MockModelBinderProviderContext : ModelBinderProviderContext
    {
        public MockModelBinderProviderContext(Type type)
        {
            Metadata = new MockModelMetadata(type);
        }

        public override BindingInfo BindingInfo { get; }
        public override ModelMetadata Metadata { get; }
        public override IModelMetadataProvider MetadataProvider { get; }

        public override IModelBinder CreateBinder(ModelMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
