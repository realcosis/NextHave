namespace NextHave.BL.Validations
{
	public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
	{
		public RequiredAttribute() :
			base()
		{
			ErrorMessageResourceType = typeof(DataAnnotationsResources);
			ErrorMessageResourceName = "Required";
		}
	}
}