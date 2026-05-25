using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pearl : MonoBehaviour
{
    void Awake() => GetComponent<CircleCollider2D>().isTrigger = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.OnPearlCollected();
        gameObject.SetActive(false);
    }
}
