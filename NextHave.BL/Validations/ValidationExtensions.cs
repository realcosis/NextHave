using Dolphin.Core.Exceptions;
using Newtonsoft.Json.Linq;
using NextHave.BL.Extensions;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NextHave.BL.Validations
{
	public static class ValidationExtensions
	{
		static readonly SemaphoreSlim semaphoreSlim = new(1, 1);
		static readonly Dictionary<Type, List<ChildProperty>> objectProperties = [];
		static readonly Type[] skipTypes = [typeof(JObject), typeof(IEnumerable), typeof(Stream)];

		public static void Validate(this object model)
		{
			if (model != null)
			{
				var errors = ValidateObject(model);

				if (errors.Count > 0)
					throw new DolphinException([.. errors.Select(s => s!.ErrorMessage!)]);
			}
		}

		static List<ValidationResult> ValidateObject(object value)
		{
			var ret = new List<ValidationResult>();
			var objectProperties = GetProperties(value.GetType());

			foreach (var objectProperty in objectProperties)
			{
				var propertyValue = objectProperty.Property.GetValue(value);
				var labelAttribute = objectProperty.Property.GetCustomAttributes(typeof(LabelAttribute), true)
					.Cast<LabelAttribute>()
					.FirstOrDefault();
				var validationContext = new ValidationContext(value, null, null)
				{
					DisplayName = labelAttribute?.Name ?? objectProperty.Property.Name,
					MemberName = objectProperty.Property.Name
				};

				Validator.TryValidateProperty(propertyValue, validationContext, ret);

				if (!objectProperty.IsSimple)
				{
					if (propertyValue == null && objectProperty.IsRequired)
						continue;

					if (propertyValue != null && !objectProperty.SkipPropertiesValidation)
					{
						if (objectProperty.IsCollection)
						{
							if (!objectProperty.Property.PropertyType.IsGenericType ||
								!objectProperty.Property.PropertyType.GetGenericArguments()[0].IsSimpleType())
								foreach (object pv in propertyValue as IEnumerable ?? Array.Empty<object>())
									ret.AddRange(ValidateObject(pv));
						}
						else
							ret.AddRange(ValidateObject(propertyValue));
					}
				}
			}

			return ret;
		}

		static List<ChildProperty> GetProperties(Type type)
		{
			if (!objectProperties.ContainsKey(type))
			{
				semaphoreSlim.Wait();

				try
				{
					if (!objectProperties.ContainsKey(type))
					{
						var childProperties = new List<ChildProperty>();

						if (type.GetCustomAttribute<SkipChildPropertiesValidationAttribute>() == default)
						{
							var properties = type.GetProperties().ToList();

							foreach (var property in properties)
							{
								if (property.PropertyType.IsSimpleType())
								{
									bool isRequired = property.GetCustomAttribute<RequiredAttribute>() != null;
									childProperties.Add(new ChildProperty(property, isRequired, false, true, false));
								}
								else
								{
									//Vedo se ha l'attributo required
									var isCollection = IsCollection(property.PropertyType);
									var isRequired = isCollection ?
										property.GetCustomAttribute<RequiredItemsAttribute>() != null :
										property.GetCustomAttribute<RequiredAttribute>() != null;
									var skipPropertiesValidation = IsSkippable(property);
									childProperties.Add(new ChildProperty(property, isRequired, isCollection, false, skipPropertiesValidation));
								}
							}
						}

						objectProperties.Add(type, childProperties);
					}
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}

			return objectProperties[type];
		}

		static bool IsCollection(Type type) =>
			type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);

		static bool IsSkippable(PropertyInfo property) =>
			property.GetCustomAttribute<SkipChildPropertiesValidationAttribute>() != null || skipTypes.Any(a => a.IsAssignableFrom(property.PropertyType));
	}
}