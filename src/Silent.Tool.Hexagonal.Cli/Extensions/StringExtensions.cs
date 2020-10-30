using System;
using System.Diagnostics;
using System.IO;

namespace Silent.Tool.Hexagonal.Cli
{
    public static class StringExtensions
    {
        public static bool CreateDirectorySafely(this string directory)
        {
            return !Directory.Exists(directory) && Directory.CreateDirectory(directory) != null;
        }

        public static bool RunInCommandPrompt(this string command)
        {
            var processInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"/c {command}",
                FileName = "cmd.exe"
            };
            var process = Process.Start(processInfo);
            string result = process.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return process.ExitCode == 0;
        }

        public static string GetArgumentValue(string[] args)
        {
            return args != null && args.Length > 0
                ? args[0]
                : null;
        }

        public static string GetOptionsValue(string[] args, string option)
        {
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == option)
                    {
                        return args[i + 1];
                    }
                }
            }

            return null;
        }
    }
}
