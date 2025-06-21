namespace NextHave.BL.Validations
{
	public class MinLengthAttribute : System.ComponentModel.DataAnnotations.MinLengthAttribute
	{
		public MinLengthAttribute(int minLength) :
			base(minLength)
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "MinLength";
		}
	}
}