using Firefly_pp_Runner.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.ModelBinders
{
    public class LookupActionEnumModelBinder<T> : IModelBinder where T : Enum
    {
        private static JsonSerializerSettings? _jsonSerializerSettingsInstance;
        private static JsonSerializerSettings _jsonSerializerSettings { get
            {
                if (_jsonSerializerSettingsInstance == null)
                {
                    _jsonSerializerSettingsInstance = new JsonSerializerSettings();
                    _jsonSerializerSettingsInstance.ConfigureFireflyppRunnerSettings();
                }
                return _jsonSerializerSettingsInstance;
            } }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if(valueProviderResult.Length == 0)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No value was provided for enum");
                return Task.CompletedTask;
            }

            var stringvalue = valueProviderResult.FirstValue;
            try
            {
                bindingContext.Model = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(stringvalue, _jsonSerializerSettings), _jsonSerializerSettings);
                bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            }
            catch (JsonSerializationException ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
