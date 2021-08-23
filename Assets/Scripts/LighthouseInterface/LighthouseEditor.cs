
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using UnityEditor;
using System;
using System.Linq;
using TMPro;
using UnityEngine.Rendering.UI;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using UnityEngine.Animations;

[Serializable]
public class LighthouseEditor : MonoBehaviour {
	public static LighthouseEditor instance;
	/* Legacy Setings
	bool isPlaying;
	bool initialized;
	Vector2 scrollPos = Vector2.zero;
	GUIStyle style, table;
	Rect detailWindowRect = new Rect(200, 100, 400, 300);
	Rect overviewWindowRect = new Rect(100, 100, 500, 500);
	Texture tex_side, tex_front, tex_angle;
	int viewerIndex = 0;
	bool onViewer = true, onOverview = false;
	*/
	public Transform root;
	List<LHInfo> lhList = new List<LHInfo>();
	List<Pair> pairList = new List<Pair>();
	string[] pairStrList;
	int maxPairLength = 0;
	StandardManager standardManager { get => StandardManager.instance; }
	int refreshRate = 1;
	[SerializeField]
	GameObject LighthouseInstance;
	[HideInInspector]
	public bool isTestMode = false;
	Coroutine refreshRoutine;
	[Header("LH List Settings")]
	public Transform listContainer;
	public GameObject serialObj, deviceObj, pairObj, activityObj, nicknameObj;
	List<LHUI> lhUIList = new List<LHUI>();
	[Header("LH Pair Settings")]
	public Transform pairContainer;
	public GameObject pairRowObj;
	[Header("LH Matrix Settings")]
	public Transform matrixContainer;
	public GameObject matrixObj;
	GameObject[] matrixList;
	[Header("LH Focus Settings")]
	public Transform focusContainer;
	public GameObject focusObj;
	[Header("3D View Settings")]
	public List<Color> colorList = new List<Color>();
	// Add menu named "Lighthouse View" to the Window menu
	//[MenuItem("Window/Lighthouse View")]
	// static void Init() {
	//   // Get existing open window or if none, make a new one:
	//   LighthouseEditor window = (LighthouseEditor)EditorWindow.GetWindow(typeof(LighthouseEditor));
	//   window.Show();
	// }

	private void OnEnable()
    {
		instance = this;
		isTestMode = false;
		/* Legacy setting
		if (PlayerPrefs.HasKey("DetailRect"))
        {
            Vector2 detailRectPos = JsonUtility.FromJson<Vector2>(PlayerPrefs.GetString("DetailRect"));
            detailWindowRect.x = detailRectPos.x;
            detailWindowRect.y = detailRectPos.y;
        }
		*/
	}

    private void Start()
    {
		GetLighthouses();
        // temporary fix for pairlist being called
        // before live lighthouses initialized
        Invoke("UpdatePairList", 1);
    }

    #region Lighthouse Listing
    public void OnPairEdit(Dropdown dropdown)
	{
		LHUI lhui = lhUIList.Where((lh) => lh.pair == dropdown).FirstOrDefault();
		int i = lhui.index;
		int value = dropdown.value;
		int preIndex = lhList[i].pairIndex;
		if (preIndex != value && lhList[i].isActive)
        {
			maxPairLength = lhList[i].SetIndex(pairList, value);
			UpdateLHList();
			UpdatePairList();
		}
	}
	public void OnTxtFieldEdit(TMP_InputField txt)
	{
		LHUI lhui = lhUIList.Where((lh) => lh.nickname == txt).FirstOrDefault();
		int i = lhui.index;
		lhList[i].nickname = txt.text;
		UpdatePairList();
	}
	public void OnSeiralClick(Button btn)
	{
		LHUI lhui = lhUIList.Where((lh) => lh.serial == btn).FirstOrDefault();
		int i = lhui.index;
		// viewerIndex = i;
	}

