using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Diagnostics;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Xml.Linq;

public class TerrainGen : MonoBehaviour {
    private ChunkGen chunkGen;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Vector2[] uv;

    private Renderer objectRenderer;

    private Texture2D grassMap2D;
    private bool isGrassVisible;

    private Vector2 coord;

    private void Start() {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGen>();
        objectRenderer = GetComponent<Renderer>();

        if (chunkGen == null)
            return;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.materials = new Material[] { chunkGen.terrainMaterial[1] };

        coord = new Vector2(transform.position.x, transform.position.z);

        StartCoroutine(CalculateTerrain(OnReceiveTerrainData));
    }

    private void Update() {
        if (chunkGen != null && grassMap2D != null) {

            if (isGrassVisible != chunkGen.grassChunks.Contains(coord))
                isGrassVisible = chunkGen.grassChunks.Contains(coord);

            if (isGrassVisible) {
                meshRenderer.materials = chunkGen.terrainMaterial;
                GetComponent<Renderer>().material.SetTexture("_GrassMap", grassMap2D);
            } else {
                meshRenderer.materials = new Material[] { chunkGen.terrainMaterial[1] };
            }
        }
    }


    private void OnReceiveTerrainData(TerrainData terrainData) {
        mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = terrainData.GetVertices();
        mesh.triangles = terrainData.GetTriangles();
        mesh.uv = terrainData.GetUv();
        mesh.RecalculateBounds();
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        StartCoroutine(SpawnObjects(() => { GetGrassMap(); }));
    }

    private IEnumerator SpawnObjects(Action callBack) {
        for (int i = 0, x = 0; x <= chunkGen.chunkResolution; x++) {
            for (int z = 0; z <= chunkGen.chunkResolution; z++) {
                bool hasObject = false;

                float y = Noise(x, z);

                hasObject = SpawnTree(x, y, z);

                if (!hasObject)
                    hasObject = SpawnRock(x, y, z);

                if (i == 64) {
                    i = 0;
                    yield return new WaitForSeconds(5);
                }

                if (hasObject) i++;
            }
        }

        callBack();
    }



    private void GetGrassMap() {
        grassMap2D = new Texture2D(chunkGen.chunkResolution, chunkGen.chunkResolution);

        for (int x = 0; x <= chunkGen.chunkResolution; x++) {
            for (int z = 0; z <= chunkGen.chunkResolution; z++) {
                float y = Noise(x, z);

                float doesSpawn = Noises.Grass((x + transform.position.x), (z + transform.position.z));
                float density = Noises.GrassDensity((x + transform.position.x), (z + transform.position.z));

                if (doesSpawn > 0 && density > 0.35f && y > chunkGen.waterLevel) {
                    Color color = new Color(1f, 1f, 1f);
                    grassMap2D.SetPixel(x, z, color);
                } else {
                    Color color = new Color(0f, 0f, 0f);
                    grassMap2D.SetPixel(x, z, color);
                }

            }
        }

        grassMap2D.Apply();
    }

    private IEnumerator CalculateTerrain(Action<TerrainData> callBack) {
        Vector3[] vertices = new Vector3[(int)((chunkGen.chunkResolution + 1) * (chunkGen.chunkResolution + 1))];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution; x++) {
            for (int z = 0; z <= chunkGen.chunkResolution; z++) {
                float y = Noise(x, z);

                vertices[i] = new Vector3(
                    x * (int)(128 / chunkGen.chunkResolution), y,
                    z * (int)(128 / chunkGen.chunkResolution));

                i++;
            }
        }

        for (int i = 0; i < uv.Length; i++) {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        triangles = new int[(int)chunkGen.chunkResolution * (int)chunkGen.chunkResolution * 6];

        int tris = 0;
        int vert = 0;

        for (int x = 0; x < chunkGen.chunkResolution; x++) {
            for (int z = 0; z < chunkGen.chunkResolution; z++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = (int)(vert + chunkGen.chunkResolution + 1);
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = (int)(vert + chunkGen.chunkResolution + 1);
                triangles[tris + 5] = (int)(vert + chunkGen.chunkResolution + 2);

                vert++;
                tris += 6;
            }

            vert++;
        }

        TerrainData terrainData = new TerrainData(vertices, uv, triangles);

        yield return new WaitForEndOfFrame();

        callBack(terrainData);
    }

