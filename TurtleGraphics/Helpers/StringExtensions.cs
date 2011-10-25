using TurtleGraphics.Constants;

namespace TurtleGraphics.Helpers
{
    public static class StringExtensions
    {
        public static string TrimQuotes(this string source)
        {
            return source.Trim().TrimStart(new[] { GlobalConstants.Quote }).TrimEnd(new[] { GlobalConstants.Quote });
        }
    }
}
