using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TransactionService.Extensions
{
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue(typeof(T).Name, out var value))
            {
                var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();
                var result = await validator.ValidateAsync((T)value);
                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(result.ToDictionary());
                    return;
                }
            }
            await next();
        }
    }
}
