using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
//Dynamic footstep system.
//Note there is a known issue where the footsteps object will occasionally duplicate itself, if this happens, delete the footsteps parent object and change a setting or move a node to refresh the footsteps
public class Footsteps : MonoBehaviour
{
    [Header("References")]
    public Footstep leftFootPrefab;
    public Transform[] nodes;

    [SerializeField]
    private Transform _footstepsParent;
    private Transform footstepsParent
    {
        get
        {
            if (_footstepsParent == null)
            {
                var go = new GameObject();
                go.name = "footsteps";
                go.transform.parent = transform;
                _footstepsParent = go.transform;
            }
            return _footstepsParent;
        }
    }

    [Header("Path")]
    public float spaceBetweenFootSteps = 0.35f;
    public float footstepDistanceFromPath = 0.25f;
    public bool leftFootFirst = false;

    [Header("Animation")]
    public float footstepFadeInTime = 0.25f;
    public float footstepFadeOutTime = 1;
    public float footstepStayTime = 0.3f;
    public float completeAnimationTime = 20;
    public bool hideOnStart = true;

    [Header("Debug")]
    [SerializeField]
    private Footstep[] footSteps = new Footstep[0];

    [SerializeField]
    private int animateMultipleNumLoops = 0;
    [SerializeField]
    private float animateMultipleTimeBetweenLoops = 4f;
    [SerializeField]
    private bool animateMultipleOnClick = false;
    [SerializeField]
    private bool animateOnceOnClick = false;
    [SerializeField]
    private bool stopAnimationOnClick = false;
    [SerializeField]
    private bool stopAnimationImmediateOnClick = false;
    [SerializeField]
    private bool showOnClick = false;
    [SerializeField]
    private bool hideOnClick = false;

    private void OnEnable()
    {
        if(hideOnStart && Application.isPlaying)
        { 
            Hide();
        }
    }

    /// <summary>
    /// Start an animation
    /// </summary>
    /// <param name="numTimes">if 0, will loop forever</param>
    /// <param name="timeBetween">time in seconds between loops</param>
    public void StartAnimation(int numTimes, float timeBetween)
    {
        StopAnimation();
        animateRoutine = StartCoroutine(Animate(numTimes, timeBetween));

        HideStartNode();
    }


    /// <summary>
    /// Stop the current animation slowly
    /// </summary>
    public void StopAnimation()
    {
        if(animateRoutine != null)
        {
            StopCoroutine(animateRoutine);
            animateRoutine = null;
        }
    }

    public void StopAnimationImmediate()
    {
        if (animateRoutine != null)
        {
            StopCoroutine(animateRoutine);
            animateRoutine = null;
        }
        if (animateSingleRoutines != null)
        {
            animateSingleRoutines.ForEach((routine) => StopCoroutine(routine));
            animateSingleRoutines.Clear();
        }
    }

    public void Show()
    {
        footSteps.ToList().ForEach((f) => f.Show());
    }

    public void Hide()
    {
        footSteps.ToList().ForEach((f) => f.Hide());
    }

    public void ShowStartNode()
    {
        nodes[0].gameObject.SetActive(true);
    }

    public void HideStartNode()
    {
        nodes[0].gameObject.SetActive(false);
    }

    public void ShowEndNode()
    {
        nodes[nodes.Length - 1].gameObject.SetActive(true);
    }

    public void HideEndNode()
    {
        nodes[nodes.Length - 1].gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (animateMultipleOnClick == true)
        {
            animateMultipleOnClick = false;

            if (Application.isPlaying)
                StartAnimation(animateMultipleNumLoops, animateMultipleTimeBetweenLoops);
            else
                EditorUtility.DisplayDialog("Error", "Cannot play animation in edit mode.", "Close");
            return;
        }

        if (animateOnceOnClick == true)
        {
            animateOnceOnClick = false;

            if (Application.isPlaying)
                StartAnimation(1, 0f);
            else
                EditorUtility.DisplayDialog("Error", "Cannot play animation in edit mode.", "Close");
            return;
        }

        if (stopAnimationOnClick == true)
        {
            stopAnimationOnClick = false;

            if (Application.isPlaying)
                StopAnimation();
            else
                EditorUtility.DisplayDialog("Error", "Cannot stop animation in edit mode.", "Close");
            return;
        }

        if (stopAnimationImmediateOnClick == true)
        {
            stopAnimationImmediateOnClick = false;

            if (Application.isPlaying)
                StopAnimationImmediate();
            else
                EditorUtility.DisplayDialog("Error", "Cannot stop animation in edit mode.", "Close");
            return;
        }

        if (showOnClick == true)
        {
            showOnClick = false;
            if (Application.isPlaying)
                Show();
            else
                EditorUtility.DisplayDialog("Error", "Cannot edit material in edit mode.", "Close");
            return;
        }

        if (hideOnClick == true)
        {
            hideOnClick = false;
            if (Application.isPlaying)
                Hide();
            else
                EditorUtility.DisplayDialog("Error", "Cannot edit material in edit mode.", "Close");
            return;
        }

        PlaceFootsteps();
    }

