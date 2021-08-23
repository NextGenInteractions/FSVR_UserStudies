using HutongGames.PlayMaker;
[ActionCategory("Metric Manager")]
[HutongGames.PlayMaker.Tooltip("Creates a tally counter to count a given metric -- for example, instances of aid required for a trainee to successfully complete an action.")]
public class CreateTally: FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The name of the tally to be created.")]
    public string tallyName;

    [HutongGames.PlayMaker.Tooltip("The count that the tally will start at -- usually 0.")]
    public int startingCount = 0;


    public override void OnEnter()
    {
        MetricManager.CreateTally(tallyName, startingCount);
        Finish();
    }
}