    private float Noise(float x, float z) {
        float xVal = (((x + chunkGen.seed) * (128 / chunkGen.chunkResolution)) + transform.position.x);
        float zVal = (((z + chunkGen.seed) * (128 / chunkGen.chunkResolution)) + transform.position.z);


        float y = Noises.Continentalness(xVal, zVal) * 200;
        y *= Noises.Erosion(xVal, zVal);
        y += Noises.PeaksValleys(xVal, zVal) * 200;


        return y;
    }

    private bool SpawnTree(float x, float y, float z) {
        float doesSpawn = Noises.Forest((x + transform.position.x), (z + transform.position.z));

        float density = Noises.TreeDensity((x + transform.position.x), (z + transform.position.z));

        if (doesSpawn > 0 && (y + transform.position.y) > chunkGen.waterLevel && density > 0.85f) {

            int whatSpawns = Noises.Variation(x, z, chunkGen.trees.Length -1);

            Debug.Log(whatSpawns);

            Vector3 spawnPos = new Vector3(x * (128 / chunkGen.chunkResolution) + transform.position.x, y + transform.position.y, z * (128 / chunkGen.chunkResolution) + transform.position.z);

            float scale = Noises.Scale(x, y, 3, 2);

            Vector3 boxSize = new Vector3(3, 5, 3) * scale;
            Collider[] colliders = Physics.OverlapBox(spawnPos, boxSize / 2);

            foreach (Collider collider in colliders) {
                if (collider.gameObject.tag == "Tree") {
                    return false;
                }
            }

            GameObject currentTree = Instantiate(chunkGen.trees[whatSpawns], spawnPos, Quaternion.identity);

            float rotation = Noises.Rotation(x, z);
            currentTree.transform.rotation = Quaternion.Euler(0, rotation, 0);

            Vector3 currentScale = currentTree.transform.localScale;
            currentScale *= scale;
            currentTree.transform.localScale = currentScale;

            currentTree.transform.parent = transform;

            return true;
        }

        return false;
    }

    private bool SpawnRock(float x, float y, float z) {
        float doesSpawn = Noises.Rock((x + transform.position.x), (z + transform.position.z));

        float density = Noises.RockDensity((x + transform.position.x), (z + transform.position.z));

        if (doesSpawn > 0 && (y + transform.position.y) > chunkGen.waterLevel && density > 0.98f) {

            int whatSpawns = Noises.Variation(x, z, chunkGen.rocks.Length - 1);

            GameObject currentRock = Instantiate(chunkGen.rocks[whatSpawns], new Vector3(
                x * (128 / chunkGen.chunkResolution) + transform.position.x,
                y + transform.position.y,
                z * (128 / chunkGen.chunkResolution) + transform.position.z), Quaternion.identity);


            float rotation = Noises.Rotation(x, z);
            currentRock.transform.rotation = Quaternion.Euler(rotation, rotation, rotation);


            float scale = Noises.Scale(x, y, 10, 1);
            Vector3 currentScale = currentRock.transform.localScale;
            currentScale *= scale;
            currentRock.transform.localScale = currentScale;

            currentRock.transform.parent = transform;

            return true;
        }

        return false;
    }

    private bool SpawnGrass(float x, float y, float z) {
        float doesSpawn = Noises.Grass((x + transform.position.x), (z + transform.position.z));
        float density = Noises.GrassDensity((x + transform.position.x), (z + transform.position.z));

        if (doesSpawn > 0 && (y + transform.position.y) > chunkGen.waterLevel && density > 0.45f) {
            int whatSpawns = (int)UnityEngine.Random.Range(0, chunkGen.grasses.Length);

            Vector3 spawnPos = new Vector3(x * (128 / chunkGen.chunkResolution) + transform.position.x, y + transform.position.y, z * (128 / chunkGen.chunkResolution) + transform.position.z);

            Vector3 boxSize = new Vector3(0.35f, 0.28f, 0.41f);
            Collider[] colliders = Physics.OverlapBox(spawnPos, boxSize / 2);

            foreach (Collider collider in colliders) {
                if (collider.gameObject.tag == "Tree" || collider.gameObject.tag == "Rock") {
                    return false;
                }
            }

            return true;
        }

        return false;
    }
}


public class TerrainData {
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    public TerrainData(Vector3[] vertices, Vector2[] uv, int[] triangles) {
        this.vertices = vertices;
        this.uv = uv;
        this.triangles = triangles;
    }

    public Vector3[] GetVertices() { return vertices; }
    public Vector2[] GetUv() { return uv; }
    public int[] GetTriangles() { return triangles; }
}