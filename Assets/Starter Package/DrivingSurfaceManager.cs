using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// Gestiona la superficie de conducción AR.
/// Expone PlaneManager y RaycastManager a otros scripts.
public class DrivingSurfaceManager : MonoBehaviour
{
    // Plano bloqueado por el jugador (null = aún eligiendo)
    public ARPlane LockedPlane { get; private set; }

    public ARPlaneManager   PlaneManager   { get; private set; }
    public ARRaycastManager RaycastManager { get; private set; }

    void Awake()
    {
        PlaneManager   = GetComponent<ARPlaneManager>();
        RaycastManager = GetComponent<ARRaycastManager>();
    }

    /// Bloquea el plano elegido y oculta todos los demás.
    /// Desactiva ARPlaneManager para no detectar más planos.
    public void LockPlane(ARPlane plane)
    {
        LockedPlane = plane;

        foreach (var p in PlaneManager.trackables)
            p.gameObject.SetActive(p == plane);

        // Dejar de detectar nuevos planos una vez fijo el juego
        PlaneManager.enabled = false;
    }
}