	public void OnActivityToggle(Toggle tog)
	{
		LHUI lhui = lhUIList.Where((lh) => lh.activity == tog).FirstOrDefault();
		int i = lhui.index;
		maxPairLength = lhList[i].Activate(pairList, tog.isOn);
		UpdateLHList();
		UpdatePairList();
	}
	public void UpdateLHList()
    {
		int uiCount = lhUIList.Count;
		int count = lhList.Count;
		for (int i = 0; i < count; ++i)
        {
			if (i >= uiCount)
            {
				LHUI ui = new LHUI();
				ui.Init(pairStrList.ToList(), i);
				lhUIList.Add(ui);
			}
            else
            {
				lhUIList[i].SetIndex(i);
            }
        }

		for (int i = uiCount - 1; i >= 0; --i)
        {
			if (i >= count)
            {
				lhUIList[i].Clear();
				lhUIList.RemoveAt(i);
            }
        }
    }
    #endregion
    #region Lighthouse Pairing
    public void UpdatePairList()
	{
		for (int i = pairContainer.childCount - 1; i >= 0; --i)
			DestroyImmediate(pairContainer.GetChild(i).gameObject);
		for (int i = 0; i < pairList.Count; ++i)
		{
			int matrixL = pairList[i].GetValidLength();
			if (matrixL > 0)
			{
				Transform newRow;
				newRow = GameObject.Instantiate(pairRowObj).transform;
				newRow.SetParent(pairContainer);
				newRow.localScale = Vector3.one;
				Transform theCol = newRow.GetChild(0);
				theCol.GetComponentInChildren<TMP_Text>().text = pairList[i].name;
				for (int j = 0; j < maxPairLength; j++)
				{
					bool inList = j < pairList[i].lhList.Count;
					LHInfo lh;
					if (inList && (lh=pairList[i].lhList[j]) != null)
					{
						if (lh.isActive)
						{
							Transform newCol = GameObject.Instantiate(theCol.gameObject).transform;
							newCol.SetParent(newRow);
							newCol.localScale = Vector3.one;
							TMP_Text[] txts = newCol.GetComponentsInChildren<TMP_Text>();
							txts[0].text = lh.nickname;
							txts[1].text = lh.id.ToString();

                            if (standardManager == null)
                              return;

							txts[2].text = "<color=" + standardManager.GetAngleResult(lh.height, lh.angle) + ">" + lh.height.ToString("0.00") + "</color>";
							txts[3].text = "<color=" + standardManager.GetAngleResult(lh.height, lh.angle) + ">" + lh.angle.ToString("0.00") + "</color>";
						}
					}
				}
			}
		}
	}
    #endregion
    #region Lighthouse Matrix
    public void OnMatrixClick(GameObject matrix)
    {
		int index = (matrixList.ToList()).IndexOf(matrix);
		pairList[index].showMatrix = matrix.GetComponent<UIFoldout>().isOn;
		UpdateMatrix();
    }

	public void UpdateMatrix()
    {
		int i = 0;
		RectTransform current = null;
		while (i < matrixList.Length - 1)
        {
			if (current == null && matrixList[i] != null)
			{
				current = matrixList[i].GetComponent<RectTransform>();
			}
            if (current != null && matrixList[i + 1] != null)
			{
				int maxLen = pairList[i].GetValidLength();
				RectTransform next = matrixList[i + 1].GetComponent<RectTransform>();
				float y = (pairList[i].showMatrix) ? current.anchoredPosition.y - 180 - (maxLen * 100) : current.anchoredPosition.y - 180;
				next.anchoredPosition = new Vector2(-600, y);
				current = matrixList[i + 1].GetComponent<RectTransform>();
			}
			++i;
		}
	}

