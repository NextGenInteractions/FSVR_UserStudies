using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class StandardManager: MonoBehaviour
{
	public static string[] Colors = new string[] { "green", "yellow", "lime", "red", "black", "blue", "brown", "cyan", "grey", "olive", "orange", "silver", "white" };
	private Standard _standard;
	private static StandardManager _instance;
	public bool show;
	public Vector2 tempH, tempA;
	public int tempC;
	[SerializeField]
	GameObject singularRule;
	[SerializeField]
	Transform container;
	[SerializeField]
	TMP_InputField disThres;
	public string decimalRule = "0.00";

	public Standard standard
	{
		get
		{
			if (_standard == null)
				LoadStanddard();
			return _standard;
		}
	}

	public static StandardManager instance { get => _instance; }

	public void Awake()
    {
		_instance = this;
    }

    private void Start()
	{
		LoadStanddard();
	}

    public void Init()
	{
		_standard = new Standard();
	}

	public void EditDistanceThreshold()
    {
		float.TryParse(disThres.text, out _standard.distanceThreshold);
    }

	public void UpdateStandard()
    {
		disThres.text = _standard.distanceThreshold.ToString(decimalRule);
		int count = _standard.heights.Count;
		int objCount = container.childCount;
		float viewPortHeight = count * 400;
		Vector2 size = GetComponent<RectTransform>().sizeDelta;
		size.y = viewPortHeight;
		GetComponent<RectTransform>().sizeDelta = size;
		_standard.ruleList.Clear();
		for (int i = 0; i < count; ++i)
        {
			SingularStandard singularStandard;
			Transform theRule;
			if (i >= objCount)
            {
				theRule = GameObject.Instantiate(singularRule).transform;
				theRule.SetParent(container);
				theRule.localScale = Vector3.one;
				theRule.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
			}
            else
            {
				theRule = container.GetChild(i);
			}
			theRule.GetComponent<RectTransform>().offsetMin = Vector2.up * (viewPortHeight - 200 - (400 * i));
			theRule.GetComponent<RectTransform>().offsetMax = Vector2.zero;
			singularStandard = theRule.GetComponent<SingularStandard>();
			singularStandard.SetIndex (i);
			singularStandard.OnEditIn(_standard.heights[i], _standard.angles[i]);
			_standard.ruleList.Add(singularStandard);

		}
    }

	public void EditRule(int index, Vector2 height, Vector2 angle, int color)
	{
		if (index < _standard.heights.Count && Modified(index, height, angle, color))
		{
			_standard.heights[index] = height;
			_standard.angles[index] = angle;
			_standard.colors[index] = color;
			UpdateStandard();
		}
	}

	bool Modified(int index, Vector2 height, Vector2 angle, int color)
	{
		return (height.x != _standard.heights[index].x || height.y != _standard.heights[index].y || angle.x != _standard.angles[index].x || angle.y != _standard.angles[index].y);
	}

	public void AddRule(Vector2 height, Vector2 angle, int color)
	{
		int count = 0;
		//if ((count = _standard.heights.Count) > 0)
		//{
		//	for (int i = 0; i < count; ++i)
		//	{
		//		if (_standard.heights[i].x > height.y)
		//		{
		//			count = i;
		//			break;
		//		}
		//	}
		//}
		_standard.heights.Insert(count, height);
		_standard.angles.Insert(count, angle);
		_standard.colors.Insert(count, color);
		tempA = Vector2.zero;
		tempH = Vector2.zero;
		tempC = 0;
		UpdateStandard();
	}

	public void AddRule()
    {
		AddRule(Vector2.zero, Vector2.zero, 0);
    }

	public void DeleteRule(int index)
	{
		if (index < _standard.heights.Count)
		{
			_standard.heights.RemoveAt(index);
			_standard.angles.RemoveAt(index);
			_standard.colors.RemoveAt(index);
			for(int i = index; i < _standard.ruleList.Count; ++i)
            {
				_standard.ruleList[i].SetIndex(i);
            }
			UpdateStandard();
		}
	}

	public void SaveStandard()
	{
		PlayerPrefs.SetString("LighthouseStandard", JsonUtility.ToJson(_standard));
	}

	public void LoadStanddard()
	{
		if (PlayerPrefs.HasKey("LighthouseStandard"))
			_standard = JsonUtility.FromJson<Standard>(PlayerPrefs.GetString("LighthouseStandard"));
		else
			Init();
		UpdateStandard();
	}

	public string GetAngleResult(float height, float angle)
	{
		for (int i = 0; i < standard.heights.Count; ++i)
		{
			if (height >= standard.heights[i].x && height <= standard.heights[i].y)
			{
				float lower = height - standard.heights[i].x;
				float max = standard.heights[i].y - standard.heights[i].x;
				float perc = lower / max;
				float expectedAngle = (standard.angles[i].y - standard.angles[i].x) * perc + standard.angles[i].x;
				float angleDiff = Mathf.Abs(angle - expectedAngle);
				if (angleDiff > 5)
					return "red";
				else if (angleDiff > 2)
					return "orange";
				else
					return "green";
			}
		}
		return "red";
	}

	public Color GetAngleColorResult(float height, float angle)
	{
		string str = GetAngleResult(height, angle);
		switch (str)
		{
			case "red":
				return Color.red;
			case "orange":
				return Color.yellow;
			case "green":
				return Color.green;
			default:
				return Color.red;
		}
	}

	public Color GetTiltColorResult (float angle)
	{
		angle = Mathf.Abs(angle);
		angle = Mathf.Min(angle, 360 - angle);
		if (angle > 5)
			return Color.red;
		else if (angle > 3)
			return Color.yellow;
		else
			return Color.green;
	}

	public string GetHeightResult(float height, float angle)
	{
		for (int i = 0; i < standard.angles.Count; ++i)
		{
			if (angle >= standard.angles[i].x && angle <= standard.angles[i].y)
			{
				float lower = angle - standard.angles[i].x;
				float max = standard.angles[i].y - standard.angles[i].x;
				float perc = lower / max;
				float expectedHeight = (standard.heights[i].y - standard.heights[i].x) * perc + standard.heights[i].x;
				float heightDiff = Mathf.Abs(height - expectedHeight);
				if (heightDiff > 0.5)
					return "red";
				else if (heightDiff > 0.3)
					return "orange";
				else
					return "green";
			}
		}
		return "red";
	}
}

public class Standard
{
	public float distanceThreshold = 6;
	public List<Vector2> heights = new List<Vector2>();
	public List<Vector2> angles = new List<Vector2>();
	public List<int> colors = new List<int>();
	public List<SingularStandard> ruleList = new List<SingularStandard>();
}
