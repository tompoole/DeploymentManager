using CommandLine;

namespace DeploymentManager.WorkerProcess
{
    public class CommandLineOptions
    {
        [Option('i', "jobId", Required = true)]
        public int JobId { get; set; }
    }
}