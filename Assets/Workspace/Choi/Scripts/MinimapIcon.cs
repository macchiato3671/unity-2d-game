using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Transform target;
    public Rect roomBounds;
    public RectTransform minimapArea;
    private RectTransform iconTransform;

    void Start()
    {
        iconTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null) return;

        Vector2 normalized = new Vector2(
            Mathf.InverseLerp(roomBounds.xMin, roomBounds.xMax, target.position.x),
            Mathf.InverseLerp(roomBounds.yMin, roomBounds.yMax, target.position.y)
        );

        iconTransform.anchoredPosition = new Vector2(
            normalized.x * minimapArea.rect.width,
            normalized.y * minimapArea.rect.height
        );
    }
}
