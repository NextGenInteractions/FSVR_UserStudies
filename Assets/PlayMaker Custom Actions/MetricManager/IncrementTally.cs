using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Increments a given tally counter.")]
public class IncrementTally : FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the tally to increment.")]
    public string tallyName;

    [HutongGames.PlayMaker.Tooltip("Optionally, a note to print to the log immediately after the tally is incremented.")]
    public string optionalNote;


    public override void OnEnter()
    {
        MetricManager.IncrementTally(tallyName, optionalNote);
        Finish();
    }
}
