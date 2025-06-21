using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class RequiredIfAttribute : RequiredAttribute
	{
		readonly string _otherPropertyName;

		public RequiredIfAttribute(string otherPropertyName) :
			base()
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "Required";
			_otherPropertyName = otherPropertyName;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
			(bool)validationContext.ObjectType.GetProperty(_otherPropertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!.GetValue(validationContext.ObjectInstance)! ?
			base.IsValid(value, validationContext) : ValidationResult.Success;
	}
}