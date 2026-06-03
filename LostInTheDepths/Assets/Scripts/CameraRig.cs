using Cinemachine;
using UnityEngine;

/// <summary>
/// Wraps a Cinemachine virtual camera that smoothly follows the player. Created
/// at runtime by <see cref="GameBootstrap"/>; call <see cref="Attach"/> to point
/// it at the player and configure the orthographic framing. The dead/soft zones
/// let the fish drift near screen centre without the camera reacting, then ease
/// it back once the fish moves further out, so the view never jitters.
/// </summary>
public class CameraRig : MonoBehaviour
{
    [Header("Target")]
    public Transform follow;

    [Header("Framing")]
    public float orthoSize = 6f;

    [Header("Soft Zone")]
    [Range(0f, 1f)] public float deadZoneW = 0.12f;
    [Range(0f, 1f)] public float deadZoneH = 0.12f;
    [Range(0f, 1f)] public float softZoneW = 0.55f;
    [Range(0f, 1f)] public float softZoneH = 0.55f;
    public float xDamping = 1.4f;
    public float yDamping = 1.4f;

    CinemachineVirtualCamera vcam;

    /// <summary>
    /// Ensures the main camera has a <see cref="CinemachineBrain"/>, then creates
    /// (or reuses) an orthographic virtual camera set to follow <paramref name="target"/>
    /// with the framing values configured on this component.
    /// </summary>
    public void Attach(Camera mainCamera, Transform target)
    {
        follow = target;

        if (mainCamera != null && mainCamera.GetComponent<CinemachineBrain>() == null)
            mainCamera.gameObject.AddComponent<CinemachineBrain>();

        vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam == null)
            vcam = gameObject.AddComponent<CinemachineVirtualCamera>();

        vcam.m_Lens.Orthographic = true;
        vcam.m_Lens.OrthographicSize = orthoSize;
        vcam.Follow = follow;

        var framer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framer == null)
            framer = vcam.AddCinemachineComponent<CinemachineFramingTransposer>();

        framer.m_DeadZoneWidth = deadZoneW;
        framer.m_DeadZoneHeight = deadZoneH;
        framer.m_SoftZoneWidth = softZoneW;
        framer.m_SoftZoneHeight = softZoneH;
        framer.m_XDamping = xDamping;
        framer.m_YDamping = yDamping;
        framer.m_LookaheadTime = 0f;
        framer.m_CameraDistance = 10f;
    }
}
