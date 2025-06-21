namespace NextHave.BL.Validations
{
	public class MaxLengthAttribute : System.ComponentModel.DataAnnotations.MaxLengthAttribute
	{
		public MaxLengthAttribute(int maxLength) :
			base(maxLength)
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "MaxLength";
		}
	}
}