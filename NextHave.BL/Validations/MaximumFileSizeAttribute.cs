using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
    public class MaximumFileSizeAttribute : ValidationAttribute
    {
        readonly long _maxKb;

        public MaximumFileSizeAttribute(long maxKb) : base()
        {
            ErrorMessageResourceType = typeof(DataAnnotationsResources);
            ErrorMessageResourceName = "MaximumFileSize";
            _maxKb = maxKb;
        }

        public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, _maxKb);

        public override bool IsValid(object? value) => value == default || ((IFormFile)value).Length <= _maxKb * 1024;
    }
}