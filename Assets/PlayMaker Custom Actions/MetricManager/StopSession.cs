using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Stops the metric-recording session.")]
public class StopSession : FsmStateAction
{
    public override void OnEnter()
    {
        MetricManager.StopLive();
        Finish();
    }
}
