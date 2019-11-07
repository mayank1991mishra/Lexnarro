using System;
using System.Globalization;
using System.Web.Mvc;

namespace Lexnarro.HelperClasses
{
    public class MyDateTimeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (!string.IsNullOrEmpty(displayFormat) && value != null)
            {
                displayFormat = displayFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);
                // use the format specified in the DisplayFormat attribute to parse the date
                if (DateTime.TryParse(value.AttemptedValue, CultureInfo.GetCultureInfo("en-IE"), DateTimeStyles.None, out DateTime date))
                    return date;
                else
                {
                    bindingContext.ModelState.AddModelError(
                        bindingContext.ModelName,
                        string.Format("{0} is an invalid date format", value.AttemptedValue)
                    );
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}