using Haondt.Web.DynamicForm.Models;
using Haondt.Web.Views;

namespace Haondt.Web.DynamicForm
{
    public class AlertDynamicFormFactory(
        string title,
        string text,
        string? hxGet = null) : IDynamicFormFactory
    {
        public DynamicFormModel Create()
        {
            var model = new DynamicFormModel
            {
                Title = title,
                HxGet = hxGet,
                Items =
                [
                    new DynamicFormText
                    {
                        Value = text
                    }
                ],
            };

            if (string.IsNullOrEmpty(hxGet))
                model.Buttons =
                [
                    new DynamicFormButton
                    {
                           Text = "ok",
                           Type = DynamicFormButtonType.Button,
                           HyperTrigger = "closeModal"
                    }
                ];
            else
            {
                model.HxGet = hxGet;
                model.Buttons =
                [
                    new DynamicFormButton
                    {
                           Text = "ok",
                           Type = DynamicFormButtonType.Submit
                    }
                ];
            }

            return model;
        }
    }
}
