using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas;

namespace DeploymentManager.Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = Host
                .Configure<DeploymentManagerService>()
                .Named("TMPW Deployment Manager Service")
                .WithArguments(args);

            Host.Start(config);
        }
    }
}
