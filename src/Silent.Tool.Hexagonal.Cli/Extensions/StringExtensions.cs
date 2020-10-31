using System.IO;

namespace Silent.Tool.Hexagonal.Cli
{
    public static class StringExtensions
    {
        public static bool CreateDirectorySafely(this string directory)
        {
            return !Directory.Exists(directory) && Directory.CreateDirectory(directory) != null;
        }
    }
}
