using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ReticleBehaviour : MonoBehaviour
{
    [Header("Dependencias")]
    public DrivingSurfaceManager DrivingSurfaceManager;

    [SerializeField]
    private GameObject Child;

    public ARPlane CurrentPlane { get; private set; }

    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        // Comprobar cámara
        if (m_Camera == null)
        {
            m_Camera = Camera.main;

            if (m_Camera == null)
            {
                Debug.LogWarning("ReticleBehaviour: No se encontró la AR Camera.");
                return;
            }
        }

        // Comprobar DrivingSurfaceManager
        if (DrivingSurfaceManager == null)
        {
            Debug.LogError(
                "ReticleBehaviour: DrivingSurfaceManager no está asignado."
            );

            SetReticleVisible(false);
            return;
        }

        // Comprobar RaycastManager
        if (DrivingSurfaceManager.RaycastManager == null)
        {
            Debug.LogError(
                "ReticleBehaviour: RaycastManager no está disponible."
            );

            SetReticleVisible(false);
            return;
        }

        // Centro de la pantalla
        Vector2 screenCenter = m_Camera.ViewportToScreenPoint(
            new Vector3(0.5f, 0.5f)
        );

        var hits = new List<ARRaycastHit>();

        bool hasHit = DrivingSurfaceManager.RaycastManager.Raycast(
            screenCenter,
            hits,
            TrackableType.PlaneWithinBounds
        );

        CurrentPlane = null;

        if (!hasHit || hits.Count == 0)
        {
            SetReticleVisible(false);
            return;
        }

        // Buscar el impacto correcto
        ARRaycastHit selectedHit = default;
        bool foundHit = false;

        var lockedPlane = DrivingSurfaceManager.LockedPlane;

        if (lockedPlane == null)
        {
            selectedHit = hits[0];
            foundHit = true;
        }
        else
        {
            foreach (var h in hits)
            {
                if (h.trackableId == lockedPlane.trackableId)
                {
                    selectedHit = h;
                    foundHit = true;
                    break;
                }
            }
        }

        if (!foundHit)
        {
            SetReticleVisible(false);
            return;
        }

        // Buscar el plano asociado
        if (DrivingSurfaceManager.PlaneManager == null)
        {
            Debug.LogError(
                "ReticleBehaviour: PlaneManager no está disponible."
            );

            SetReticleVisible(false);
            return;
        }

        ARPlane plane = DrivingSurfaceManager.PlaneManager.GetPlane(
            selectedHit.trackableId
        );

        if (plane == null)
        {
            SetReticleVisible(false);
            return;
        }

        CurrentPlane = plane;

        // IMPORTANTE:
        // Usamos la pose solamente después de comprobar
        // que tenemos un plano válido.
        transform.SetPositionAndRotation(
            selectedHit.pose.position,
            selectedHit.pose.rotation
        );

        SetReticleVisible(true);
    }

    private void SetReticleVisible(bool visible)
    {
        if (Child != null)
            Child.SetActive(visible);
    }
}
