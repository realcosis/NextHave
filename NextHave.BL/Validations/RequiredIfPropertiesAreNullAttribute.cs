using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class RequiredIfPropertiesAreNullAttribute(params string[] propertyNames) : RequiredAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
			propertyNames.Select(x => validationContext.ObjectType.GetProperty(x)?.GetValue(validationContext.ObjectInstance))
										.All(x => x is string s ? string.IsNullOrWhiteSpace(s) : x == null) ?
													base.IsValid(value, validationContext) : ValidationResult.Success;
	}
}