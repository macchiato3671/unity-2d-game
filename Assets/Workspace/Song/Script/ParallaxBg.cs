using UnityEngine;

public class LoopingParallax : MonoBehaviour
{
    public Transform cam;
    public float parallaxFactor;

    float spriteWidth;
    Vector3 startPosition;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        startPosition = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        float distanceMoved = cam.position.x * parallaxFactor;
        float newX = startPosition.x + distanceMoved;

        transform.position = new Vector3(newX, startPosition.y, startPosition.z);

        float offsetX = cam.position.x * (1 - parallaxFactor);
        int loopCount = Mathf.FloorToInt(offsetX / spriteWidth);
        transform.position += new Vector3(loopCount * spriteWidth, 0f, 0f);
    }
}