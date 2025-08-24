namespace Mac.Modules.Results;

public class ShowResultService(ISnackbar snackbar)
{
    public void ShowResultMessage(Result result)
    {
        if (result.IsSuccess)
        {
            foreach (var success in result.Successes)
            {
                snackbar.Add(success.Message, Severity.Success);
            }
        }
        else
        {
            foreach (var error in result.Errors)
            {
                snackbar.Add(error.Message, Severity.Error);
            }
        }
    }
}