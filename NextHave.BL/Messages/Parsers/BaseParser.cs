namespace NextHave.BL.Messages.Parsers
{
    public sealed class BaseParser<T> : AbstractParser<T> where T : IInput, new()
    {
        public override IInput Parse(ClientMessage packet)
            => new T();
    }
}