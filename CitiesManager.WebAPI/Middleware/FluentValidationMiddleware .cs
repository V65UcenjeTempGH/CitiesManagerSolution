using FluentValidation;
using Newtonsoft.Json;
using System.Net;

namespace CitiesManager.WebAPI.Middleware
{
    /// <summary>
    /// 20.07.2023.
    /// ... && _validator is CityUpdateValidator) 
    /// ... && _validator is CityAddValidator)
    /// NE TREBA !!!
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ValidatorMiddleware<TModel> : IMiddleware where TModel : class
    {
        private readonly AbstractValidator<TModel?> _validator;

        public ValidatorMiddleware(AbstractValidator<TModel> validator)
        {
            _validator = validator!;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                // POST ili PUT
                // Validate the request body
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                TModel? model = JsonConvert.DeserializeObject<TModel>(requestBody);
                var validationResult = await _validator.ValidateAsync(model);

                if (!validationResult.IsValid)
                {
                    // Handle validation errors
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(validationResult.Errors));

                    return;
                }
            }

            await next(context);
        }
    }

}
