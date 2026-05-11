using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays an objective/instruction message at the start of the game,
/// then fades it out after a configurable delay.
/// Attach to a UI GameObject with a TextMeshProUGUI child, or
/// assign objectiveText directly in the Inspector.
/// </summary>
public class ObjectiveText : MonoBehaviour
{
    [Tooltip("The text component to use for the objective message")]
    public TextMeshProUGUI objectiveText;

    [Tooltip("Message shown to the player at the start of a level")]
    [TextArea(2, 5)]
    public string objectiveMessage =
        "Destroy all enemies to win!\n" +
        "WASD to move  |  Mouse to aim  |  Left-click to fire\n" +
        "Collect power-ups for speed, fire rate, or health boosts.";

    [Tooltip("Seconds the message remains fully visible")]
    public float displayDuration = 3.5f;

    [Tooltip("Seconds the message takes to fade to transparent")]
    public float fadeDuration = 1.5f;

    private void Start()
    {
        if (objectiveText != null)
        {
            objectiveText.text = objectiveMessage;
            SetAlpha(1f);
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, elapsed / fadeDuration));
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void SetAlpha(float a)
    {
        Color c = objectiveText.color;
        c.a = a;
        objectiveText.color = c;
    }
}
