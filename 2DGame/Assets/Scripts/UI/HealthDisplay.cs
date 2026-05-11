using UnityEngine;
using TMPro;

/// <summary>
/// Displays the player's current health on the HUD.
/// Attach to a UI GameObject alongside a TextMeshProUGUI component,
/// then assign the displayText field in the Inspector.
/// </summary>
public class HealthDisplay : UIelement
{
    [Tooltip("TextMeshPro text component used to show the health value")]
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
        if (playerHealth != null)
            displayText.text = "HP: " + playerHealth.currentHealth + " / " + playerHealth.maximumHealth;
    }
}
