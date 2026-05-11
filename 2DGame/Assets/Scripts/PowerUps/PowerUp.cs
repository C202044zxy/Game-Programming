using System.Collections;
using UnityEngine;

/// <summary>
/// A collectible power-up that the player picks up on trigger contact.
/// Three types are supported:
///   SpeedBoost    - multiplies moveSpeed on the Controller for a duration
///   FireRateBoost - divides fireRate on all ShootingControllers (lower = faster)
///   HealthRestore - instantly heals the player by a flat amount
///
/// Tag the player GameObject as "Player" for this to work.
/// </summary>
public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, FireRateBoost, HealthRestore }

    [Tooltip("What kind of power-up this is")]
    public PowerUpType powerUpType = PowerUpType.SpeedBoost;

    [Tooltip("How long the timed effects last (ignored for HealthRestore)")]
    public float duration = 5f;

    [Tooltip("Effect magnitude:\n" +
             "SpeedBoost / FireRateBoost: multiplier (e.g. 1.5 = 50% boost)\n" +
             "HealthRestore: flat HP restored")]
    public float magnitude = 1.5f;

    [Tooltip("Optional particle/visual prefab instantiated at the pickup position")]
    public GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyEffect(other.gameObject);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void ApplyEffect(GameObject player)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                Controller ctrl = player.GetComponent<Controller>();
                if (ctrl != null)
                    ctrl.StartCoroutine(BoostSpeed(ctrl));
                break;

            case PowerUpType.FireRateBoost:
                ShootingController[] guns = player.GetComponentsInChildren<ShootingController>();
                if (guns.Length > 0)
                    guns[0].StartCoroutine(BoostFireRate(guns));
                break;

            case PowerUpType.HealthRestore:
                Health health = player.GetComponent<Health>();
                if (health != null)
                    health.ReceiveHealing((int)magnitude);
                break;
        }
    }

    private IEnumerator BoostSpeed(Controller ctrl)
    {
        float original = ctrl.moveSpeed;
        ctrl.moveSpeed *= magnitude;
        yield return new WaitForSeconds(duration);
        if (ctrl != null)
            ctrl.moveSpeed = original;
    }

    private IEnumerator BoostFireRate(ShootingController[] guns)
    {
        float[] originals = new float[guns.Length];
        for (int i = 0; i < guns.Length; i++)
        {
            originals[i] = guns[i].fireRate;
            guns[i].fireRate /= magnitude;
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < guns.Length; i++)
            if (guns[i] != null)
                guns[i].fireRate = originals[i];
    }
}
