namespace NextHave.BL.Validations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class LabelAttribute : Attribute
	{
		readonly ResourceData _labelResourceData;
		readonly ResourceData? _descriptionResourceData;

		public LabelAttribute(string labelLocalizedResource, string? descriptionLocalizedResource = null)
		{
			_labelResourceData = new ResourceData(labelLocalizedResource);

			if (!string.IsNullOrWhiteSpace(descriptionLocalizedResource))
				_descriptionResourceData = new ResourceData(descriptionLocalizedResource);
		}

		public string Name => _labelResourceData.GetValue() ?? string.Empty;

		public string Description => _descriptionResourceData?.GetValue() ?? string.Empty;
	}
}