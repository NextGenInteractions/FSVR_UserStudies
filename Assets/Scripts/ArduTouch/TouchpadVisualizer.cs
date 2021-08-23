using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NextGen.Tools;
using NextGen.VrManager.Devices.Serials;

public class TouchpadVisualizer : MonoBehaviour
{
    public Wristcuff wristcuff;

    public Transform raw;
    public Transform lastRaw;
    public Transform cursor;
    public List<VisualizerPixel> pixels;
    public TextMeshProUGUI text;

    public Color paintColor;

    public bool Touching { get { return wristcuff.Touching; } }
    public Vector2 RawPoint { get { return wristcuff.Point; } }


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 13; j++)
                pixels.Add(transform.GetChild(0).GetChild(i).GetChild(j).GetComponent<VisualizerPixel>());
        }

        /*
        GetPixelAt(1, 1).SetColor(Color.red);
        GetPixelAt(2, 2).SetColor(Color.red);
        GetPixelAt(4, 4).SetColor(Color.red);
        GetPixelAt(6, 8).SetColor(Color.red);
        */
    }

    // Update is called once per frame
    void Update()
    {
        raw.gameObject.SetActive(Touching);
        cursor.gameObject.SetActive(Touching);
        if (Touching && RawPoint != Vector2.zero)
        {
            raw.transform.localPosition = new Vector3(RawPoint.x, RawPoint.y, -0.25f);
            //lastRaw.transform.localPosition = new Vector3(LastRawPoint.x, LastRawPoint.y, -0.25f);
            //cursor.transform.localPosition = new Vector3(TweenPoint.x, TweenPoint.y, -1);
            GetPixelAt(RawPoint).SetColor(paintColor);
        }

        text.text = string.Format(
            "Touching: {0}\n" +
            "--Point: {1}\n",
            Touching, RawPoint);

    }

    private VisualizerPixel GetPixelAt(Vector2 _point)
    {
        VisualizerPixel pixel = null;

        pixel = pixels[(((int)_point.y - 1) * 13) + ((int)_point.x - 1)];

        return pixel;
    }

    private VisualizerPixel GetPixelAt(int x, int y)
    {
        return GetPixelAt(new Vector2(x, y));
    }
}
