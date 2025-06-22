using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
    public class EmailAttribute : ValidationAttribute
    {
        readonly DataTypeAttribute _emailValidationAttribute = new(DataType.EmailAddress);

        public EmailAttribute()
        {
            ErrorMessageResourceType = typeof(DataAnnotationsResources);
            ErrorMessageResourceName = "InvalidEmail";
        }

        public override bool IsValid(object? value) => _emailValidationAttribute.IsValid(value);
    }
}