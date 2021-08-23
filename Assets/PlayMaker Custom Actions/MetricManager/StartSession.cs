using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Starts the metric-recording session.")]
public class StartSession : FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the session.")]
    public FsmString sessionName = "untitled";
    public bool useUiToGetSessionName;

    public override void OnEnter()
    {
        if(!useUiToGetSessionName)
            MetricManager.StartLive(sessionName.ToString());
        else
            MetricManager.StartLive(MetricManagerUi.sessionNameInField);
        Finish();
    }
}
