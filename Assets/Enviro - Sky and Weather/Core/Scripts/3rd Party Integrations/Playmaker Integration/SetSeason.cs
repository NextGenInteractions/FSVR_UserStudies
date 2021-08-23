using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ENVIRO")]
	[Tooltip("Changes the current season.")]
	public class SetSeason : FsmStateAction
	{
		[RequiredField]
		[Tooltip("What Season to set")]
		public EnviroSeasons.Seasons Season;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			EnviroSkyMgr.instance.Seasons.calcSeasons = false;
            EnviroSkyMgr.instance.ChangeSeason(Season);

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
            EnviroSkyMgr.instance.ChangeSeason(Season);
        }
		
	}


}