using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Log the current tracked time of a given timer to the metric log, without stopping that timer.")]
public class LogTimerTime: FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the timer to log.")]
    public string timerName;


    public override void OnEnter()
    {
        MetricManager.LogTimerTime(timerName);
        Finish();
    }
}
