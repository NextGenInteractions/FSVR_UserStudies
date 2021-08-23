using NextGen.Tools;
using NextGen.VrManager.Devices.Serials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchpadToCanvas : MonoBehaviour
{
    public Wristcuff wristcuff;
    public Vector2 size;
    public Vector2 Size { get { return new Vector2(size.x * transform.lossyScale.x, size.y * transform.lossyScale.y); } }
    public float scrollRectPanFactor = 1.5f;
    public GameObject startingScreen;

    public bool Touching { get { return wristcuff.Touching; } }
    public Vector2 CursorCoordinates { get { return wristcuff.Point; } }
    public Vector2 TweenedCursorCoordinates { get { return wristcuff.Point; } }

    private Vector3 Origin { get { return transform.TransformPoint(LocalOrigin); } }
    private Vector3 LocalOrigin { get { return Vector3.zero - (new Vector3(size.x, size.y, 0) / 2); } }
    public Vector3 CursorLocation { get { return ToWorldSpace(CursorCoordinates); } }
    public Vector3 LocalCursorLocation { get { return ToLocalSpace(CursorCoordinates); } }
    public Vector3 TweenedCursorLocation { get { return ToWorldSpace(TweenedCursorCoordinates); } }
    public Vector3 LocalTweenedCursorLocation { get { return ToLocalSpace(TweenedCursorCoordinates); } }

    List<IArduTouchElement> arduTouched = new List<IArduTouchElement>();
    bool triggerTap = false;

    void OnEnable()
    {
        TouchpadInterpreter.OnTap += Tap;
    }

    private void OnDisable()
    {
        TouchpadInterpreter.OnTap -= Tap;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set all screens to active, so even inactive screens in the hierarchy will auto-populate with ArduTouch components.
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        //Give all Buttons ArduTouch components.
        foreach (Button button in GetComponentsInChildren(typeof(Button), true))
        {
            button.onClick.AddListener(button.GetComponent<ArduTouchEventHandler>().Click);

            ArduTouchButton arduTouch = button.gameObject.AddComponent<ArduTouchButton>();
            arduTouch.SetColors(button.colors.normalColor, button.colors.highlightedColor, button.colors.pressedColor);

            RectTransform rectTransform = button.GetComponent<RectTransform>();
            SetPivotToCenter(rectTransform);

            BoxCollider box = button.gameObject.AddComponent<BoxCollider>();
            if (box.GetComponentInParent<GridLayoutGroup>() != null)
                box.size = box.transform.parent.GetComponent<GridLayoutGroup>().cellSize;
            else box.size = rectTransform.sizeDelta;
        }

        //Give all ScrollRects ArduTouch components.
        foreach (ScrollRect scrollRect in GetComponentsInChildren(typeof(ScrollRect), true))
        {
            ArduTouchScrollRect arduTouch = scrollRect.gameObject.AddComponent<ArduTouchScrollRect>();
            arduTouch.content = scrollRect.content;
            arduTouch.scaleFactor = scrollRectPanFactor;

            RectTransform rectTransform = scrollRect.GetComponent<RectTransform>();
            SetPivotToCenter(rectTransform);

            BoxCollider box = scrollRect.gameObject.AddComponent<BoxCollider>();
            if (box.GetComponentInParent<GridLayoutGroup>() != null)
                box.size = box.transform.parent.GetComponent<GridLayoutGroup>().cellSize;
            else box.size = rectTransform.sizeDelta;
        }

        foreach(Scrollbar scrollbar in GetComponentsInChildren(typeof(Scrollbar), true))
        {
            ArduTouchScrollbar arduTouch = scrollbar.gameObject.AddComponent<ArduTouchScrollbar>();

            RectTransform rectTransform = scrollbar.GetComponent<RectTransform>();
            SetPivotToCenter(rectTransform);

            BoxCollider box = scrollbar.gameObject.AddComponent<BoxCollider>();
            box.size = rectTransform.sizeDelta;
        }

        //Set all screens to inactive, except for the one screen specified as the "start screen" in the Inspector.
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        startingScreen.SetActive(true);
    }

    void SetPivotToCenter(RectTransform rectTransform)
    {
        Vector3 centerPoint = rectTransform.localPosition;
        Vector3 offset = Vector3.zero;
        //Top-left pivot. 
        if (rectTransform.pivot == new Vector2(0, 1)) offset = new Vector3(rectTransform.sizeDelta.x, -rectTransform.sizeDelta.y, 0);
        //Top-right pivot.
        if (rectTransform.pivot == new Vector2(1, 1)) offset = new Vector3(-rectTransform.sizeDelta.x, -rectTransform.sizeDelta.y, 0);
        //Bottom-left pivot. 
        if (rectTransform.pivot == new Vector2(0, 0)) offset = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, 0);
        //Bottom-right pivot.
        if (rectTransform.pivot == new Vector2(1, 0)) offset = new Vector3(-rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, 0);
        offset /= 2;
        //offset *= transform.lossyScale.z;
        centerPoint += offset;

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localPosition = centerPoint;
    }


    // Update is called once per frame
    void Update()
    {
        arduTouched.Clear();
        if (Touching)
        {
            RaycastHit[] hits = Physics.RaycastAll(CursorLocation, transform.forward, 5f * transform.lossyScale.z);
            foreach (RaycastHit hit in hits)
            {
                IArduTouchElement element = hit.transform.GetComponent<IArduTouchElement>();
                if (element != null)
                {
                    arduTouched.Add(element);
                    element.OnArduTouchHover();
                }
            }
        }

        if (triggerTap) TriggerTap();
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 1));
        Gizmos.DrawSphere(LocalOrigin, 0.25f);

        if (Application.isPlaying && Touching)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(LocalCursorLocation, 0.25f);
            Gizmos.DrawRay(LocalCursorLocation, Vector3.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(LocalTweenedCursorLocation, 0.25f);
            Gizmos.DrawRay(LocalTweenedCursorLocation, Vector3.forward);
        }
    }

    private void Tap(Vector2 _point)
    {
        triggerTap = true;
    }

    private void TriggerTap()
    {
        if (arduTouched.Count > 0)
        {
            foreach (IArduTouchElement element in arduTouched)
                element.OnArduTouchTap();
        }
        triggerTap = false;
    }

    private Vector3 ToLocalSpace(Vector2 _point)
    {
        return LocalOrigin + new Vector3(
            ((_point.x - 0.5f) / 13) * size.x,
            ((_point.y - 0.5f) / 9) * size.y,
            -0.5f
            );
    }

    private Vector3 ToWorldSpace(Vector2 _point)
    {
        return transform.TransformPoint(ToLocalSpace(_point));
    }
}