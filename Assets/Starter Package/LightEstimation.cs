using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Arrastra aquí el ARCameraManager del XR Origin")]
    ARCameraManager m_CameraManager;

    Light m_Light;

    void Awake()  => m_Light = GetComponent<Light>();

    void OnEnable()
    {
        // Suscribirse al evento de cada frame de la cámara AR
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += OnFrameReceived;
    }

    void OnDisable()
    {
        // Siempre desuscribirse para evitar memory leaks
        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= OnFrameReceived;
    }

    void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        var e = args.lightEstimation;

        // Brillo promedio del entorno (0 a 1)
        if (e.averageBrightness.HasValue)
            m_Light.intensity = e.averageBrightness.Value;

        // Temperatura de color en Kelvin (1000 cálido - 20000 frío)
        if (e.averageColorTemperature.HasValue)
            m_Light.colorTemperature = e.averageColorTemperature.Value;

        // Corrección de color (tinte ambiental del entorno)
        if (e.colorCorrection.HasValue)
            m_Light.color = e.colorCorrection.Value;

        // Dirección de la fuente de luz principal del mundo real
        if (e.mainLightDirection.HasValue)
            m_Light.transform.rotation =
                Quaternion.LookRotation(e.mainLightDirection.Value);

        // Color de la luz principal detectada
        if (e.mainLightColor.HasValue)
            m_Light.color = e.mainLightColor.Value;

        // Intensidad en lúmenes de la luz principal
        if (e.mainLightIntensityLumens.HasValue)
            m_Light.intensity = e.averageMainLightBrightness
                                 ?? m_Light.intensity;

        // Iluminación ambiental por armónicos esféricos (skybox)
        if (e.ambientSphericalHarmonics.HasValue)
        {
            RenderSettings.ambientMode  = AmbientMode.Skybox;
            RenderSettings.ambientProbe = e.ambientSphericalHarmonics.Value;
        }
    }
}