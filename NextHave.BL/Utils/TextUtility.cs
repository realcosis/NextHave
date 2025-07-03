using HtmlAgilityPack;

namespace NextHave.BL.Utils
{
    public static class TextUtility
    {
        public static string GetString(this double value)
            => value.ToString();

        public static string Escape(this string input, bool allowBreaks = false)
        {
            input = input.Trim();

            input = input.Replace(Convert.ToChar(1), ' ');
            input = input.Replace(Convert.ToChar(2), ' ');
            input = input.Replace(Convert.ToChar(3), ' ');
            input = input.Replace(Convert.ToChar(9), ' ');

            if (!allowBreaks)
            {
                input = input.Replace(Convert.ToChar(10), ' ');
                input = input.Replace(Convert.ToChar(13), ' ');
            }

            input = input.RemoveAllTags();

            return input;
        }

        public static string RemoveAllTags(this string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc.DocumentNode.InnerText;
        }
    }
}