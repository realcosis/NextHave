using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class DateLessThanNowAttribute : ValidationAttribute
	{
		public DateLessThanNowAttribute()
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "DateLessThanNow";
		}

		public override bool IsValid(object? value) => value == null || (DateTime)value < DateTime.UtcNow;
	}
}