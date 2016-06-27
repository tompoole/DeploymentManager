using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess.SourceControl
{
    public class GitProvider
    {
        private readonly DeploymentContext _context;

        public GitProvider(DeploymentContext context)
        {
            _context = context;
        }

        public bool Checkout()
        {
            try
            {
                if (Directory.Exists(_context.GitDirectory))
                {
                    string currentDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(_context.GitDirectory);

                    // revert any changes to the git working directory
                    RunGitProcess("reset -q --hard");
                    RunGitProcess("clean -qdf");
                    
                    bool pullSuccess = RunGitProcess("pull -q");
                    if (!pullSuccess)
                    {
                        return false;
                    }

                    bool checkoutSuccess = RunGitProcess("checkout -qf " + _context.Job.Branch); // force checkout, quietly
                    if (!checkoutSuccess)
                    {
                        return false;
                    }

                    Directory.SetCurrentDirectory(currentDir);
                }
                else
                {
                    DateTime startTime = DateTime.Now;

                    _context.CurrentLogger.Info("Directory {0} does not exist. Cloning repository {1}", _context.ProjectDirectory, _context.Job.Url);

                    // clone new git repository
                    string arguments = string.Format("clone \"{0}\" \"{1}\" -q", _context.Job.Url, _context.GitDirectory);
                    bool cloneSuccess = RunGitProcess(arguments);

                    if (!cloneSuccess)
                    {
                        _context.CurrentLogger.Fatal("Could not clone git directory.");
                        return false;
                    }

                    _context.CurrentLogger.Info("Clone complete in {0} seconds.", DateTime.Now.Subtract(startTime).TotalSeconds);

                    // set working directory to git directory:
                    string currentDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(_context.GitDirectory);

                    // checkout to provided branch
                    bool checkoutSuccess = RunGitProcess("checkout " + _context.Job.Branch);
                    if (!checkoutSuccess)
                    {
                        _context.CurrentLogger.Fatal("Error in checkout");
                        return false;
                    }

                    Directory.SetCurrentDirectory(currentDir);
                }
            }
            catch(Exception exception)
            {
                _context.CurrentLogger.Fatal("Error in git checkout ", exception);
                return false;
            }

            return true;
        }

        private bool RunGitProcess(string arguments)
        {
            string gitPath = ConfigurationManager.AppSettings["GitProcessPath"];

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = gitPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardError = true
                }
            };

            _context.CurrentLogger.Info("Running git process with arguments {0}", arguments);

            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                _context.CurrentLogger.Fatal("Git process completed with error");
                _context.CurrentLogger.Fatal(error);
                return false;
            }

            return true;
        }

    }


}