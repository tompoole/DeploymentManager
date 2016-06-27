using System;
using NPoco;

namespace DeploymentManager.Core.Entities
{
    [TableName("Jobs")]
    [PrimaryKey("Id",AutoIncrement = true)]
    public class Job
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public JobState State { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }

        public string StatusMessage { get; set; }
    }
}