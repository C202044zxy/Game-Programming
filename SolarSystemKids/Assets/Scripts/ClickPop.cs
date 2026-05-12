using UnityEngine;

/// Brief scale-pulse visual response on click.
/// Sits beside ClickableCelestial; both receive OnMouseDown.
public class ClickPop : MonoBehaviour
{
    public float popScale = 1.25f;
    public float popDuration = 0.35f;

    Vector3 baseScale;
    float popTimer = -1f;

    void Start() { baseScale = transform.localScale; }

    void OnMouseDown() { popTimer = 0f; }

    void Update()
    {
        if (popTimer < 0f) return;
        popTimer += Time.deltaTime;
        float t = Mathf.Clamp01(popTimer / popDuration);
        float curve = Mathf.Sin(t * Mathf.PI);                // bell curve
        transform.localScale = baseScale * Mathf.Lerp(1f, popScale, curve);
        if (t >= 1f) { transform.localScale = baseScale; popTimer = -1f; }
    }
}
