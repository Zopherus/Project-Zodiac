using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RayCastButtonScript : MonoBehaviour, ICanvasRaycastFilter {

    Collider2D myCollider;
    RectTransform rectTransform;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
    }

    #region ICanvasRaycastFilter implementation
    public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        //Vector2 worldPoint = Vector2.zero;
        //bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, eventCamera, out worldPoint);

        var worldPoint = Vector3.zero;
        var isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            screenPos,
            eventCamera,
            out worldPoint
        );
        if (isInside)
            isInside = myCollider.OverlapPoint(worldPoint);
        return isInside;
    }
    #endregion
}
