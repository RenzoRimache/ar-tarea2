using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PackageSpawner : MonoBehaviour
{
    [Header("Dependencias")]
    public DrivingSurfaceManager DrivingSurfaceManager;
    public PackageBehaviour Package;
    public GameObject PackagePrefab;

    public static Vector3 RandomInTriangle(Vector3 v1, Vector3 v2)
    {
        float u = Random.Range(0.0f, 1.0f);
        float v = Random.Range(0.0f, 1.0f);

        if (v + u > 1)
        {
            v = 1 - v;
            u = 1 - u;
        }

        return (v1 * u) + (v2 * v);
    }

    public static Vector3 FindRandomLocation(ARPlane plane)
    {
        var meshVisualizer =
            plane.GetComponent<ARPlaneMeshVisualizer>();

        if (meshVisualizer == null)
        {
            Debug.LogError(
                "PackageSpawner: El plano no tiene ARPlaneMeshVisualizer."
            );

            return plane.transform.position;
        }

        var mesh = meshVisualizer.mesh;

        if (mesh == null || mesh.triangles.Length < 3)
        {
            Debug.LogWarning(
                "PackageSpawner: El mesh del plano todavía no tiene triángulos."
            );

            return plane.transform.position;
        }

        var triangles = mesh.triangles;

        int triangleIndex =
            Random.Range(0, triangles.Length / 3) * 3;

        var vertices = mesh.vertices;

        int v1 = triangles[triangleIndex];
        int v2 = triangles[triangleIndex + 1];

        var randomInTriangle =
            RandomInTriangle(vertices[v1], vertices[v2]);

        return plane.transform.TransformPoint(randomInTriangle);
    }

    public void SpawnPackage(ARPlane plane)
    {
        if (PackagePrefab == null)
        {
            Debug.LogError(
                "PackageSpawner: PackagePrefab no está asignado."
            );

            return;
        }

        if (plane == null)
        {
            Debug.LogError(
                "PackageSpawner: El plano es NULL."
            );

            return;
        }

        Vector3 spawnPosition = FindRandomLocation(plane);

        var packageClone = Instantiate(
            PackagePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Package = packageClone.GetComponent<PackageBehaviour>();

        if (Package == null)
        {
            Debug.LogError(
                "PackageSpawner: El PackagePrefab no tiene PackageBehaviour."
            );

            Destroy(packageClone);
            return;
        }

        Debug.Log(
            "PAQUETE CREADO en: " + spawnPosition
        );
    }

    private void Update()
    {
        if (DrivingSurfaceManager == null)
        {
            Debug.LogError(
                "PackageSpawner: DrivingSurfaceManager no está asignado."
            );

            enabled = false;
            return;
        }

        var lockedPlane = DrivingSurfaceManager.LockedPlane;

        if (lockedPlane == null)
            return;

        if (Package == null)
        {
            SpawnPackage(lockedPlane);
        }

        if (Package != null)
        {
            Vector3 packagePosition = Package.transform.position;

            packagePosition.y = lockedPlane.transform.position.y;

            Package.transform.position = packagePosition;
        }
    }
}
