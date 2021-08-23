using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteNode : MonoBehaviour
{
	public Vector3 Pos
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = value;
		}
	}

	public enum NodeType
	{
		StopPoint,
		Continuity,
		Divergence
	}

	public NodeType nodeType = NodeType.Continuity;
	
	[Range(0, 1)]
	public float resistance = 0;
	[Tooltip("On StopPoint, Avatar turns to face next node")]
	public bool faceNextNode = false;
	[HideInInspector]
	public List<AnimationVariable> motionAnimList;
	[HideInInspector]
	public List<AnimationVariable> pauseAnimList;
	[SerializeField]
	public AnimationSequel enterNodeSequel, exitNodeSequel, onNodeSequel;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class AnimationVariable
{
	public enum Type
	{
		Boolean,
		Intager,
		Float,
		Trigger
	}
	public Type type = Type.Boolean;
	public string variableName;
	public bool Boolean;
	public int Integer;
	public float Float;
}

[System.Serializable]
public class AnimationSequel
{
	public enum PlayRoutine
	{
		Once,
		Loop,
		LoopForTime,
		LoopForNumber
	}

	public PlayRoutine routine = PlayRoutine.Once;
	public float time;
	public int numberOfRun;
	public List<string> stateList = new List<string>();
}
