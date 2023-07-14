//using System.ComponentModel.DataAnnotations;

//namespace CitiesManager.Core.Helpers.Validators
//{
//    /// <summary>
//    /// S15-L163, L175
//    /// </summary>
//    public class ValidationHelperH
//    {
//        internal static void ModelValidation(object obj)
//        {
//            //Model validations
//            ValidationContext validationContext = new ValidationContext(obj);
//            List<ValidationResult> validationResults = new List<ValidationResult>();

//            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
//            if (!isValid)
//            {
//                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
//            }
//        }
//    }
//}
