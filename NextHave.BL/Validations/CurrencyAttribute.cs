using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class CurrencyAttribute : ValidationAttribute
	{
		public CurrencyAttribute()
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "Currency";
		}

		public override bool IsValid(object? value) =>
			value == default || (decimal)value * 100 == (long)((decimal)value * 100);
	}
}