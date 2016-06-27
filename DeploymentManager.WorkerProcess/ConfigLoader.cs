using System;
using System.IO;
using Newtonsoft.Json;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess
{
    public class ConfigLoader
    {
        public static bool LoadConfigurationIntoContext(DeploymentContext context)
        {
            try
            {
                string expectedConfigFile = Path.Combine(context.ConfigurationDirectory, context.Job.Branch + ".json");
                string configFileContents = File.ReadAllText(expectedConfigFile);

                DeploymentConfiguration config = JsonConvert.DeserializeObject<DeploymentConfiguration>(configFileContents);

                context.TaskParameters.InitialiseParametersFromConfiguration(config);

                // set web directory
                string webDirectory = config.GlobalParameters != null && config.GlobalParameters.ContainsKey("webDirectory") ? config.GlobalParameters["webDirectory"] : "WWW";
                string fullWebDirectory = Path.Combine(context.GitDirectory, webDirectory);
                context.WebSiteDirectory = fullWebDirectory;

                context.Configuration = config;
            }
            catch (Exception exception)
            {
                context.CurrentLogger.Fatal("Cannot load configuration file ({0}). Ensure the file exists and is valid JSON.", context.Branch);
                context.CurrentLogger.Fatal(exception);
                return false;
            }

            return true;
        }
    }
}