namespace DeploymentManager.Core.Entities
{
    public enum JobState
    {
        Pending,
        Running,
        Completed,
        CompletedWithError,
        Failed
    }
}