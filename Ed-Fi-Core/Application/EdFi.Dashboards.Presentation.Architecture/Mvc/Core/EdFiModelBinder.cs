using System;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core
{
    /// <summary>
    /// Override for DefaultModelBinder in order to implement fixes to its behavior.
    /// </summary>
    public class EdFiModelBinder : DefaultModelBinderWithAliasSupport
    {
        private static readonly JavaScriptSerializer jss = new JavaScriptSerializer { MaxJsonLength = 50000000 };
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelName == "request")
            {
                var json = controllerContext.HttpContext.Request["json"];
                if (!String.IsNullOrEmpty(json))
                {
                    var o = jss.Deserialize(json, bindingContext.ModelType);
                    bindingContext.ModelMetadata.Model = o;
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }

        /// <summary>
        /// Fix for the default model binder's failure to decode enum types when binding to JSON.
        /// </summary>
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var propertyType = propertyDescriptor.PropertyType;

            if (propertyType == typeof (decimal))
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                decimal value;
                if (valueProviderResult != null && Decimal.TryParse(valueProviderResult.AttemptedValue, out value))
                {
                    return value;
                }
            }


            if (propertyType.IsEnum)
            {
                if (bindingContext.ModelType == propertyType)
                    return bindingContext.Model;

                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (null != providerValue)
                {
                    var value = providerValue.RawValue;
                    if (null != value)
                    {
                        var valueType = value.GetType();
                        if (!valueType.IsEnum)
                        {
                            return Enum.ToObject(propertyType, value);
                        }
                    }
                }
            }

            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}