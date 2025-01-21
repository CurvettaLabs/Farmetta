using System.Diagnostics;

namespace KlipperInstaller;

public static class CmdHelper
{
    public static async Task RunCmd(string[] cmds)
    {

        // Create and configure the process
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", // The shell to use
                RedirectStandardInput = true,  // To send commands
                RedirectStandardOutput = true, // To capture output
                RedirectStandardError = true,  // To capture errors
                UseShellExecute = false,       // Required for redirection
                CreateNoWindow = true,          // Do not create a terminal window
                UserName = "root"
            }
        };

        try
        {
            // Start the process
            process.Start();

            // Get the input and output streams
            using (StreamWriter stdin = process.StandardInput)
            using (StreamReader stdout = process.StandardOutput)
            using (StreamReader stderr = process.StandardError)
            {
                if (stdin.BaseStream.CanWrite)
                {
                    foreach (string command in cmds)
                    {
                        // Write each command to the shell
                        Console.WriteLine($"Running: {command}");
                        stdin.WriteLine(command);
                    }

                    // Signal the end of commands
                    stdin.Close();

                    // Read and display the output
                    string output = stdout.ReadToEnd();
                    string error = stderr.ReadToEnd();

                    Console.WriteLine("Output:");
                    Console.WriteLine(output);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error:");
                        Console.WriteLine(error);
                    }
                }
            }

            await process.WaitForExitAsync(); // Wait for the process to finish
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}