using System.Collections.Generic;

namespace DeploymentManager.Core
{
    public class DeploymentConfiguration
    {
        public List<string> Tasks { get; set; }
        public Dictionary<string, string> GlobalParameters { get; set; }
        public Dictionary<string, Dictionary<string, string>> TaskParameters { get; set; }
    }
}