    private void Update()
    {
        bool shouldReplace = false;

        for(int i = 0; i < nodes.Length; i++)
        {
            var currNode = nodes[i];
            if (currNode.hasChanged)
            {
                //rotate start node toward next node
                if(i == 0 && i < nodes.Length - 1)
                {
                    currNode.rotation = Quaternion.LookRotation(nodes[i + 1].position - currNode.position, Vector3.up);
                }
                //rotate end node away from previous node
                else if(i > 0 && i == nodes.Length - 1)
                {
                    currNode.rotation = Quaternion.LookRotation(currNode.position - nodes[i- 1].position, Vector3.up);
                }
                shouldReplace = true;
                currNode.hasChanged = false;
            }
        }

        if (shouldReplace)
            PlaceFootsteps();
    }

    private void PlaceFootsteps()
    {
        if (footSteps.Length > 0)
            ClearFootsteps();

        var totalDistanceBetweenNodes = GetTotalDistanceBetweenNodes();
        footSteps = new Footstep[Mathf.FloorToInt(totalDistanceBetweenNodes / spaceBetweenFootSteps)];

        for(int i = 0; i < footSteps.Length; i++)
        {
            bool leftFoot = i % 2 == 0;

            if(!leftFootFirst)
            {
                leftFoot = !leftFoot;
            }

            var footInstance = Instantiate(leftFootPrefab, footstepsParent.transform);
            footSteps[i] = footInstance;

            PosRot posRot = GetPositionOnPath(((float)i / footSteps.Length) * totalDistanceBetweenNodes);

            footInstance.transform.position = posRot.position;
            footInstance.transform.rotation = posRot.rotation;
            footInstance.name = $"foot {i} {(leftFoot ? "left" : "right")}";

            if (!leftFoot)
            {
                footInstance.transform.localPosition += footInstance.transform.right * footstepDistanceFromPath;
                footInstance.transform.localScale = new Vector3(-footInstance.transform.localScale.x, footInstance.transform.localScale.y, footInstance.transform.localScale.z);
                //footInstance.GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
            else
            {
                footInstance.transform.localPosition -= footInstance.transform.right * footstepDistanceFromPath;
            }
        }

        if (hideOnStart && Application.isPlaying)
        {
            Hide();
        }
    }

    private void ClearFootsteps()
    {
        StartCoroutine(DestroyFootsteps(footSteps.ToList()));
        footSteps = new Footstep[0];
    }

    private IEnumerator DestroyFootsteps(List<Footstep> footsteps)
    {
        yield return null;
        footsteps.ForEach((g) => { if (g) DestroyImmediate(g.gameObject); });
    }

    Coroutine animateRoutine = null;
    List<Coroutine> animateSingleRoutines = new List<Coroutine>();

    private IEnumerator Animate(int numTimes, float timeBetween)
    {
        var waitBetween = new WaitForSeconds(timeBetween);

        bool animateForever = numTimes <= 0;

        int currentLoop = 0;

        while(currentLoop < numTimes || animateForever)
        {
            currentLoop++;

            animateSingleRoutines.Add(StartCoroutine(AnimateSingle()));
            yield return waitBetween;
        }
    }

    private IEnumerator AnimateSingle()
    {
        float currentAnimationTime = 0f;
        float timeSinceLastFootstep = 0f;

        var totalDistanceBetweenNodes = GetTotalDistanceBetweenNodes();
        var timeBetweenFootsteps = (spaceBetweenFootSteps / totalDistanceBetweenNodes) * completeAnimationTime;

        int currentFootstepIndex = -1;

        while (currentAnimationTime < completeAnimationTime)
        {
            currentAnimationTime += Time.deltaTime;
            timeSinceLastFootstep += Time.deltaTime;

            if (currentAnimationTime > completeAnimationTime)
                break;

            if (timeSinceLastFootstep > timeBetweenFootsteps)
            {
                timeSinceLastFootstep = 0f;
                currentFootstepIndex++;

                var currentFootstep = footSteps[currentFootstepIndex];

                currentFootstep.Animate(footstepFadeInTime, footstepStayTime, footstepFadeOutTime);
            }
            yield return null;
        }
    }

    private float GetTotalDistanceBetweenNodes()
    {
        float totalDistance = 0f;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            var currNode = nodes[i];
            var nextNode = nodes[i + 1];

            totalDistance += Mathf.Abs((currNode.position - nextNode.position).magnitude);
        }
        return totalDistance;
    }

    private PosRot GetPositionOnPath(float targetDistance)
    {
        float currDistance = 0f;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            var currNode = nodes[i];
            var nextNode = nodes[i + 1];

            var prevDistance = currDistance;
            var distanceBetweenNodes = Mathf.Abs((currNode.position - nextNode.position).magnitude);
            currDistance += distanceBetweenNodes;

            if (currDistance >= targetDistance)
            {
                return new PosRot(Vector3.Lerp(currNode.position, nextNode.position, (targetDistance - prevDistance) / distanceBetweenNodes), Quaternion.LookRotation(nextNode.position - currNode.position, Vector3.up));
            }
        }
        //fallback if failed
        return new PosRot(Vector3.zero, Quaternion.identity);
    }

    public struct PosRot
    {
        public Vector3 position;
        public Quaternion rotation;

        public PosRot(Vector3 _pos, Quaternion _rot)
        {
            position = _pos;
            rotation = _rot;
        }
    }
}