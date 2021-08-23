using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingularStandard : MonoBehaviour
{
    public TMP_InputField hLower, hUpper, aLower, aUpper;
    public int index;
    [SerializeField]
    Image bgm;
    StandardManager sm;

    private void OnEnable()
    {
        sm = StandardManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIndex(int _index)
    {
        index = _index;
        bgm.enabled = index % 2 == 0;
    }

    public void OnEditIn(Vector2 height, Vector2 angle)
    {
        hLower.text = height.x.ToString(sm.decimalRule);
        hUpper.text = height.y.ToString(sm.decimalRule);
        aLower.text = angle.x.ToString(sm.decimalRule);
        aUpper.text = angle.y.ToString(sm.decimalRule);
    }

    public void OnEditOut()
    {
        StandardManager sm = StandardManager.instance;
        float hl, hu, al, au;
        if (float.TryParse(hLower.text, out hl) && float.TryParse(hUpper.text, out hu) && float.TryParse(aLower.text, out al) && float.TryParse(aUpper.text, out au))
        {
            sm.EditRule(index, new Vector2(hl, hu), new Vector2(al, au), 0);
        }
    }

    public void OnDelete()
    {
        sm.DeleteRule(index);
        DestroyImmediate(this.gameObject);
    }
}
