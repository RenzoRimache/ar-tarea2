using UnityEngine;
using UnityEngine.InputSystem;

public class CarManager : MonoBehaviour
{
    [Header("Dependencias")]
    public GameObject CarPrefab;
    public ReticleBehaviour Reticle;
    public DrivingSurfaceManager DrivingSurfaceManager;

    private GameObject m_SpawnedCar;

    private void Update()
    {
        bool tapped = false;

        // Pantalla táctil - Android / iPhone
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            tapped = true;
        }

        // Mouse - Unity Editor
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            tapped = true;
        }

        if (tapped)
        {
            TrySpawnCar();
        }

        // Si el auto existe, hacerlo seguir al retículo
        if (m_SpawnedCar != null &&
            Reticle != null &&
            Reticle.CurrentPlane != null)
        {
            CarBehaviour car = m_SpawnedCar.GetComponent<CarBehaviour>();

            if (car != null)
            {
                car.SetTargetPosition(Reticle.transform.position);
            }
        }
    }

    private void TrySpawnCar()
    {
        // Comprobar que tenemos retículo
        if (Reticle == null)
        {
            Debug.LogError("CarManager: Falta asignar Reticle.");
            return;
        }

        // Comprobar que existe un plano detectado
        if (Reticle.CurrentPlane == null)
        {
            Debug.LogWarning("CarManager: No hay ningún plano AR detectado.");
            return;
        }

        // Comprobar que existe el prefab
        if (CarPrefab == null)
        {
            Debug.LogError("CarManager: Falta asignar CarPrefab.");
            return;
        }

        // Bloquear el plano
        if (DrivingSurfaceManager != null &&
            DrivingSurfaceManager.LockedPlane == null)
        {
            DrivingSurfaceManager.LockPlane(Reticle.CurrentPlane);
        }

        // Crear el vehículo
        if (m_SpawnedCar == null)
        {
            m_SpawnedCar = Instantiate(
                CarPrefab,
                Reticle.transform.position,
                Reticle.transform.rotation
            );

            Debug.Log("Vehículo creado correctamente.");
        }
    }
}
