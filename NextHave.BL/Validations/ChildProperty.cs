using System.Reflection;

namespace NextHave.BL.Validations
{
	internal class ChildProperty
	{
		internal ChildProperty(PropertyInfo property, bool isRequired, bool isCollection, bool isSimple, bool skipPropertiesValidation)
		{
			Property = property;
			IsRequired = isRequired;
			IsCollection = isCollection;
			IsSimple = isSimple;
			IsRequired = isRequired;
			SkipPropertiesValidation = skipPropertiesValidation;
		}

		internal PropertyInfo Property { get; set; }

		internal bool IsRequired { get; set; }

		internal bool IsCollection { get; set; }

		internal bool IsSimple { get; set; }

		internal bool SkipPropertiesValidation { get; set; }
	}
}