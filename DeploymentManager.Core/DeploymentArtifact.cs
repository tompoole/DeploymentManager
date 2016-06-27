using System;
using System.Collections.Generic;
using System.Linq;

namespace DeploymentManager.Core
{
    public class DeploymentArtifactManager
    {
        readonly Dictionary<string, DeploymentArtifact> _deploymentArtifacts = new Dictionary<string, DeploymentArtifact>();

        public DeploymentArtifact GetArtifactByTask<T>() where T : IDeploymentTask
        {
            string taskName = typeof (T).Name;
            return _deploymentArtifacts[taskName];
        }

        public DeploymentArtifact AddArtifact<T>(string pathToArtifact)
        {
            string taskName = typeof (T).Name;
            return AddArtifact(taskName, pathToArtifact);
        }

        public DeploymentArtifact AddArtifact(string key, string pathToArtifact)
        {
            var artifact = new DeploymentArtifact
            {
                Created = DateTime.Now,
                Path = pathToArtifact,
                TaskName = null,
            };

            _deploymentArtifacts.Add(key, artifact);
            return artifact;
        }

        public DeploymentArtifact GetArtifactByKey(string key)
        {
            return _deploymentArtifacts.ContainsKey(key) ? _deploymentArtifacts[key] : null;
        }

    }


    public class DeploymentArtifact
    {
        public string TaskName { get; set; }

        public string Path { get; set; }
        public DateTime Created { get; set; }
    }
}