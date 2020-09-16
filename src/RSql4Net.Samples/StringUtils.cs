using System.Linq;

namespace RSql4Net.Samples
{
    /// <summary>
    /// string extensions
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// convert to snwke_case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }    
    }
}
