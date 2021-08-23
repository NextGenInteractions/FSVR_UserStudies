using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Starts a stopped timer with the given name.")]
public class StartTimer: FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the timer to be started.")]
    public string timerName;

    public override void OnEnter()
    {
        MetricManager.StartTimer(timerName);
        Finish();
    }
}
