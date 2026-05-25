using Cinemachine;
using UnityEngine;

/// <summary>
/// Bootstraps the Cinemachine virtual camera at runtime to follow the player.
/// Attach to any persistent GameObject in the scene (e.g. GameManager).
/// </summary>
public class CameraSetup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform playerTransform;

    [Header("Soft Zone (degrees of freedom before camera moves)")]
    [Range(0f, 1f)] [SerializeField] float deadZoneWidth = 0.1f;
    [Range(0f, 1f)] [SerializeField] float deadZoneHeight = 0.1f;
    [Range(0f, 1f)] [SerializeField] float softZoneWidth = 0.6f;
    [Range(0f, 1f)] [SerializeField] float softZoneHeight = 0.6f;

    void Start()
    {
        if (virtualCamera == null || playerTransform == null) return;

        virtualCamera.Follow = playerTransform;

        var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framingTransposer != null)
        {
            framingTransposer.m_DeadZoneWidth = deadZoneWidth;
            framingTransposer.m_DeadZoneHeight = deadZoneHeight;
            framingTransposer.m_SoftZoneWidth = softZoneWidth;
            framingTransposer.m_SoftZoneHeight = softZoneHeight;
            // Gentle lag so the camera trails slightly behind fast movement
            framingTransposer.m_LookaheadTime = 0f;
            framingTransposer.m_XDamping = 1.5f;
            framingTransposer.m_YDamping = 1.5f;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (virtualCamera == null) return;
        var ft = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (ft == null) return;
        ft.m_DeadZoneWidth = deadZoneWidth;
        ft.m_DeadZoneHeight = deadZoneHeight;
        ft.m_SoftZoneWidth = softZoneWidth;
        ft.m_SoftZoneHeight = softZoneHeight;
    }
#endif
}
