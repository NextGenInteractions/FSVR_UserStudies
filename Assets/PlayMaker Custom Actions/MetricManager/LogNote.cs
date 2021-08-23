using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Adds a given line to the metric log.")]
public class LogNote : FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The note to log.")]
    public string noteToLog;


    public override void OnEnter()
    {
        MetricManager.LogEvent(noteToLog);
        Finish();
    }
}
