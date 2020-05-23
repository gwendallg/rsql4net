using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RSql4Net.Tests.Models
{
    public class MockModelBindingContext : ModelBindingContext
    {
        public override ActionContext ActionContext { get; set; }
        public override string BinderModelName { get; set; }
        public override BindingSource BindingSource { get; set; }
        public override string FieldName { get; set; }
        public override bool IsTopLevelObject { get; set; }
        public override object Model { get; set; }
        public override ModelMetadata ModelMetadata { get; set; }
        public override string ModelName { get; set; }
        public override ModelStateDictionary ModelState { get; set; }
        public override Func<ModelMetadata, bool> PropertyFilter { get; set; }
        public override ValidationStateDictionary ValidationState { get; set; }
        public override IValueProvider ValueProvider { get; set; }
        public override ModelBindingResult Result { get; set; }

        public override NestedScope EnterNestedScope(ModelMetadata modelMetadata, string fieldName, string modelName,
            object model)
        {
            throw new NotImplementedException();
        }

        public override NestedScope EnterNestedScope()
        {
            throw new NotImplementedException();
        }

        protected override void ExitNestedScope()
        {
            throw new NotImplementedException();
        }
    }
}
