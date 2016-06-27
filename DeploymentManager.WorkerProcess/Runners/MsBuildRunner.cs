using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using NLog;

namespace DeploymentManager.WorkerProcess.Runners
{
    public class MsBuildRunner
    {
        private const string MsBuildExe = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
        private readonly Logger _logger;
        private readonly StringBuilder _outputStringBuilder;

        public MsBuildRunner(Logger logger)
        {
            _logger = logger;
            _outputStringBuilder = new StringBuilder();
        }
        
        public string GetBuildFileFullPath(string buildFile)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            return Path.Combine(baseDirectory, "msbuild", buildFile);
        }

        public bool StartMsDeployProcess(string msBuildProjectFile, string targetName, params string[] parameters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("\"{0}\" ", GetBuildFileFullPath(msBuildProjectFile));

            if (!string.IsNullOrEmpty(targetName))
            {
                stringBuilder.AppendFormat("/t:{0} ", targetName);
            }

            foreach (var parameter in parameters)
            {
                stringBuilder.AppendFormat("/p:{0} ", parameter);
            }

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = MsBuildExe,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    Arguments = stringBuilder.ToString(),
                    UseShellExecute = false
                }
            };

            _logger.Info("Starting msbuild with arguments of {0}", process.StartInfo.Arguments);

            process.OutputDataReceived += process_OutputDataReceived;

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

     
            if (process.ExitCode == 0)
            {
                _logger.Info("Finished msbuild process successfully");
                return true;
            }

            _logger.Error("Finished msbuild process with error:");
            _logger.Error(_outputStringBuilder.ToString());
            return false;
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                _outputStringBuilder.AppendLine(e.Data);
            }
        }
    }
}