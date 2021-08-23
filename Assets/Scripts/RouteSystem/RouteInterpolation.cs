using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RouteInterpolation : MonoBehaviour
{
    enum ModelType
    {
        humanoid,
        car
    }

    enum AnimationState
    {
        Stop,
        StartWalking,
        Walking,
        EndWalking
    }
    private AnimationState preState = AnimationState.Stop;
    private AnimationState state = AnimationState.Stop;
    private float currentTime;
    private float lastTime = 0;
    [Header("General Settings")]
    [SerializeField]
    public float topSpeed = 10;
    [SerializeField]
    private float acceleration = 10;
    private float currentSpeed = 0;
    private float speedAmplifier = 2;
    private int accelOrDeccel = 1;

    public bool running = false;
    public float speedMultiplier = 2;

    public float CurrentSpeed { get { return currentSpeed; } }
    public float TopSpeed { get { return running ? topSpeed * speedMultiplier : topSpeed; } }

    [SerializeField]
    private bool walking = true;
    [SerializeField]
    private bool looping = true, endWithInitialPosition = true, drift = false;
    [SerializeField]
    private ModelType modelType = ModelType.car;
    private bool runningBuffer = true;
    private Coroutine animRoutine, animControllerRoutine;
    private bool coroutineRunning = false;
    private bool rotationCoroutineRunning = false;
    [SerializeField]
    private bool breakLoop = false;

    [Header("Animation Settings")]
    public string walkAnimation = "Walk";
    public string jogAnimation = "Walk";
    public string turnAnim_Walk_Right_Big = "MOB1_Walk_R_CIR_Loop_IPC";
    public string turnAnim_Walk_Left_Big = "MOB1_Walk_R_CIR_Loop_IPC";
    public string turnAnim_Walk_Right_Small = "MOB1_Walk_R_CIR_Loop_IPC";
    public string turnAnim_Walk_Left_Small = "MOB1_Walk_R_CIR_Loop_IPC";
    public string turnAnim_Jog_Right_Big = "MOB1_Walk_L_CIR_Loop_IPC";
    public string turnAnim_Jog_Left_Big = "MOB1_Walk_L_CIR_Loop_IPC";
    public string turnAnim_Jog_Right_Small = "MOB1_Walk_L_CIR_Loop_IPC";
    public string turnAnim_Jog_Left_Small = "MOB1_Walk_L_CIR_Loop_IPC";
    public string turnInPlace_Right = "MOB1_Stand_Rlx_Turn_In_Place_R_Loop_IPC";
    public string turnInPlace_Left = "MOB1_Stand_Rlx_Turn_In_Place_L_Loop_IPC";

    [Header("Track Settings")]
    [SerializeField]
    [Range(0, 1)]
    private float catmullRomAlpha = 0.5f;
    [SerializeField]
    private bool selfPosCounted = false;
    [SerializeField]
    private Transform routeRoot;

    [Header("Interaction Settings")]
    [SerializeField]
    private float steeringSensitivity = 5;
    [SerializeField]
    private float tireRollSensitivity = 10, tireTurnSensitivity = 2;
    [SerializeField]
    private GameObject steeringWheel, tireFrontLeft, tireFrontRight, tireRear;

    private string walkStr = "Walk";

    private string currentAnimStr = "Idle";

    [Header("Route Joints")]
    [SerializeField]
    private List<RouteNode> routeJoints = new List<RouteNode>();
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float animationSpeedFactor = 1;
    [SerializeField]
    private List<int> skipList = new List<int>();


    private List<ComputedWayPoint> wayPoints = new List<ComputedWayPoint>();

    private bool skipAction = false;
    private int skipActionIndex = 0;
    private AnimationSequel currentSequel;

    [Header("Debug")]
    [SerializeField]
    private bool debugWayPoints = false;
    [SerializeField]
    private WayPointDebugType debugWayPointType = WayPointDebugType.SOLID;
    [SerializeField]
    private float debugWayPointSize = 0.2f;
    [SerializeField]
    private Color debugWayPointColour = Color.green;
    [Header("Track")]
    [SerializeField]
    private bool debugTrack = true;
    [SerializeField]
    [Range(0, 1)]
    private float debugTrackResolution = 0.8f;
    [SerializeField]
    private Color debugTrackColour = Color.red;
    [SerializeField]
    private int currentIndex = 0;
    [System.Serializable]
    private class WayPoint
    {

        public Vector3 _pos;
        public Vector3 Pos
        {
            set
            {
                _pos = value;
                node.Pos = value;
            }
            get
            {
                // _pos = wayPointObject.transform.position;
                return _pos;
            }
        }
        public GameObject wayPointObject { get { return node.gameObject; } }
        [HideInInspector]
        public RouteNode node;
        public float _resistance;
        public float resistance
        {
            get => node.resistance;
            set
            {
                _resistance = (value <= 0) ? 0 : (value >= 1) ? 1 : value;
                node.resistance = _resistance;
            }
        }
    }

    private class ComputedWayPoint
    {
        public Vector3 pos;
        public float time;
        public RouteNode node;
        public List<AnimationVariable> motionAnimList { get { return node.motionAnimList; } }
        public List<AnimationVariable> pauseAnimList { get { return node.pauseAnimList; } }
    }

    [System.Serializable]
    private enum WayPointDebugType
    {
        SOLID,
        WIRE
    }

    //private void Start()
    //{
    //	wayPoints.Sort((x, y) => x.time.CompareTo(y.time));
    //	wayPoints.Insert(0, wayPoints[0]);
    //	wayPoints.Add(wayPoints[wayPoints.Count - 1]);
    //}

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
            RePopulateWayPoints();
        //This means that if currentTime is paused, then resumed, there is not a big jump in time
        if (runningBuffer != walking)
        {
            runningBuffer = walking;
            lastTime = Time.time;
        }

        currentSpeed += acceleration * Time.deltaTime * accelOrDeccel;

        if (currentSpeed > TopSpeed)
            currentSpeed = TopSpeed;
        else if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
        AddCurrentTime(Time.deltaTime * currentSpeed / speedAmplifier);
        if (walking)
        {
            accelOrDeccel = 1;
            lastTime = Time.time;
            //if (animator != null)
            //{
            //	float a = (animator.GetFloat("WalkingSpeed") + Time.deltaTime * (currentSpeed / topSpeed)) * animationSpeedFactor;
            //	animator.SetFloat("WalkingSpeed", a);
            //}
            if (currentSpeed <= 0 && (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !animator.GetNextAnimatorStateInfo(0).IsName("Idle")) && modelType == ModelType.humanoid && state == AnimationState.Stop)
            {
                currentAnimStr = "Idle";
                animator.CrossFade("Idle", 0.15f);
                preState = AnimationState.Stop;
            }
            else if (currentSpeed > 0 && (!animator.GetCurrentAnimatorStateInfo(0).IsName(walkStr) && !animator.GetNextAnimatorStateInfo(0).IsName(walkStr)) && modelType == ModelType.humanoid && walkStr != currentAnimStr)
            {
                if (IsWalkString() && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.95f)
                {
                    animator.CrossFade(walkStr, 0.05f);
                    currentAnimStr = walkStr;
                }
                else if (!IsWalkString())
                {
                    animator.CrossFade(walkStr, 0.15f);
                    currentAnimStr = walkStr;
                }
            }
            if (state != preState)
            {
                if (state == AnimationState.Stop)
                {

                }
                else if (state == AnimationState.StartWalking)
                {
                    preState = AnimationState.StartWalking;
                }
                else if (state == AnimationState.EndWalking)
                {
                    preState = AnimationState.EndWalking;
                }
            }
        }
        else
        {
            accelOrDeccel = -1;
            //if (animator != null)
            //{
            //	float a = animator.GetFloat("WalkingSpeed") + ((1 - animator.GetFloat("WalkingSpeed") % 1) * Time.deltaTime);
            //	animator.SetFloat("WalkingSpeed", a);
            //}
            if (modelType == ModelType.humanoid)
                currentSpeed = 0;
        }
        Vector3 nextPos = GetPosition(currentTime);
        float angle = Vector3.Angle(transform.forward, nextPos - transform.position);
        if (walking)
        {
            bool turnRight = Vector3.Cross(transform.rotation * Vector3.forward, Quaternion.LookRotation(nextPos - transform.position, Vector3.up) * Vector3.forward).y < 0;
            SetWalkString(angle, turnRight);
        }
        Vector3 vector = (walking || !drift) ? nextPos - transform.position : wayPoints[ClampListPos(currentIndex + 1)].pos - transform.position;
        Quaternion turningAngle = vector == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(vector, Vector3.up);
        // Quaternion turningAngle = Quaternion.LookRotation(nextPos - transform.position, Vector3.up);
        Quaternion tireRotation = Quaternion.Lerp(transform.rotation, turningAngle, Time.deltaTime * tireTurnSensitivity);
        if (modelType == ModelType.car)
        {
            vector = nextPos - transform.position;
            Quaternion tireTurn = vector == Vector3.zero ? Quaternion.identity : Quaternion.LerpUnclamped(Quaternion.identity, Quaternion.LookRotation(vector, Vector3.up) * Quaternion.Inverse(transform.rotation), tireTurnSensitivity);
            float tireSpeed = Vector3.Distance(nextPos, transform.position);
            tireRear.transform.Rotate(tireRollSensitivity * tireSpeed, 0, 0, Space.Self);
            tireFrontLeft.transform.rotation = Mathf.Abs(angle) > 5 ? tireRotation * tireTurn : tireRotation;
            tireFrontRight.transform.rotation = Mathf.Abs(angle) > 5 ? tireRotation * tireTurn : tireRotation;
            float tireRollValue = tireRear.transform.localEulerAngles.x;
            tireFrontLeft.transform.Rotate(tireRollValue, 0, 0, Space.Self);
            tireFrontRight.transform.Rotate(tireRollValue, 0, 0, Space.Self);
            Vector3 eularWheel = steeringWheel.transform.localEulerAngles;
            eularWheel.z = angle * steeringSensitivity;
            steeringWheel.transform.localEulerAngles = eularWheel;
        }
        if (walking)
            transform.rotation = tireRotation;
        if (currentSpeed > 0)
            transform.position = nextPos; // Vector3.Lerp(transform.position, nextPos, Time.deltaTime);
    }
    private List<float> lengths = new List<float>();
    private List<float> speeds = new List<float>();
    private List<float> accels = new List<float>();
    public float spdInit = 0;

    private void Start()
    {
        wayPoints.Sort((x, y) => x.time.CompareTo(y.time));
        for (int seg = 1; seg < wayPoints.Count - 2; seg++)
        {
            Vector3 p0 = wayPoints[seg - 1].pos;
            Vector3 p1 = wayPoints[seg].pos;
            Vector3 p2 = wayPoints[seg + 1].pos;
            Vector3 p3 = wayPoints[seg + 2].pos;
            float len = 0.0f;
            Vector3 prevPos = GetCatmullRomPosition(0.0f, p0, p1, p2, p3, catmullRomAlpha);
            for (int i = 1; i <= Mathf.FloorToInt(1f / debugTrackResolution); i++)
            {
                Vector3 pos = GetCatmullRomPosition(i * debugTrackResolution, p0, p1, p2, p3, catmullRomAlpha);
                len += Vector3.Distance(pos, prevPos);
                prevPos = pos;
            }
            float spd0 = seg == 1 ? spdInit : speeds[seg - 2];
            float lapse = wayPoints[seg + 1].time - wayPoints[seg].time;
            float acc = (len - spd0 * lapse) * 2 / lapse / lapse;
            float speed = spd0 + acc * lapse;
            lengths.Add(len);
            speeds.Add(speed);
            accels.Add(acc);
        }
        PopulateWayPoints();
    }

    private void PopulateWayPoints()
    {
        if (routeJoints.Count == 0)
            return;
        wayPoints.Clear();
        wayPoints.Add(new ComputedWayPoint()
        {
            pos = transform.position,
            time = 0,
            node = routeJoints[0]
        });
        for (int i = 0; i < routeJoints.Count; ++i)
        {
            wayPoints.Add(new ComputedWayPoint()
            {
                pos = routeJoints[i].Pos,
                time = wayPoints[wayPoints.Count - 1].time + Vector3.Distance(wayPoints[wayPoints.Count - 1].pos, routeJoints[i].Pos) * (1 + routeJoints[i].resistance),
                node = routeJoints[i]
            });
        }
        if (looping || endWithInitialPosition)
            wayPoints.Add(new ComputedWayPoint()
            {
                pos = wayPoints[0].pos,
                time = wayPoints[wayPoints.Count - 1].time + Vector3.Distance(wayPoints[0].pos, wayPoints[wayPoints.Count - 1].pos) * (1 + routeJoints[0].resistance),
                node = routeJoints[0]
            });
    }

    public void RePopulateWayPoints()
    {
        for (int i = 1; i < wayPoints.Count; ++i)
        {
            wayPoints[i].pos = (i < wayPoints.Count - 1) ? wayPoints[i].node.gameObject.transform.position : wayPoints[i].pos;
            wayPoints[i].time = wayPoints[i - 1].time + Vector3.Distance(wayPoints[i - 1].pos, wayPoints[i].pos) * (1 + wayPoints[i].node.resistance);
        }
    }

    private float AddCurrentTime(float deficit)
    {
        currentTime += deficit;
        if (currentTime > wayPoints[wayPoints.Count - 1].time)
        {
            currentTime -= wayPoints[wayPoints.Count - 1].time;
        }
        return currentTime;
    }

    private float GetNextTime(float deficit)
    {
        float nextTime = currentTime + deficit;
        if (nextTime > wayPoints[wayPoints.Count - 1].time)
        {
            nextTime -= wayPoints[wayPoints.Count - 1].time;
        }
        return nextTime;
    }
    private bool IsSkipping(int index)
    {
        return skipList.Contains(index);
    }

    private bool IsWalkString()
    {
        string[] walkStrs = new string[]
        {
            walkAnimation,
            jogAnimation,
            turnAnim_Jog_Left_Big,
            turnAnim_Jog_Left_Small,
            turnAnim_Jog_Right_Big,
            turnAnim_Jog_Right_Small,
            turnAnim_Walk_Left_Big,
            turnAnim_Walk_Left_Small,
            turnAnim_Walk_Right_Big,
            turnAnim_Walk_Right_Small
        };
        foreach (string str in walkStrs)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(str))
                return true;
        }
        return false;
    }

    private void SetWalkString(float angle, bool isRight)
    {
        if (angle > 10)
            walkStr = isRight ? ((running) ? turnAnim_Jog_Right_Big : turnAnim_Walk_Right_Big) : ((running) ? turnAnim_Jog_Left_Big : turnAnim_Walk_Left_Big);
        else if (angle > 5)
            walkStr = isRight ? ((running) ? turnAnim_Jog_Right_Small : turnAnim_Walk_Right_Small) : ((running) ? turnAnim_Jog_Left_Small : turnAnim_Walk_Left_Small);
        //else if (angle > 5)
        //    walkStr = isRight ? "MOB1_Walk_R_90_IPC" : "MOB1_Walk_L_90_IPC";
        else
            walkStr = running ? jogAnimation : walkAnimation;
    }

    #region public designer methods
    public void StartCar()
    {
        walking = true;
        accelOrDeccel = 1;
        if (animControllerRoutine != null)
            StopCoroutine(animControllerRoutine);
        if (animRoutine != null)
            StopCoroutine(animRoutine);
        // PlayMotionAnimation(currentIndex);
    }

    public void StopCar()
    {
        walking = false;
        accelOrDeccel = -1;
        if (modelType == ModelType.humanoid)
            currentSpeed = 0;
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void SetLooping(bool loop)
    {
        looping = loop;
    }

    public void Resume()
    {
        breakLoop = true;
    }
    public void AddSkipIndex(int index)
    {
        if (!skipList.Contains(index))
            skipList.Add(index);
    }

    public void LoadRouteRoot(Transform root)
    {
        lastTime = Time.time;
        currentIndex = 0;
        currentTime = 0;
        routeRoot = root;
        skipList.Clear();
        routeJoints.Clear();
        for (int i = 0; i < routeRoot.childCount; ++i)
            routeJoints.Add(routeRoot.GetChild(i).GetComponent<RouteNode>());
        PopulateWayPoints();
    }

    public void SetOnNode(int index, float offset = 0, bool isFullSpeed = false)
    {
        if (index >= 0 && index < wayPoints.Count)
        {
            if (animControllerRoutine != null)
                StopCoroutine(animControllerRoutine);
            if (animRoutine != null)
                StopCoroutine(animRoutine);
            walking = true;
            if (isFullSpeed)
                currentSpeed = TopSpeed;
            currentTime = wayPoints[index].time + offset;
        }
    }

    public void RemoveSkipIndex(int index)
    {
        if (skipList.Contains(index))
            skipList.Remove(index);
    }

    public void SkipToAction(int index)
    {
        skipAction = true;
        skipActionIndex = index;
    }

    public void ExitActionSequel()
    {
        if (currentSequel != null)
            SkipToAction(currentSequel.stateList.Count);
    }

    public void SkipToLastAction()
    {
        if (currentSequel != null)
            SkipToAction(currentSequel.stateList.Count - 1);
    }
    public void SkipToFirstAction()
    {
        SkipToAction(0);
    }
    #endregion

    //public Vector3 GetPosition(float time)
    //{
    //	//Check if before first waypoint
    //	if (time <= wayPoints[0].time)
    //	{
    //		return wayPoints[0].pos;
    //	}
    //	//Check if after last waypoint
    //	else if (time >= wayPoints[wayPoints.Count - 1].time)
    //	{
    //		return wayPoints[wayPoints.Count - 1].pos;
    //	}

    //	//Check time boundaries - Find the nearest WayPoint your object has passed
    //	float minTime = -1;
    //	// float maxTime = -1;
    //	int minIndex = -1;
    //	for (int i = 1; i < wayPoints.Count; i++)
    //	{
    //		if (time > wayPoints[i - 1].time && time <= wayPoints[i].time)
    //		{
    //			// maxTime = wayPoints[i].time;
    //			int index = i - 1;
    //			minTime = wayPoints[index].time;
    //			minIndex = index;
    //		}
    //	}

    //	float spd0 = minIndex == 1 ? spdInit : speeds[minIndex - 2];
    //	float len = lengths[minIndex - 1];
    //	float acc = accels[minIndex - 1];
    //	float t = time - minTime;
    //	float posThroughSegment = spd0 * t + acc * t * t / 2;
    //	float percentageThroughSegment = posThroughSegment / len;

    //	//Define the 4 points required to make a Catmull-Rom spline
    //	Vector3 p0 = wayPoints[ClampListPos(minIndex - 1)].pos;
    //	Vector3 p1 = wayPoints[minIndex].pos;
    //	Vector3 p2 = wayPoints[ClampListPos(minIndex + 1)].pos;
    //	Vector3 p3 = wayPoints[ClampListPos(minIndex + 2)].pos;

    //	return GetCatmullRomPosition(percentageThroughSegment, p0, p1, p2, p3, catmullRomAlpha);
    //}

    #region Animation Routine
    public void PlayMotionAnimation(int index)
    {

        if (animator != null)
            foreach (AnimationVariable animV in wayPoints[index].motionAnimList)
            {
                switch (animV.type)
                {
                    case AnimationVariable.Type.Boolean:
                        animator.SetBool(animV.variableName, animV.Boolean);
                        break;
                    case AnimationVariable.Type.Intager:
                        animator.SetInteger(animV.variableName, animV.Integer);
                        break;
                    case AnimationVariable.Type.Float:
                        animator.SetFloat(animV.variableName, animV.Float);
                        break;
                    case AnimationVariable.Type.Trigger:
                        animator.SetTrigger(animV.variableName);
                        break;
                }
            }
    }

    public void PlayPauseAnimation(int index)
    {

        if (animator != null)
            foreach (AnimationVariable animV in wayPoints[index].pauseAnimList)
            {
                switch (animV.type)
                {
                    case AnimationVariable.Type.Boolean:
                        animator.SetBool(animV.variableName, animV.Boolean);
                        break;
                    case AnimationVariable.Type.Intager:
                        animator.SetInteger(animV.variableName, animV.Integer);
                        break;
                    case AnimationVariable.Type.Float:
                        animator.SetFloat(animV.variableName, animV.Float);
                        break;
                    case AnimationVariable.Type.Trigger:
                        animator.SetTrigger(animV.variableName);
                        break;
                }
            }
    }

    public void EnterNode()
    {
        if (wayPoints[currentIndex].node.nodeType == RouteNode.NodeType.StopPoint && animator != null)
        {
            if (IsSkipping(currentIndex))
            {
                RemoveSkipIndex(currentIndex);
                return;
            }
            if (animControllerRoutine != null)
                StopCoroutine(animControllerRoutine);
            if (animRoutine != null)
                StopCoroutine(animRoutine);
            animControllerRoutine = StartCoroutine(EnterNodeAnimationSequel());
        }
    }

    IEnumerator EnterNodeAnimationSequel()
    {
        walking = false;
        breakLoop = false;
        /// Play Entry animation
        animRoutine = StartCoroutine(PlayAnimationRoutine(wayPoints[currentIndex].node.enterNodeSequel));
        yield return new WaitUntil(() => !coroutineRunning);

        /// Play rotation animation to adjust rotation
        if (wayPoints[currentIndex].node.faceNextNode)
        {
            animRoutine = StartCoroutine(RootRotation(wayPoints[ClampListPos(currentIndex + 1)].pos));
            yield return new WaitUntil(() => !rotationCoroutineRunning);
        }

        if (wayPoints[currentIndex].node.gameObject.transform.childCount > 0)
        {
            animRoutine = StartCoroutine(RootRotation(wayPoints[currentIndex].node.gameObject.transform.GetChild(0).position));
            yield return new WaitUntil(() => !rotationCoroutineRunning);
        }

        /// Play OnNode animation
        animRoutine = StartCoroutine(PlayAnimationRoutine(wayPoints[currentIndex].node.onNodeSequel));
        yield return new WaitUntil(() => !coroutineRunning);

        /// Play rotation animation to adjust rotation
        //animRoutine = StartCoroutine(RootRotation(GetPosition(GetNextTime(0.5f))));
        //yield return new WaitUntil(() => !rotationCoroutineRunning);

        /// Play Exit animation
        animRoutine = StartCoroutine(PlayAnimationRoutine(wayPoints[currentIndex].node.exitNodeSequel));
        yield return new WaitUntil(() => !coroutineRunning);
        walking = true;
        breakLoop = false;
        // animator.CrossFade("Idle_To_Walk", 0.15f, 0, 0.5f);
    }

    IEnumerator RootRotation(Vector3 nextPos)
    {
        rotationCoroutineRunning = true;
        Vector3 cross = Vector3.Cross(transform.rotation * Vector3.forward, Quaternion.LookRotation(nextPos - transform.position, Vector3.up) * Vector3.forward);
        float turnRight = cross.y < 0 ? -20 : 20;
        float rotateDirection = (((transform.rotation.eulerAngles.y - Quaternion.LookRotation(nextPos - transform.position).eulerAngles.y) + 360f) % 360f) > 180.0f ? 20 : -20;
        if (Vector3.Angle(transform.forward, nextPos - transform.position) > 5)
        {
            animator.CrossFade(rotateDirection > 0 ? turnInPlace_Right : turnInPlace_Left, 0.05f, 0, 0f);
            yield return new WaitUntil(() =>
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - transform.position), Time.deltaTime * tireTurnSensitivity);
                float angle = Vector3.Angle(transform.forward, nextPos - transform.position);
                if (angle < 5)
                {
                    transform.rotation = Quaternion.LookRotation(nextPos - transform.position);
                    return true;
                }
                else
                    return false;
            });
        }
        rotationCoroutineRunning = false;

    }

    IEnumerator PlayAnimationRoutine(AnimationSequel sequel)
    {
        coroutineRunning = true;
        float time = Time.time;
        float timer = sequel.time;
        int count = 0;
        int numberOfRun = sequel.numberOfRun;
        currentSequel = sequel;
        while (count < sequel.stateList.Count)
        {
            string[] cmd = sequel.stateList[count].Split(' ');
            if (cmd.Length >= 3 && cmd[0].Equals("CMD"))
            {
                switch (cmd[1])
                {
                    case "FACE":
                        int index = 0;
                        bool faceChild = int.TryParse(cmd[2], out index);
                        Coroutine rotationRoutine;
                        if (faceChild)
                            rotationRoutine = StartCoroutine(RootRotation(wayPoints[currentIndex].node.gameObject.transform.GetChild(index).position));
                        else
                            rotationRoutine = StartCoroutine(RootRotation(GetPosition(GetNextTime(0.5f))));
                        yield return new WaitUntil(() => {
                            if (skipAction)
                            {
                                count = skipActionIndex - 1;
                                skipAction = false;
                                StopCoroutine(rotationRoutine);
                                rotationCoroutineRunning = false;
                            }
                            return !rotationCoroutineRunning;
                        });
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName(sequel.stateList[count]))
                    animator.Play(sequel.stateList[count], 0, 0f);
                else
                    animator.CrossFade(sequel.stateList[count], 0.15f, 0, 0f);
                currentAnimStr = sequel.stateList[count];
                yield return new WaitUntil(() => {
                    if (skipAction)
                    {
                        count = skipActionIndex - 1;
                        skipAction = false;
                        return true;
                    }
                    // Debug.LogWarning("Start Time: " + time + "; Current Time: " + Time.time);
                    if (sequel.routine == AnimationSequel.PlayRoutine.LoopForTime && timer <= Time.time - time)
                    {
                        count = sequel.stateList.Count;
                        return true;
                    }
                    else
                        return animator.GetCurrentAnimatorStateInfo(0).IsName(sequel.stateList[count]) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.85f;
                }
                );
            }
            ++count;
            if (count >= sequel.stateList.Count && sequel.routine != AnimationSequel.PlayRoutine.Once)
            {
                if (sequel.routine == AnimationSequel.PlayRoutine.LoopForNumber || (sequel.routine == AnimationSequel.PlayRoutine.LoopForTime && timer >= Time.time - time) || (sequel.routine == AnimationSequel.PlayRoutine.Loop && !breakLoop))
                    count = 0;
                if (sequel.routine == AnimationSequel.PlayRoutine.LoopForNumber)
                {
                    --numberOfRun;
                    if (numberOfRun <= 0)
                        count = sequel.stateList.Count;
                }
            }
        }
        currentSequel = null;
        coroutineRunning = false;
    }
    #endregion

    #region Catmull-Rom Math
    public Vector3 GetPosition(float time)
    {
        //Check if before first waypoint
        if (time <= wayPoints[0].time)
        {
            return wayPoints[0].pos;
        }
        //Check if after last waypoint
        else if (time >= wayPoints[wayPoints.Count - 1].time)
        {
            return wayPoints[wayPoints.Count - 1].pos;
        }

        //Check time boundaries - Find the nearest WayPoint your object has passed
        float minTime = -1;
        float maxTime = -1;
        int minIndex = -1;
        for (int i = 1; i < wayPoints.Count; i++)
        {
            if (time > wayPoints[i - 1].time && time <= wayPoints[i].time)
            {
                maxTime = wayPoints[i].time;
                int index = i - 1;
                minTime = wayPoints[index].time;
                minIndex = index;
            }
        }
        if (currentIndex != minIndex)
        {
            // PlayMotionAnimation(currentIndex);
            currentIndex = minIndex;
            EnterNode();
        }

        float timeDiff = maxTime - minTime;
        float percentageThroughSegment = 1 - ((maxTime - time) / timeDiff);

        //Define the 4 points required to make a Catmull-Rom spline
        Vector3 p0 = selfPosCounted ? transform.position : wayPoints[ClampListPos(minIndex - 1)].pos;
        Vector3 p1 = wayPoints[minIndex].pos;
        Vector3 p2 = wayPoints[ClampListPos(minIndex + 1)].pos;
        Vector3 p3 = wayPoints[ClampListPos(minIndex + 2)].pos;

        return GetCatmullRomPosition(percentageThroughSegment, p0, p1, p2, p3, catmullRomAlpha);
    }

    //Prevent Index Out of Array Bounds
    private int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = wayPoints.Count - 1;
        }

        if (pos >= wayPoints.Count)
        {
            pos = pos % wayPoints.Count;
        }
        else if (pos > wayPoints.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }

    //Math behind the Catmull-Rom curve. See here for a good explanation of how it works. https://stackoverflow.com/a/23980479/4601149
    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float alpha)
    {
        float dt0 = GetTime(p0, p1, alpha);
        float dt1 = GetTime(p1, p2, alpha);
        float dt2 = GetTime(p2, p3, alpha);

        Vector3 t1 = ((p1 - p0) / dt0) - ((p2 - p0) / (dt0 + dt1)) + ((p2 - p1) / dt1);
        Vector3 t2 = ((p2 - p1) / dt1) - ((p3 - p1) / (dt1 + dt2)) + ((p3 - p2) / dt2);

        t1 *= dt1;
        t2 *= dt1;

        Vector3 c0 = p1;
        Vector3 c1 = t1;
        Vector3 c2 = (3 * p2) - (3 * p1) - (2 * t1) - t2;
        Vector3 c3 = (2 * p1) - (2 * p2) + t1 + t2;
        Vector3 pos = CalculatePosition(t, c0, c1, c2, c3);

        return pos;
    }

    private float GetTime(Vector3 p0, Vector3 p1, float alpha)
    {
        if (p0 == p1)
            return 1;
        return Mathf.Pow((p1 - p0).sqrMagnitude, 0.5f * alpha);
    }

    private Vector3 CalculatePosition(float t, Vector3 c0, Vector3 c1, Vector3 c2, Vector3 c3)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return c0 + c1 * t + c2 * t2 + c3 * t3;
    }

    //Utility method for drawing the track
    private void DisplayCatmullRomSpline(int pos, float resolution, Color begin = new Color(), Color end = new Color())
    {
        Vector3 p0 = wayPoints[ClampListPos(pos - 1)].pos;
        Vector3 p1 = wayPoints[pos].pos;
        Vector3 p2 = wayPoints[ClampListPos(pos + 1)].pos;
        Vector3 p3 = wayPoints[ClampListPos(pos + 2)].pos;

        Vector3 lastPos = p1;
        int maxLoopCount = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= maxLoopCount; i++)
        {
            float t = i * resolution;
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3, catmullRomAlpha);
            Gizmos.color = Color.Lerp(begin, end, t);
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            if (routeRoot != null && routeRoot.childCount != routeJoints.Count)
            {
                LoadRouteRoot(routeRoot);
            }
            PopulateWayPoints();
        }
        if (debugWayPoints)
        {
            Gizmos.color = debugWayPointColour;
            foreach (ComputedWayPoint s in wayPoints)
            {
                if (debugWayPointType == WayPointDebugType.SOLID)
                {
                    Gizmos.DrawSphere(s.pos, debugWayPointSize);
                }
                else if (debugWayPointType == WayPointDebugType.WIRE)
                {
                    Gizmos.DrawWireSphere(s.pos, debugWayPointSize);
                }
            }
        }

        if (debugTrack)
        {
            Color initialColor = Color.green;
            Gizmos.color = debugTrackColour;
            if (wayPoints.Count >= 2)
            {
                for (int i = 0; i < wayPoints.Count - 1; i++)
                {
                    //if (i == 0 || i == wayPoints.Count - 2 || i == wayPoints.Count - 1)
                    //{
                    //	continue;
                    //}
                    DisplayCatmullRomSpline(i, debugTrackResolution, Color.Lerp(initialColor, debugTrackColour, (i == 0) ? 0 : wayPoints[i - 1].time / wayPoints[wayPoints.Count - 1].time), Color.Lerp(initialColor, debugTrackColour, wayPoints[i].time / wayPoints[wayPoints.Count - 1].time));
                }
            }
        }
#endif
    }
}
