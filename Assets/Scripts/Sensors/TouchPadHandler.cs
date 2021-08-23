using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchPadHandler : MonoBehaviour
{
    public static float Rows = 9f, Cols = 13f;
    private Vector2 _coordinate = new Vector2();
    public List<Quadrant> quadrants = new List<Quadrant>();
    public bool isTouching = false;
    public UnityEvent untouchEvent, touchEvent;
    private bool _isTouching = false;
    public Vector2 GetCoordinate() => _coordinate;

    //TouchpadInterpreter reference. --Connor
    public TouchpadInterpreter interpreter;

    // Start is called before the first frame update

    private void Start()
    {
        GetComponent<ArduinoInputHandler>().onUntouch += OnUntouchEvent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        quadrants.ForEach(a => a.UpdateQuadrant(_coordinate, isTouching));

        //interpreter.Ping(_isTouching,_coordinate);
    }

    void OnUntouchEvent()
    {
        if (_isTouching != isTouching)
        {
            _isTouching = false;
            untouchEvent.Invoke();
            quadrants.ForEach(a => a.isQuadrantOn = false);
        }
        isTouching = false;
    }

    public void UpdateCoord(float? x, float? y)
    {
        if (_isTouching != isTouching)
        {
            _isTouching = true;
            touchEvent.Invoke();
        }
        isTouching = true;
        if (x != null)
            _coordinate.x = (float)x;
        if (y != null)
            _coordinate.y = (float)y;
    }
    /*
    public Vector2 GetQuadrant(int quadrantIndex)
    {
        if (quadrantIndex < quadrants.Count)
            return quadrants[quadrantIndex].GetQuadrant(_coordinate);
        else
            return -Vector2.one;
    }
    */

    [Serializable]
    public class Quadrant
    {
        public Vector2 RowBounds = new Vector2(1, 9);
        public Vector2 ColBounds = new Vector2(1, 13);
        public UnityEvent OnQuadrantEnter;
        public UnityEvent OnQuadrantExit;
        public bool isQuadrantOn;

        public void UpdateQuadrant(Vector2 input, bool isTouching)
        {
            bool result = input.x >= ColBounds.x && input.x <= ColBounds.y && input.y >= RowBounds.x && input.y <= RowBounds.y && isTouching;
            if (result == isQuadrantOn)
                return;
            if (result)
                OnQuadrantEnter.Invoke();
            else
                OnQuadrantExit.Invoke();
            isQuadrantOn = result;
        }

        #region Legacy Code (Commented out)
        /*
        public int RowNum = 1;
        public int ColNum = 1;

        public Vector2 GetQuadrant(Vector2 coord)
        {
            Vector2 result = new Vector2();
            float aCol = Cols / ColNum;
            float aRow = Rows / RowNum;
            int counter = ColNum;
            for (float i = Cols; i >= 0; i -= aCol, --counter)
            {
                if (i < coord.x)
                {
                    result.x = counter;
                    break;
                }
            }
            counter = RowNum;
            for (float i = Rows; i >= 0; i -= aRow, --counter)
            {
                if (i < coord.y)
                {
                    result.y = counter;
                    break;
                }
            }
            return result;
        }
        */
        #endregion
    }
}
