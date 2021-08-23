using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Stops a timer with the given name, logging its current time to the metric log while also preventing it from ticking up any further.")]
public class StopTimer: FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the timer to be stopped.")]
    public string timerName;

    public override void OnEnter()
    {
        MetricManager.StopTimer(timerName);
        Finish();
    }
}
