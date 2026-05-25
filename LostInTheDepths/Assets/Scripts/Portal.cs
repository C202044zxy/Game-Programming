using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Portal : MonoBehaviour
{
    CircleCollider2D col;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.enabled   = false;
    }

    public void Activate() => col.enabled = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.LoadWinScene();
    }
}
