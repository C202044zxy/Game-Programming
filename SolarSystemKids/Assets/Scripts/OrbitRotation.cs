using UnityEngine;

/// Spins a body in place and (optionally) orbits another body.
public class OrbitRotation : MonoBehaviour
{
    [Header("Orbit")]
    public Transform orbitCenter;
    public float orbitSpeed = 20f;
    public Vector3 orbitAxis = Vector3.up;

    [Header("Self Rotation")]
    public float selfRotationSpeed = 50f;
    public Vector3 selfRotationAxis = Vector3.up;

    void Update()
    {
        if (orbitCenter != null)
            transform.RotateAround(orbitCenter.position, orbitAxis,
                orbitSpeed * Time.deltaTime);

        transform.Rotate(selfRotationAxis, selfRotationSpeed * Time.deltaTime, Space.Self);
    }
}