	public void DrawMatrix()
	{
		for (int i = matrixContainer.childCount - 1; i >= 0; --i)
			DestroyImmediate(matrixContainer.GetChild(i).gameObject);
		matrixList = new GameObject[pairList.Count];
		for (int i = 0; i < pairList.Count; ++i)
		{
			int matrixL = pairList[i].GetValidLength();
			if (matrixL > 0)
			{
                pairList[i].CalcDisMatrix();
				Transform newMatrix;
				newMatrix = GameObject.Instantiate(matrixObj).transform;
				newMatrix.SetParent(matrixContainer);
				newMatrix.localScale = Vector3.one;
				newMatrix.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, 850);
				newMatrix.GetComponent<TMP_Text>().text = pairList[i].name;
				matrixList[i] = (newMatrix.gameObject);
				pairList[i].showMatrix = newMatrix.GetComponent<UIFoldout>().isOn;
				newMatrix.name = "Matrix-" + pairList[i].name;
				newMatrix.GetComponent<UIFoldout>().onValueChanged.AddListener(delegate { OnMatrixClick(newMatrix.gameObject); });
				Transform theRow = newMatrix.Find("Row");
				theRow.GetComponent<GridLayoutGroup>().constraintCount = matrixL + 1;
				Transform theCol = theRow.Find("Col");
				theCol.GetComponent<GridLayoutGroup>().constraintCount = matrixL + 1;
				for (int k = 0; k <= matrixL; ++k)
				{
					Transform newCol = GameObject.Instantiate(theCol.gameObject).transform;
					newCol.SetParent(theRow);
					newCol.localScale = Vector3.one;
					GameObject theCell = newCol.Find("Cell").gameObject;
					for (int l = 0; l <= matrixL; ++l)
					{
						Transform newCell = GameObject.Instantiate(theCell).transform;
						newCell.SetParent(newCol);
						newCell.localScale = Vector3.one;
						string label = pairList[i].distanceMatrix[k][l];
						if (float.TryParse(label, out float n))
						{
							label = (n < standardManager.standard.distanceThreshold) ? "<color=green>" + n.ToString(standardManager.decimalRule) + "</color>" : "<color=red>" + n.ToString(standardManager.decimalRule) + "</color>";
						}
						newCell.GetComponentInChildren<TMP_Text>().text = label;
					}
					theCell.SetActive(false);
				}
				theCol.gameObject.SetActive(false);
			}
			else
				matrixList[i] = null;
		}
		UpdateMatrix();
	}
    #endregion
    #region Lighthouse FocusView

	public void DrawFocusView()
    {
		int counter = 0;
		for (int i = focusContainer.childCount - 1; i >= 0; --i)
			DestroyImmediate(focusContainer.GetChild(i).gameObject);
		for(int i = 0; i < lhList.Count; ++i)
        {
			LHInfo lh = lhList[i];
			if (lh.isActive)
            {
				Transform newFocus = GameObject.Instantiate(focusObj).transform;
				newFocus.SetParent(focusContainer);
				newFocus.localScale = Vector3.one;
				newFocus.GetComponent<RectTransform>().anchoredPosition = Vector2.up * (1775 - counter * 425);
				++counter;
				newFocus.Find("Txt_FocusName").GetComponent<TMP_Text>().text = lh.serial + " - " + lh.nickname;
				newFocus.Find("Txt_Height").GetComponent<TMP_Text>().text = "Height: " + lh.height.ToString(standardManager.decimalRule) + " m";
				Transform pitch = newFocus.Find("PitchFrame");
				pitch.GetComponent<Image>().color = standardManager.GetAngleColorResult(lh.height, lh.angle);
				Transform roll = newFocus.Find("RollFrame");
				roll.GetComponent<Image>().color = standardManager.GetTiltColorResult(lh.tiltAngle);
				roll.Find("RollInfo").GetComponent<TMP_Text>().text = "Roll angle: " + lh.tiltAngle.ToString(standardManager.decimalRule) + " deg";
				pitch.Find("PitchInfo").GetComponent<TMP_Text>().text = "Pitch angle: " + lh.angle.ToString(standardManager.decimalRule) + " deg";
				roll.Find("RollImage").localEulerAngles = Vector3.back * lh.tiltAngle;
				pitch.Find("PitchImage").localEulerAngles = Vector3.forward * lh.angle;
			}
        }
	}

    #endregion

    #region Lighthouse General Function

	public void AssignRefreshRate(float input)
	{
		refreshRate = (int)input;
	}

	public void AudoRefresh(bool isAuto)
    {
		if (refreshRoutine != null)
			StopCoroutine(refreshRoutine);
		if (isAuto)
			refreshRoutine = StartCoroutine(RefreshRoutine());
    }

	IEnumerator RefreshRoutine()
    {
        while (true)
		{
			yield return new WaitForSeconds(refreshRate);
			GetLighthouses();
		}
    }


    public void SaveLighthouseData()
    {
		PlayerPrefs.SetString("LighthouseData", JsonUtility.ToJson(new LighthouseCollection(lhList)));
	}

	public void DeleteLighthouseData()
    {
		if (PlayerPrefs.HasKey("LighthouseData"))
			PlayerPrefs.DeleteKey("LighthouseData");
    }
    public void GetLighthouses()
	{
		lhUIList.ForEach(a => a.Clear());
		lhUIList.Clear();
		lhList.ForEach(a => a.Activate(pairList, false));
		lhList.Clear();
        pairList.Clear();
        pairList.Add(new Pair("Unpaired"));
		LighthouseCollection data = new LighthouseCollection();
		if (PlayerPrefs.HasKey("LighthouseData"))
		{
			data = JsonUtility.FromJson<LighthouseCollection>(PlayerPrefs.GetString("LighthouseData"));
		}
		if (!isTestMode)
		{
			try
			{
				var error = ETrackedPropertyError.TrackedProp_Success;
				ETrackedDeviceProperty propModel = ETrackedDeviceProperty.Prop_RenderModelName_String;
				ETrackedDeviceProperty propSerial = ETrackedDeviceProperty.Prop_SerialNumber_String;
				char nickname = 'A';
				for (uint i = 0; i < 16; i++)
				{
					var model = new System.Text.StringBuilder((int)64);
					OpenVR.System.GetStringTrackedDeviceProperty(i, propModel, model, 64, ref error);
					// OpenVR.System.pose

					if (model.ToString().Contains("basestation"))
					{
						var serial = new System.Text.StringBuilder((int)64);
						OpenVR.System.GetStringTrackedDeviceProperty(i, propSerial, serial, 64, ref error);
						var id = i;
						LHInfo info = new LHInfo(nickname++.ToString(), id, serial.ToString());
						lhList.Add(info);
						pairList[0].AddLH(info);
						TrackedDevicePose_t pose = new TrackedDevicePose_t(), gamePose = new TrackedDevicePose_t();
						OpenVR.Compositor.GetLastPoseForTrackedDeviceIndex(i, ref pose, ref gamePose);
						var actualPose = new SteamVR_Utils.RigidTransform();
						info.position = actualPose.pos;
						info.rotation = actualPose.rot.eulerAngles;
						//Debug.LogWarning(info.nickname + ": [pos: " + info.position.ToString() + "], [rot: " + info.rotation.ToString() + "]");
					}
				}
			}
			catch
			{
				data.lhList.ForEach(lh => lhList.Add(new LHInfo(lh)));
				lhList.ForEach(lh => pairList[0].AddLH(lh));
			}
		}
		else
		{
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			int count = UnityEngine.Random.Range(2, 9);
			char nickname = 'A';
			for (uint i = 0; i < count; i++)
			{
				var serial = "LH-" + i;
				var id = i;
				LHInfo info = new LHInfo(nickname++.ToString(), id, serial.ToString());
				lhList.Add(info);
				pairList[0].AddLH(info);
				info.position = new Vector3(UnityEngine.Random.Range(-3.0f, 3.0f), UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(-3.0f, 3.0f));
				info.rotation = new Vector3(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-60f, 60f), UnityEngine.Random.Range(-3.0f, 3.0f));
			}
		}
		pairList[0].lhList.Sort();
		maxPairLength = lhList.Count;
        int length = lhList.Count / 2;
        pairStrList = new string[length+1];
        pairStrList[0] = "NA";
        for (int i = 1; i <= length; ++i) {
          string str = "Pair-" + i.ToString();
          pairList.Add(new Pair(str));
          pairStrList[i] = str;
		}
		for (int i = 0; i < lhList.Count; ++i)
		{
			lhList[i].InstantiateInstance();
			lhList[i].SetIndex(pairList, 0);
		};
		if (data.lhList.Count > 0)
		{
			foreach (LHInfo lh in lhList)
			{
				LHInfo temp = data.lhList.Where(i => i.serial.Equals(lh.serial)).FirstOrDefault();
				if (temp != null && temp.nickname != "")
				{
					lh.nickname = temp.nickname;
					lh.pairIndex = pairStrList.Length < temp.pairIndex ? temp.pairIndex : 0;
					lh.SetIndex(pairList, temp.pairIndex);
					lh.Activate(pairList, temp.isActive);
				}
			}
		}
		UpdateLHList();
		UpdatePairList();
	}
    #endregion

    #region Serialized Class
    internal class LHUI
    {
		public Button serial;
		public TMP_Text deviceID;
		public Dropdown pair;
		public Toggle activity;
		public TMP_InputField nickname;
		public int index;


        public LHUI()
		{
			LighthouseEditor le = LighthouseEditor.instance;
			Transform serialTrans = GameObject.Instantiate(le.serialObj).transform;
			serialTrans.SetParent(le.listContainer);
			serialTrans.localScale = Vector3.one;
			serial = serialTrans.GetComponent<Button>();
			Transform deviceTrans = GameObject.Instantiate(le.deviceObj).transform;
			deviceTrans.SetParent(le.listContainer);
			deviceTrans.localScale = Vector3.one;
			deviceID = deviceTrans.GetComponent<TMP_Text>();
			Transform pairTrans = GameObject.Instantiate(le.pairObj).transform;
			pairTrans.SetParent(le.listContainer);
			pairTrans.localScale = Vector3.one;
			pair = pairTrans.GetComponent<Dropdown>();
			Transform activityTrans = GameObject.Instantiate(le.activityObj).transform;
			activityTrans.SetParent(le.listContainer);
			activityTrans.localScale = Vector3.one;
			activity = activityTrans.GetComponent<Toggle>();
			Transform nicknameTrans = GameObject.Instantiate(le.nicknameObj).transform;
			nicknameTrans.SetParent(le.listContainer);
			nicknameTrans.localScale = Vector3.one;
			nickname = nicknameTrans.GetComponent<TMP_InputField>();
		}

		public void Init(List<string> optionList, int i)
		{
			
			LighthouseEditor le = LighthouseEditor.instance;
			index = i;
			LHInfo info = le.lhList[i];
			serial.onClick.RemoveAllListeners();
			serial.onClick.AddListener(delegate { le.OnSeiralClick(serial); });
			serial.GetComponentInChildren<TMP_Text>().text = info.serial;
			deviceID.text = info.id.ToString();
			pair.ClearOptions();
			pair.AddOptions(optionList);
			pair.value = info.pairIndex;
			pair.onValueChanged.RemoveAllListeners();
			pair.onValueChanged.AddListener(delegate { le.OnPairEdit(pair); });
			activity.isOn = le.lhList[i].isActive;
			activity.onValueChanged.RemoveAllListeners();
			activity.onValueChanged.AddListener(delegate { le.OnActivityToggle(activity); });
			nickname.onEndEdit.RemoveAllListeners();
			nickname.onEndEdit.AddListener(delegate { le.OnTxtFieldEdit(nickname); });
			nickname.text = info.nickname;
		}

		public void Init(Button btn, TMP_Text device, Dropdown dropdown, Toggle tog, TMP_InputField nick, List<string> optionList, int i)
		{
			LighthouseEditor le = LighthouseEditor.instance;
			index = i;
			LHInfo info = le.lhList[i];
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(delegate { le.OnSeiralClick(btn); });
			btn.GetComponentInChildren<TMP_Text>().text = info.serial;
			serial = btn;
			device.text = info.id.ToString();
			deviceID = device;
			dropdown.ClearOptions();
			dropdown.AddOptions(optionList);
			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onValueChanged.AddListener(delegate { le.OnPairEdit(dropdown); });
			pair = dropdown;
			pair.value = info.pairIndex;
			tog.onValueChanged.RemoveAllListeners();
			tog.onValueChanged.AddListener(delegate { le.OnActivityToggle(tog); });
			activity = tog;
			nick.onEndEdit.RemoveAllListeners();
			nick.onEndEdit.AddListener(delegate { le.OnTxtFieldEdit(nick); });
			nick.text = info.nickname;
			nickname = nick;
		}

		public void SetIndex(int i)
		{
			LighthouseEditor le = LighthouseEditor.instance;
			index = i;
			LHInfo info = le.lhList[i];
			serial.GetComponentInChildren<TMP_Text>().text = info.serial;
			pair.value = info.pairIndex;
			nickname.text = info.nickname;
		}

		public void Clear()
		{
			serial.onClick.RemoveAllListeners();
			DestroyImmediate(serial.gameObject);
			pair.ClearOptions();
			pair.onValueChanged.RemoveAllListeners();
			DestroyImmediate(pair.gameObject);
			activity.onValueChanged.RemoveAllListeners();
			DestroyImmediate(activity.gameObject);
			nickname.onEndEdit.RemoveAllListeners();
			DestroyImmediate(nickname.gameObject);
			DestroyImmediate(deviceID.gameObject);
		}
    }

    [Serializable]
    public class LHInfo : IComparable<LHInfo> {
		    public string nickname;
            public uint id;
            public string serial;
            public int pairIndex = 0;
		    public GameObject lhInstance;
		    public Vector3 position;
		    public Vector3 rotation;
		    public bool _isActive = true;
		    public bool isActive
		    {
			    get
			    {
				    return _isActive;
			    }
		    }
		    public float height
		    {
			    get
			    {
				    return GetHeight();
			    }
		    }
		    public float angle
		    {
			    get
			    {
				    return GetAngle();
			    }
		    }
			
			public float tiltAngle
			{
				get
				{

					if (lhInstance != null)
						rotation = lhInstance.transform.eulerAngles;
					if (rotation.z > 180)
						return rotation.z - 360;
					else
						return rotation.z;
				}
			}
		    public LHInfo (string name, uint uid, string uSerial) {
			    nickname = name;
                id = uid;
                serial = uSerial;
            }

		    public LHInfo (LHInfo lh)
		    {
			    nickname = lh.nickname;
			    id = lh.id;
			    serial = lh.serial;
			    pairIndex = lh.pairIndex;
			    lhInstance = lh.lhInstance;
			    position = lh.position;
			    rotation = lh.rotation;
			    _isActive = lh._isActive;
		    }

		public int SetIndex (List<Pair> list, int index) {
			list[pairIndex].RemoveLH(this);
			list[pairIndex].lhList.Sort();
			list[index].AddLH(this);
			list[pairIndex].lhList.Sort();
			list[pairIndex].CalcDisMatrix();
			list[index].CalcDisMatrix();
			pairIndex = index;
			int maxL = 0;
			list.ForEach(pair => maxL = Mathf.Max(pair.GetValidLength(), maxL));
			return maxL;
		}

		float GetHeight ()
		{
			if (lhInstance != null)
				position = lhInstance.transform.position;
			return position.y;
		}

		float GetAngle()
		{
			if (lhInstance != null)
				rotation = lhInstance.transform.eulerAngles;
			if (rotation.x > 180)
				return rotation.x - 360;
			else
				return rotation.x;
		}

		public int Activate(List<Pair> list, bool input)
		{
			if (input == isActive)
				return SetIndex(list, pairIndex);
			else
				_isActive = input;
			if (input)
			{
				InstantiateInstance();
			}
			else
			{
				if (lhInstance != null)
					Destroy(lhInstance);
				lhInstance = null;
			}
			return SetIndex(list, pairIndex);
		}

		public void InstantiateInstance()
		{
			LighthouseEditor le = LighthouseEditor.instance;
			lhInstance = Instantiate(le.LighthouseInstance) as GameObject;
			lhInstance.transform.SetParent(LighthouseEditor.instance.root);
			lhInstance.name = serial;
			lhInstance.transform.position = position;
			lhInstance.transform.rotation = Quaternion.Euler(rotation);
			lhInstance.transform.Find("Title_View").GetComponent<TMP_Text>().text = "LH - " + nickname;
			lhInstance.transform.Find("Title_Main").GetComponent<TMP_Text>().text = "LH - " + nickname;
			int index = le.lhList.IndexOf(this);
			if (index < le.colorList.Count)
				lhInstance.transform.Find("IRZone").GetComponent<Renderer>().material.color = le.colorList[index];
			lhInstance.SetActive(true);
			if (LighthouseEditor.instance.isTestMode)
			{
				lhInstance.GetComponent<SteamVR_TrackedObject>().enabled = false;

			}
			else
			{
				lhInstance.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex((int)id);
			}
		}

		public int CompareTo(LHInfo other)
		{
			int value1 = (isActive) ? -100 : 0;
			int value2 = (other.isActive) ? -100 : 0;
			value1 += (int)id;
			value2 += (int)other.id;
			return value1 - value2;
		}
	}

	[Serializable]
	public class Pair {
		public string name;
		public bool paired = false;
		public bool showMatrix = false;
		public List<LHInfo> lhList = new List<LHInfo>();
		public int listMaxLength = 0;
		public string[][] distanceMatrix;

		public Pair(string uName) {
		  name = uName;
		}

		public void AddLH (LHInfo lh) {
		  if (!lhList.Contains(lh))
			lhList.Add(lh);
		}

		public void RemoveLH (LHInfo lh) {
		  if (lhList.Contains(lh))
			lhList.Remove(lh);
		}

		public int GetValidLength ()
		{
			int sum = 0;
			lhList.ForEach(lh => sum += lh.isActive ? 1 : 0);
			return sum;
		}

		public void CalcDisMatrix ()
		{
			int length = GetValidLength();
			distanceMatrix = new string[length + 1][];
			for (int i = 0; i <= length; ++i)
			{
				distanceMatrix[i] = new string[length + 1];
				int j = i - 1;
				if (j >= 0)
				{
					distanceMatrix[0][i] = lhList[j].nickname;
					distanceMatrix[i][0] = lhList[j].nickname;
				}
			}

			for (int i = 1; i <= length; ++i)
			{
				for (int j = 1; j <= length; ++j)
				{
					distanceMatrix[i][j] = Vector3.Distance(lhList[i - 1].position, lhList[j - 1].position).ToString();
				}
			}
		}
  }

	[Serializable]
	public class LighthouseCollection
	{
		public List<LHInfo> lhList = new List<LHInfo>();
		public LighthouseCollection (List<LHInfo> list)
		{
			lhList = list;
		}

		public LighthouseCollection() { }
	}

    #endregion

    #region Legacy Logic
    /*
	    private void OnDestroy()
    {
        Vector2 detailRectPos = new Vector2(detailWindowRect.x, detailWindowRect.y);
        PlayerPrefs.SetString("DetailRect", JsonUtility.ToJson(detailRectPos).ToString());
    }
	void DrawStandard (int unusedWindowID = 3)
    {
        GUILayout.BeginVertical("box");
		GUILayout.Label("<b><size=15>Standard rules</size></b>", style, GUILayout.Height(20));
        GUILayout.Label("LH Distance Threshold");
        string str = GUILayout.TextField(standardManager.standard.distanceThreshold.ToString(), GUILayout.Width(300));
        float.TryParse(str, out standardManager.standard.distanceThreshold);
        Vector2 height;
        Vector2 angle;
        for (int i = 0; i < standardManager.standard.heights.Count; ++i)
		{
			height = standardManager.standard.heights[i];
			angle = standardManager.standard.angles[i];
			int color = 0;
			GUILayout.BeginHorizontal();
            GUILayout.Label("Height Lower");
            str = GUILayout.TextField(height.x.ToString(), GUILayout.Width(300));
            float.TryParse(str, out height.x);
            GUILayout.Label("Height Upper");
            str = GUILayout.TextField(height.y.ToString(), GUILayout.Width(300));
            float.TryParse(str, out height.y);
            GUILayout.Label("Angle Lower");
            str = GUILayout.TextField(angle.x.ToString(), GUILayout.Width(300));
            float.TryParse(str, out angle.x);
            GUILayout.Label("Angle Upper");
            str = GUILayout.TextField(angle.y.ToString(), GUILayout.Width(300));
            float.TryParse(str, out angle.y);
            // height = EditorGUILayout.Vector2Field("Height Range",sm.standard.heights[i], GUILayout.Width(300));
            // angle = EditorGUILayout.Vector2Field("Angle Range", sm.standard.angles[i], GUILayout.Width(300));
            // color = GUILayout.Popup(sm.standard.colors[i], StandardManager.Colors , GUILayout.Width(100));
            if (GUILayout.Button("Del", GUILayout.Width(80), GUILayout.Height(30)))
				standardManager.DeleteRule(i);
			standardManager.EditRule(i, height, angle, color);
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal();
        GUILayout.Label("Height Lower");
        str = GUILayout.TextField(standardManager.tempH.x.ToString(), GUILayout.Width(300));
        float.TryParse(str, out standardManager.tempH.x);
        GUILayout.Label("Height Upper");
        str = GUILayout.TextField(standardManager.tempH.y.ToString(), GUILayout.Width(300));
        float.TryParse(str, out standardManager.tempH.y);
        GUILayout.Label("Angle Lower");
        str = GUILayout.TextField(standardManager.tempA.x.ToString(), GUILayout.Width(300));
        float.TryParse(str, out standardManager.tempA.x);
        GUILayout.Label("Angle Upper");
        str = GUILayout.TextField(standardManager.tempA.y.ToString(), GUILayout.Width(300));
        float.TryParse(str, out standardManager.tempA.y);
        // sm.tempC = GUILayout.Popup(sm.tempC, StandardManager.Colors, GUILayout.Width(100));
        if (GUILayout.Button("Add", GUILayout.Width(80), GUILayout.Height(30)))
			standardManager.AddRule(standardManager.tempH, standardManager.tempA, standardManager.tempC);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(GUILayout.Width(400));
		if (GUILayout.Button("Save"))
			standardManager.SaveStandard();
		if (GUILayout.Button("Reset"))
			standardManager.LoadStanddard();
		GUILayout.EndHorizontal();
        GUILayout.EndVertical();
	}
	void DetailLighthouseWindow(int unusedWindowID)
	{
		if (lhList.Count == 0)
		{
			onViewer = false;
			return;
		}
		if (tex_side == null)
			tex_side = Resources.Load<Texture>("Lighthouse_side");
		if (tex_front == null)
			tex_front = Resources.Load<Texture>("Lighthouse_Front");
		if (tex_angle == null)
			tex_angle = Resources.Load<Texture>("Lighthouse_Angle");
		LHInfo lh = lhList[viewerIndex];
		Vector2 pivotPoint = new Vector2(100, 195);
		GUILayout.Label("<b><size=20>" + lh.serial + "</size></b>", style, GUILayout.MinHeight(30));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Tilt Angle: <b><size=15>" + lh.rotation.z.ToString("0.00 deg") + "</size></b>", style, GUILayout.MinHeight(30));
		GUILayout.Label("Raw Angle: <b><size=15>" + lh.angle.ToString("0.00 deg") + "</size></b>", style, GUILayout.MinHeight(30));
		GUILayout.EndHorizontal();
		GUI.DrawTexture(new Rect(35, 130, 180, 180), tex_angle, ScaleMode.StretchToFill, true, 10.0F, standardManager.GetTiltColorResult(lh.rotation.z), 0, 0);
		GUI.DrawTexture(new Rect(235, 130, 180, 180), tex_angle, ScaleMode.StretchToFill, true, 10.0F, standardManager.GetAngleColorResult(lh.height, lh.angle), 0, 0);
		GUIUtility.RotateAroundPivot(lh.rotation.z, pivotPoint);
		GUI.DrawTexture(new Rect(35, 130, 130, 130), tex_front, ScaleMode.StretchToFill, true, 10.0F);
		GUIUtility.RotateAroundPivot(-lh.rotation.z, pivotPoint);
		pivotPoint = new Vector2(300, 195);
		GUIUtility.RotateAroundPivot(-lh.rotation.x, pivotPoint);
		GUI.DrawTexture(new Rect(235, 130, 130, 130), tex_side, ScaleMode.StretchToFill, true, 10.0F);
		GUIUtility.RotateAroundPivot(lh.rotation.x, pivotPoint);
		GUILayout.Label("", GUILayout.Height(250));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Height: <b><size=20>" + lh.height.ToString("0.00 m") + "</size></b>", style, GUILayout.MinHeight(30));
		GUILayout.Label("", GUILayout.Width(100));
		if (GUILayout.Button("Close"))
			onViewer = false;
		GUILayout.EndHorizontal();
		GUI.DragWindow();
	}

	void OverviewWindow(int unusedWindowID)
	{
		GUILayout.Label("", GUILayout.Width(500), GUILayout.Height(500));
		foreach (LHInfo lh in lhList)
		{
			if (lh.isActive)
			{
				Vector2 pivotPoint = new Vector2(lh.position.x * 25 + 240, lh.position.z * 25 + 240);
				GUIUtility.RotateAroundPivot(-lh.rotation.y, pivotPoint);
				GUI.DrawTexture(new Rect(lh.position.x * 25 + 250, lh.position.z * 25 + 250, 20, 20), tex_front, ScaleMode.StretchToFill);
				GUIUtility.RotateAroundPivot(lh.rotation.y, pivotPoint);
			}
		}
		GUI.DragWindow();
	}

	void OnGUI()
	{
		style = new GUIStyle(GUI.skin.label);
		style.richText = true;
		table = new GUIStyle(GUI.skin.box);
		table.richText = true;
		table.normal.textColor = style.normal.textColor;
		if (!PlayerPrefs.HasKey("LighthouseData") && !EditorApplication.isPlaying)
		{
			initialized = false;
			GUILayout.Label("<size=30>Since no previous light house data has been saved\nLight House Viewer will be shown after application starts!</size>", style, GUILayout.ExpandHeight(true));
			return;
		}
		else if (!initialized)
		{
			GetLighthouses();
			initialized = true;
		}
		else if (isPlaying != EditorApplication.isPlaying)
		{
			initialized = false;
			isPlaying = EditorApplication.isPlaying;
		}


		if (!EditorApplication.isPlaying)
			GUILayout.Label("<b><size=20><color=orange>Offline</color> Mode</size></b>", style, GUILayout.MinHeight(30));
		else
			GUILayout.Label("<b><size=20><color=green>Runtime</color> Mode</size></b>", style, GUILayout.MinHeight(30));
		GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
		if (GUILayout.Button("Overview"))
			onOverview = !onOverview;
		if (GUILayout.Button("Refresh"))
			GetLighthouses();
		if (GUILayout.Button("Save Data"))
			PlayerPrefs.SetString("LighthouseData", JsonUtility.ToJson(new LighthouseCollection(lhList)));
		if (GUILayout.Button("Delete Data"))
			PlayerPrefs.DeleteKey("LighthouseData");
		GUILayout.EndHorizontal();
		GUILayout.Label("", GUI.skin.horizontalSlider);


		scrollPos = GUILayout.BeginScrollView(scrollPos);

		GUILayout.Label("<b><size=20>Lighthouse Initialization</size></b>", style, GUILayout.MinHeight(30));
		EditorGUILayout.Space();

		string showStr = standardManager.show ? "Hide" : "Show";
		standardManager.show = EditorGUILayout.Foldout(standardManager.show, showStr + " Standard Editor", true);
		if (standardManager.show)
			DrawStandard();

		GUILayout.BeginHorizontal();
		GUILayout.Label("<size=12>Serial Number</size>", style, GUILayout.Width(150));
		GUILayout.Label("<size=12> Device ID</size>", style, GUILayout.Width(100));
		GUILayout.Label("<size=12> Pair Group</size>", style, GUILayout.Width(100));
		GUILayout.Label("<size=12>   Activity</size>", style, GUILayout.Width(100));
		GUILayout.Label("<size=12>   Nickname</size>", style, GUILayout.Width(100));

		GUILayout.EndHorizontal();

		for (int i = 0; i < lhList.Count; ++i)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(lhList[i].serial, GUILayout.Width(150)))
			{
				onViewer = true;
				viewerIndex = i;
			}
			// GUILayout.Label(lhList[i].serial, GUILayout.Width(150));
			GUILayout.Label(lhList[i].id.ToString(), GUILayout.Width(100));
			int preIndex = lhList[i].pairIndex;
			int index = EditorGUILayout.Popup(lhList[i].pairIndex, pairStrList, GUILayout.Width(100));
			if (preIndex != index && lhList[i].isActive)
				maxPairLength = lhList[i].SetIndex(pairList, index);
			bool isActive = EditorGUILayout.Toggle(lhList[i].isActive, GUILayout.Width(100));
			maxPairLength = lhList[i].Activate(pairList, isActive);
			lhList[i].nickname = GUILayout.TextField(lhList[i].nickname, GUILayout.Width(100));
			GUILayout.EndHorizontal();
		}
		GUILayout.Label("", GUI.skin.horizontalSlider);


		GUILayout.Label("<b><size=20>Lighthouse Information</size></b>", style, GUILayout.MinHeight(30));
		EditorGUILayout.Space();
		for (int i = pairList.Count - 1; i >= 0; --i)
		{
			int matrixL = pairList[i].GetValidLength();
			if (matrixL > 0)
			{
				GUILayout.Label("<i><size=15>" + pairList[i].name + "</size></i>", style, GUILayout.MinHeight(25));
				string show = pairList[i].showMatrix ? "Hide" : "Show";
				pairList[i].showMatrix = EditorGUILayout.Foldout(pairList[i].showMatrix, show + " Distance Matrix", true);
				if (pairList[i].showMatrix)
				{
					for (int k = 0; k <= matrixL; ++k)
					{
						GUILayout.BeginHorizontal(GUILayout.Width(80 * (matrixL + 1)));
						for (int l = 0; l <= matrixL; ++l)
						{
							string label = pairList[i].distanceMatrix[k][l];
							if (float.TryParse(label, out float n))
							{
								label = (n < standardManager.standard.distanceThreshold) ? "<color=lime>" + label + "</color>" : "<color=red>" + label + "</color>";
							}
							GUILayout.Label(label, table, GUILayout.Width(80), GUILayout.Height(20));
						}
						GUILayout.EndHorizontal();
					}
				}
				// GUILayout.EndToggleGroup();

				GUILayout.BeginHorizontal("Box");
				// labels
				GUILayout.BeginVertical("Box", GUILayout.Width(200));
				GUILayout.Label("<size=12>LH Serial #</size>", style);
				GUILayout.Label("<size=12>Device ID</size>", style);
				GUILayout.Label("<size=12>Height</size>", style);
				GUILayout.Label("<size=12>Angle</size>", style);
				GUILayout.EndVertical();
				for (int j = 0; j < maxPairLength; j++)
				{
					// labels
					bool inList = j < pairList[i].lhList.Count;
					LHInfo lh;
					if (inList && (lh = pairList[i].lhList[j]) != null && lh.isActive)
					{
						GUILayout.BeginVertical("Box", GUILayout.Width(150));
						GUILayout.Label(lh.serial);
						GUILayout.Label(lh.id.ToString());
						GUILayout.Label("<color=" + standardManager.GetAngleResult(lh.height, lh.angle) + ">" + lh.height.ToString("0.00 (m)") + "</color>", style);
						GUILayout.Label("<color=" + standardManager.GetAngleResult(lh.height, lh.angle) + ">" + lh.angle.ToString("0.00 (deg)") + "</color>", style);
						GUILayout.EndVertical();
					}
				}
				GUILayout.EndHorizontal();
			}
		}

		GUILayout.EndScrollView();
		if (GUILayout.Button("Test"))
		{
			Debug.Log(JsonUtility.ToJson(pairList[0]));
			Debug.Log(PlayerPrefs.GetString("LighthouseData"));
		}

		//BeginWindows();
		if (onViewer)
			detailWindowRect = GUILayout.Window(1, detailWindowRect, DetailLighthouseWindow, "Lighthouse Viewer");
		if (onOverview)
			overviewWindowRect = GUILayout.Window(2, overviewWindowRect, OverviewWindow, "Lighthouse Overview");
		//EndWindows();

	}
	*/

    #endregion

}
