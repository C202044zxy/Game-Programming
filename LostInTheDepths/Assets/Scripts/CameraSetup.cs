using Cinemachine;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform playerTransform;

    [Range(0f, 1f)] [SerializeField] float deadZoneWidth  = 0.1f;
    [Range(0f, 1f)] [SerializeField] float deadZoneHeight = 0.1f;
    [Range(0f, 1f)] [SerializeField] float softZoneWidth  = 0.6f;
    [Range(0f, 1f)] [SerializeField] float softZoneHeight = 0.6f;

    void Start()
    {
        if (virtualCamera == null) return;

        if (playerTransform == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) playerTransform = go.transform;
        }

        if (playerTransform == null) return;

        virtualCamera.Follow = playerTransform;

        var ft = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (ft == null) return;
        ft.m_DeadZoneWidth  = deadZoneWidth;
        ft.m_DeadZoneHeight = deadZoneHeight;
        ft.m_SoftZoneWidth  = softZoneWidth;
        ft.m_SoftZoneHeight = softZoneHeight;
        ft.m_LookaheadTime  = 0f;
        ft.m_XDamping       = 1.5f;
        ft.m_YDamping       = 1.5f;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (virtualCamera == null) return;
        var ft = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (ft == null) return;
        ft.m_DeadZoneWidth  = deadZoneWidth;
        ft.m_DeadZoneHeight = deadZoneHeight;
        ft.m_SoftZoneWidth  = softZoneWidth;
        ft.m_SoftZoneHeight = softZoneHeight;
    }
#endif
}
