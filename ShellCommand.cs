using System;
using System.Diagnostics;

namespace MusicDown;

public class ShellCommand
{
    public void Start(string fileName, string args, bool isShowOutput = true)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process() { StartInfo = startInfo };
        if (isShowOutput)
        {
            process.OutputDataReceived += (sender, data) => System.Console.WriteLine(data.Data);
            process.ErrorDataReceived += (sender, data) => System.Console.WriteLine(data.Data);
        }

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }
}
