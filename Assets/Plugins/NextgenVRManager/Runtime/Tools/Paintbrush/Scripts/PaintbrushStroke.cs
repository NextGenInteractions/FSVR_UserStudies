using UnityEngine;

namespace NextGen.Tools
{
    public class PaintbrushStroke : MonoBehaviour
    {
        LineRenderer lineRend;

        public void Init(Vector3 _startPos, Material _material, Color _paintColor, float width)
        {
            lineRend = gameObject.AddComponent<LineRenderer>();
            lineRend.startColor = Color.white;
            lineRend.endColor = Color.white;
            lineRend.material = _material;
            lineRend.material.color = _paintColor;
            lineRend.startWidth = width;
            lineRend.endWidth = width;
            lineRend.positionCount = 1;
            lineRend.SetPosition(0, _startPos);
        }

        public void AddPos(Vector3 _pos)
        {
            lineRend.positionCount++;
            lineRend.SetPosition(lineRend.positionCount - 1, _pos);
        }
    }
}
