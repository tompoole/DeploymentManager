using System.Diagnostics;
using NLog;

namespace DeploymentManager.WorkerProcess
{
    public class MsDeployRunner
    {
        private const string MsDeployPath = @"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe";
        private readonly Logger _logger;

        public MsDeployRunner(Logger logger)
        {
            _logger = logger;
        }

        public bool Run(string arguments)
        {
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = MsDeployPath,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    Arguments = arguments,
                    UseShellExecute = false
                }
            };

            _logger.Debug("Starting msdeploy with arguments of {0}", process.StartInfo.Arguments);

            process.OutputDataReceived += process_OutputDataReceived;

            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();

            string error = process.StandardError.ReadToEnd();


            if (!string.IsNullOrEmpty(error))
            {
                _logger.Error(error);
                return false;
            }

            return true;
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null) _logger.Debug(e.Data);
        }
    }
}