﻿using Haondt.Web.DynamicForm.Models;
using Haondt.Web.Views;

namespace Haondt.Web.Authentication.Pages
{
    public class RegisterDynamicFormFactory(
        string username,
        string? usernameError = null,
        string? passwordError = null) : IDynamicFormFactory
    {
        public DynamicFormModel Create()
        {
            return new DynamicFormModel
            {
                Title = "register",
                HxPost = "account/register",
                Style = "width: 80%; max-width: 250px",
                Items =
                [
                    new DynamicFormInput
                    {
                        Name = "username",
                        Type = DynamicFormInputType.Text,
                        Autocomplete = "username",
                        Value = username,
                        Label = "username",
                        Error = usernameError
                    },
                    new DynamicFormInput
                    {
                        Name = "password",
                        Type = DynamicFormInputType.Password,
                        Autocomplete = "current-password",
                        Label = "password",
                        Error = passwordError
                    }
                ],
                Buttons =
                [
                    new DynamicFormButton
                    {
                           Text = "submit",
                           Type = DynamicFormButtonType.Submit
                    },
                    new DynamicFormButton
                    {
                           Text = "use existing account",
                           Type = DynamicFormButtonType.Button,
                           HxGet = "partials/login"
                    }
                ],

            };
        }
    }
}
