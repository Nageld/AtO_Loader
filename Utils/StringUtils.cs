namespace AtO_Loader.Utils
{
    public static class StringUtils
    {
        public static string AppendNotNullOrWhiteSpace(this string str, string append)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                str += append;
            }

            return str;
        }
    }
}
