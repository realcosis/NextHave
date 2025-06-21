using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class RequiredItemsAttribute : ValidationAttribute
	{
		readonly int _minLength = 1;

		public RequiredItemsAttribute(int minLength) :
			base()
		{
			_minLength = minLength;
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "RequiredItems";
		}

		public override bool IsValid(object? value) =>
			value != null && value as IList != null && ((IList)value).Count >= _minLength;

		public override string FormatErrorMessage(string name) =>
			string.Format(ErrorMessageString, name, _minLength);
	}
}