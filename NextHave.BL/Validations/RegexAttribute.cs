using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Validations
{
	public class RegexAttribute : RegularExpressionAttribute
	{
		public RegexAttribute(string pattern, string localizedResourceName) :
			base(pattern)
		{
			var resourceData = new ResourceData(localizedResourceName);

			ErrorMessageResourceType = resourceData.ResourceClass;
			ErrorMessageResourceName = resourceData.ResourceName;
		}
	}
}