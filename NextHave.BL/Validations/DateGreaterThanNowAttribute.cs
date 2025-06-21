using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class DateGreaterThanNowAttribute : ValidationAttribute
	{
		public DateGreaterThanNowAttribute()
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "DateGreaterThanNow";
		}

		public override bool IsValid(object? value) => value == null || (DateTime)value > DateTime.UtcNow;
	}
}