﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DotVVM.Framework.Hosting;

namespace DotVVM.Framework.ViewModel.Validation
{
    public static class DotvvmRequestContextValidationExtensions
    {
        /// <summary>
        /// Adds a new validation error with the given message and attaches it to the root viewmodel.
        /// </summary>
        /// <param name="message">Validation error message</param>
        public static ViewModelValidationError AddModelError(this IDotvvmRequestContext context, string message)
        {
            var error = new ViewModelValidationError(message)
            {
                IsResolved = true,
                PropertyPath = "/"
            };
            context.ModelState.ErrorsInternal.Add(error);
            return error;
        }

        [Obsolete("Use a different method. Preferably one where you can provide a (lambda) expression. If you really need a manually written property path, use the AddRawModelError(...) method instead.", error: true)]
        public static ViewModelValidationError AddModelError(this IDotvvmRequestContext context, string propertyPath, string message)
        {
            return context.AddRawModelError(propertyPath, message);
        }

        /// <summary>
        /// Adds a new raw validation error. This method is intended only for advanced use-cases and it partially bypasses the validation framework. 
        /// Users of this method must provide an absolute validation path from the root viewmodel. Individual path segments need to be delimited using the '/' character. 
        /// Example 1) /Customer/Id. Example 2) /Items/0/Price.
        /// </summary>
        /// <param name="absolutePath">Absolute validation path from the root viewmodel</param>
        /// <param name="message">Validation error message</param>
        public static ViewModelValidationError AddRawModelError(this IDotvvmRequestContext context, string absolutePath, string message)
        {
            EnsurePathIsRooted(absolutePath);
            var error = new ViewModelValidationError(message)
            {
                IsResolved = true,
                PropertyPath = absolutePath ?? "/"
            };
            context.ModelState.ErrorsInternal.Add(error);
            return error;
        }

        /// <summary>
        /// Adds a new validation error with the given message and attaches it to the property determined by the provided expression. 
        /// The target property must be reachable from the root viewmodel, otherwise the error won't be attached.
        /// </summary>
        /// <param name="vm">Viewmodel or one of its descendants (reachable objects)</param>
        /// <param name="expression">Expression that determines the target property from the provided object</param>
        /// <param name="message">Validation error message</param>
        public static ViewModelValidationError AddModelError<T, TProp>(this IDotvvmRequestContext context, T vm, Expression<Func<T, TProp>> expression, string message)
        {
            var lambdaExpression = (LambdaExpression)expression;
            var propertyPath = ValidationErrorFactory.GetPathFromExpression(context.Configuration, lambdaExpression);

            var error = new ViewModelValidationError(message)
            {
                IsResolved = false,
                TargetObject = vm,
                PropertyPath = propertyPath
            };
            context.ModelState.ErrorsInternal.Add(error);
            return error;
        }

        private static void EnsurePathIsRooted(string propertyPath)
        {
            if (propertyPath != null && !propertyPath.StartsWith("/"))
                throw new ArgumentException("Hand-written paths need to be specified from the root of viewModel! Consider passing an expression (lambda) instead.");
        }
    }
}
