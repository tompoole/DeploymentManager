using System.Collections.Generic;
using System.Linq;

namespace DeploymentManager.Core
{
    public class TaskParameterManager
    {
        private readonly Dictionary<string,string> _globalParameters = new Dictionary<string, string>();
        private readonly Dictionary<string,string> _taskParameters = new Dictionary<string, string>();

        private const string GlobalParamKeyPrefix = "global_";

        public void InitialiseParametersFromConfiguration(DeploymentConfiguration config)
        {
            if (config.TaskParameters != null)
            {
                InitTaskParams(config);
            }

            if (config.GlobalParameters != null)
            {
                InitGlobalParams(config);
            }
        }

        private void InitTaskParams(DeploymentConfiguration config)
        {
            foreach (var taskConfig in config.TaskParameters)
            {
                string taskName = taskConfig.Key.ToLowerInvariant();

                foreach (var configItem in taskConfig.Value)
                {
                    string key = string.Format("{0}_{1}", taskName, configItem.Key);
                    _taskParameters.Add(key, configItem.Value);
                }
            }
        }

        private void InitGlobalParams(DeploymentConfiguration config)
        {

            foreach (var taskParameter in config.GlobalParameters)
            {
                string key = GlobalParamKeyPrefix + taskParameter.Key;
                _globalParameters.Add(key, taskParameter.Value);
            }
        }

        public string GetValue<T>(string parameterKey) where T : IDeploymentTask
        {
            string taskName = typeof (T).Name.ToLowerInvariant();
            string taskParamKey = string.Format("{0}_{1}", taskName, parameterKey);

            if (_taskParameters.ContainsKey(taskParamKey))
            {
                return _taskParameters[taskParamKey];
            }

            string globalParamKey = GlobalParamKeyPrefix + parameterKey;

            if (_globalParameters.ContainsKey(globalParamKey))
            {
                return _globalParameters[globalParamKey];
            }

            return null;
        }

    }
}