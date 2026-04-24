using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class ClickableCelestial : MonoBehaviour
{
    [Header("Identity")]
    public string objectName = "Planet";
    [TextArea(2, 4)]
    public string funFact = "I am a fun planet!";

    [Header("Visual Feedback")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;

    [Header("Audio")]
    public AudioClip clickSound;

    [Header("References (auto-found if left empty)")]
    public CameraFocusController cameraController;
    public Text factText;
    public Text titleText;
    public GameObject infoPanel;

    private Renderer rend;
    private Color originalEmission;
    private bool hasEmission;
    private AudioSource audioSource;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_EmissionColor"))
        {
            hasEmission = true;
            originalEmission = rend.material.GetColor("_EmissionColor");
            rend.material.EnableKeyword("_EMISSION");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (cameraController == null)
            cameraController = Object.FindFirstObjectByType<CameraFocusController>();
    }

    void OnMouseDown()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound);

        SetHighlight(true);

        if (cameraController != null)
            cameraController.FocusOn(transform, this);
    }

    public void ShowInfo()
    {
        if (infoPanel != null) infoPanel.SetActive(true);
        if (titleText != null) titleText.text = objectName;
        if (factText != null) factText.text = funFact;
    }

    public void HideInfo()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
        SetHighlight(false);
    }

    public void SetHighlight(bool on)
    {
        if (!hasEmission || rend == null) return;
        rend.material.SetColor("_EmissionColor",
            on ? highlightColor * highlightIntensity : originalEmission);
    }
}
