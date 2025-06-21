using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class MinNumberAttribute : ValidationAttribute
	{
		readonly bool _include;
		readonly decimal _minValue;

		public MinNumberAttribute(byte minValue, bool include = true) :
			this((int)minValue, include)
		{
		}

		public MinNumberAttribute(int minValue, bool include = true) :
			this((decimal)minValue, include)
		{
		}

		public MinNumberAttribute(long minValue, bool include = true) :
			this((decimal)minValue, include)
		{
		}

		public MinNumberAttribute(float minValue, bool include = true) :
			this((decimal)minValue, include)
		{
		}

		public MinNumberAttribute(decimal minValue, bool include = true)
		{
			_include = include;
			_minValue = minValue;
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = include ? "MinNumberInclude" : "MinNumberExclude";
		}

		public override bool IsValid(object? value) => value == default || (decimal.TryParse(value.ToString(), out decimal val) && (_include ? val >= _minValue : val > _minValue));

		public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, _minValue);
	}
}