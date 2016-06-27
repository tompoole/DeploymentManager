namespace DeploymentManager.Core
{
    public interface IDeploymentTask
    {
        TaskResult RunTask(DeploymentContext deploymentContext);
    }

    public class TaskResult
    {
        private TaskResult() { }

        public static TaskResult CreateSuccessfulResult()
        {
            return new TaskResult {State  = TaskResultState.Completed};
        }

        public static TaskResult CreateErrorResult(string errorMessage)
        {
            return new TaskResult {State = TaskResultState.CompletedWithError, ErrorMessage = errorMessage};
        }

        public static TaskResult CreateTotalFailureResult(string errorMessage)
        {
            return new TaskResult { State = TaskResultState.Failed, ErrorMessage = errorMessage };
        }


        public TaskResultState State { get; set; }
        public string ErrorMessage { get; set; }
    }
}