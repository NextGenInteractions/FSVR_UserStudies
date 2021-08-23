using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ENVIRO")]
	[Tooltip("Changes the current time to your system time.")]
	public class SetSystemTime : FsmStateAction
	{	
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void OnEnter()
		{
            EnviroSkyMgr.instance.SetTime (System.DateTime.Now);

			if (!everyFrame) {
				Finish ();
			} else {
                EnviroSkyMgr.instance.Time.ProgressTime = EnviroTime.TimeProgressMode.None;
			}
		}

		public override void OnUpdate()
		{
			EnviroSkyMgr.instance.SetTime (System.DateTime.Now);
		}
	}
}