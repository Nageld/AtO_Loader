using System.Text.RegularExpressions;

namespace AtO_Loader.Utils
{
    public static class RegexUtils
    {
        public static readonly Regex HasInvalidIdRegex = new("[^a-zA-Z0-9]", RegexOptions.Compiled);
    }
}
