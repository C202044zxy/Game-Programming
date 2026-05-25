using Cinemachine;
using UnityEngine;

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
