using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class DateGreaterOrEqualThanAttribute(string startDatePropertyName) : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var propertyInfo = validationContext.ObjectType.GetProperty(startDatePropertyName);

			if (propertyInfo == null)
				return new ValidationResult(string.Format(DataAnnotationsResources.UnknownProperty, startDatePropertyName));

			var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

			if (value == null || propertyValue == null)
				return ValidationResult.Success;

			if ((DateTime)value >= (DateTime)propertyValue)
				return ValidationResult.Success;
			else
			{
				var startDateDisplayName = propertyInfo
					.GetCustomAttributes(typeof(LabelAttribute), true)
					.Cast<LabelAttribute>()
					.Single()
					.Name;

				return new ValidationResult(string.Format(DataAnnotationsResources.GreaterOrEqualThan, validationContext.DisplayName, startDateDisplayName));
			}
		}
	}
}