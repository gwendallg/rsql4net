using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RSql4Net.Samples.Models
{
    public class ErrorModel
    {
        public ErrorModel(ModelStateDictionary modelState)
        {
            Messages = new List<string>();
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            foreach (var error in modelState.Values.SelectMany(item => item.Errors))
            {
                Messages.Add(error.ErrorMessage);
            }
        }

        public List<string> Messages { get; set; }
    }
}
