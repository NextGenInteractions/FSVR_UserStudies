using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ENVIRO")]
	[Tooltip("Gets the current time (Date).")]
	public class GetTimeDate : FsmStateAction
	{
		public FsmInt Second;
		public FsmInt Minute;
		public FsmInt Hour;
		public FsmInt Day;
		public FsmInt Year;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void OnEnter()
		{
			Second.Value = EnviroSkyMgr.instance.Time.Seconds;	
			Minute.Value = EnviroSkyMgr.instance.Time.Minutes;	
			Hour.Value = EnviroSkyMgr.instance.Time.Hours;		
			Day.Value = EnviroSkyMgr.instance.Time.Days;
			Year.Value = EnviroSkyMgr.instance.Time.Years;

			if (!everyFrame)
			{
				Finish();
			}
		}


		public override void OnUpdate()
		{
			Second.Value = EnviroSkyMgr.instance.Time.Seconds;	
			Minute.Value = EnviroSkyMgr.instance.Time.Minutes;	
			Hour.Value = EnviroSkyMgr.instance.Time.Hours;		
			Day.Value = EnviroSkyMgr.instance.Time.Days;
			Year.Value = EnviroSkyMgr.instance.Time.Years;
		}
	}
}