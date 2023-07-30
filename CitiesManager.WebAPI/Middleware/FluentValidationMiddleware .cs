using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
using System.Net;

using System.Text.Json;


namespace CitiesManager.WebAPI.Middleware
{
    /// <summary>
    /// 20.07.2023.
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
                // POST or PUT
                // Validate the request body

                //using var reader = new StreamReader(context.Request.Body);
                //var requestBody = await reader.ReadToEndAsync();

                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                //if (!IsValidJson(requestBody))
                //{
                //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    context.Response.ContentType = "application/json";
                //    await context.Response.WriteAsync("Invalid JSON in the request body.");
                //    return;
                //}

                if (string.IsNullOrEmpty(requestBody))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("Request body is empty.");
                    return;
                }

                TModel? model = JsonConvert.DeserializeObject<TModel?>(requestBody);

                var validationResult = await _validator.ValidateAsync(model);

                if (!validationResult.IsValid)
                {
                    // Handle validation errors
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(validationResult.Errors));

                    return;
                }

                Console.WriteLine("Request Body: " + requestBody);
                Console.WriteLine("Deserialized Model: " + JsonConvert.SerializeObject(model));
                Console.WriteLine("Model: " + model);


            }

            await next(context);

        }

        //private bool IsValidJson(string json)
        //{
        //    try
        //    {
        //        JToken.Parse(json);
        //        return true;
        //    }
        //    catch (JsonReaderException)
        //    {
        //        return false;
        //    }
        //}
    }

    

}
