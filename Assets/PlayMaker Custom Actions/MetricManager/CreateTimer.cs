using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Creates a timer to keep track of a given time metric -- for example, how long it takes a trainee to accomplish a given task.")]
public class CreateTimer: FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the timer to be created.")]
    public string timerName;

    [HutongGames.PlayMaker.Tooltip("Whether or not the created timer should immediately start ticking up upon being created.")]
    public bool activeOnStart = true;

    public override void OnEnter()
    {
        MetricManager.CreateTimer(timerName, activeOnStart);
        Finish();
    }
}
