using HutongGames.PlayMaker;
using NextGen.VrManager.ToolManagement;

[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Sets a compatible tool to log its input events in the metric log.")]
public class LogDeviceInput: FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The tool for which input logging is desired.")]
    public Tool tool;

    public override void OnEnter()
    {
        tool.logInputToMetricLog = true;
        Finish();
    }
}
