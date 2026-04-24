using UnityEngine;
using UnityEngine.UI;

public class CameraFocusController : MonoBehaviour
{
    [Header("Smoothing")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 4f;

    [Header("Focus Settings")]
    public float focusDistance = 4f;
    public float focusHeight = 1f;

    [Header("UI")]
    public Button backButton;

    private Vector3 homePosition;
    private Quaternion homeRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Transform focusTarget;
    private ClickableCelestial currentCelestial;
    private bool isFocused;

    void Start()
    {
        homePosition = transform.position;
        homeRotation = transform.rotation;
        targetPosition = homePosition;
        targetRotation = homeRotation;

        if (backButton != null)
        {
            backButton.onClick.AddListener(ReturnHome);
            backButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition,
            Time.deltaTime * moveSpeed);

        if (focusTarget != null && isFocused)
        {
            Quaternion lookRot = Quaternion.LookRotation(focusTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot,
                Time.deltaTime * rotateSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                Time.deltaTime * rotateSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isFocused)
            ReturnHome();
    }

    public void FocusOn(Transform target, ClickableCelestial celestial)
    {
        if (currentCelestial != null && currentCelestial != celestial)
            currentCelestial.HideInfo();

        focusTarget = target;
        currentCelestial = celestial;
        isFocused = true;

        Vector3 dir = (transform.position - target.position).normalized;
        if (dir == Vector3.zero) dir = -transform.forward;
        targetPosition = target.position + dir * focusDistance + Vector3.up * focusHeight;

        celestial.ShowInfo();

        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    public void ReturnHome()
    {
        if (currentCelestial != null) currentCelestial.HideInfo();
        currentCelestial = null;
        focusTarget = null;
        isFocused = false;

        targetPosition = homePosition;
        targetRotation = homeRotation;

        if (backButton != null) backButton.gameObject.SetActive(false);
    }
}
