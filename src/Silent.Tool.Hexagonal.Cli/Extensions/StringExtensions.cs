using System.IO;

namespace Silent.Tool.Hexagonal.Cli
{
    public static class StringExtensions
    {
        public static bool CreateDirectorySafely(this string directory)
        {
            return !Directory.Exists(directory) && Directory.CreateDirectory(directory) != null;
        }

        public  static string ReplaceToken(this string template, string token, string value)
        {
            return template.Replace(token, value);
        }
    }
}
