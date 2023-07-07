using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderWhenInView : MonoBehaviour {
    private MeshRenderer meshRenderer;
    private Camera mainCamera;
    private ChunkGen chunkGen;

    private void Start() {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGen>();
        meshRenderer = GetComponent<MeshRenderer>();
        mainCamera = chunkGen.mainCamera;
    }

    private void Update() {
        // Get the bounds of the mesh in world space
        Bounds bounds = meshRenderer.bounds;

        // Calculate the planes that define the camera's frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Check if the bounds of the mesh are within the camera's frustum
        bool isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, bounds);

        // Set the mesh renderer's enabled property based on whether the mesh is visible
        //meshRenderer.enabled = isVisible;
    }
}
