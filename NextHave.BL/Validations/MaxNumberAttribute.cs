using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class MaxNumberAttribute : ValidationAttribute
	{
		readonly bool _include;
		readonly decimal _maxValue;

		public MaxNumberAttribute(byte maxValue, bool include = true) :
			this((int)maxValue, include)
		{
		}

		public MaxNumberAttribute(int maxValue, bool include = true) :
			this((decimal)maxValue, include)
		{
		}

		public MaxNumberAttribute(long maxValue, bool include = true) :
			this((decimal)maxValue, include)
		{
		}

		public MaxNumberAttribute(float maxValue, bool include = true) :
			this((decimal)maxValue, include)
		{
		}

		public MaxNumberAttribute(decimal maxValue, bool include = true)
		{
			_include = include;
			_maxValue = maxValue;
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = include ? "MaxNumberInclude" : "MaxNumberExclude";
		}

		public override bool IsValid(object? value) => value == default || (decimal.TryParse(value.ToString(), out decimal val) && (_include ? val <= _maxValue : val < _maxValue));

		public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, _maxValue);
	}
}