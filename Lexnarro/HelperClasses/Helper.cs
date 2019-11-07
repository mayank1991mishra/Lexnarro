
namespace Lexnarro.HelperClasses
{
    public static class Helper
    {
        public static string RemoverStrs(this string str, string[] removeStrs)
        {
            foreach (var removeStr in removeStrs)
                str = str.Replace(removeStr, "");
            return str;
        }
    }
}