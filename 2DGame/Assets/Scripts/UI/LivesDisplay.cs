using UnityEngine;
using TMPro;

/// <summary>
/// Displays the player's remaining lives on the HUD.
/// Only shows when the player's Health component has useLives = true.
/// Attach to a UI GameObject, then assign displayText in the Inspector.
/// </summary>
public class LivesDisplay : UIelement
{
    [Tooltip("TextMeshPro text component used to show the lives count")]
    public TextMeshProUGUI displayText;

    private Health playerHealth;

    private void Start()
    {
        FindPlayerHealth();
    }

    private void FindPlayerHealth()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
            playerHealth = GameManager.instance.player.GetComponent<Health>();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        if (displayText == null) return;
        if (playerHealth == null) FindPlayerHealth();
        if (playerHealth == null) return;

        if (playerHealth.useLives)
            displayText.text = "Lives: " + playerHealth.currentLives;
        else
            displayText.text = "";
    }
}
