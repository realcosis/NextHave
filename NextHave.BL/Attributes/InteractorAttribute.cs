using NextHave.DAL.Enums;

namespace NextHave.BL.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InteractorAttribute(InteractionTypes interactionType) : Attribute
    {
        public InteractionTypes InteractionType { get; } = interactionType;
    }
}