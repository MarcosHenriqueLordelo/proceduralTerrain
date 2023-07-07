using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGen : MonoBehaviour {

    public int grassDst = 200;
    public const int maxViewDst = 500;
    public int chunkResolution = 128;
    private int chunksVisibleDst = 1;
    public int chunksLoadedPerFrame = 3;

    private List<Vector2> visibleChunks = new List<Vector2>();
    public List<Vector2> grassChunks = new List<Vector2>();
    private List<TerrainChunk> loadedChunks = new List<TerrainChunk>();
    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    public Material[] terrainMaterial;

    public GameObject[] trees;
    public GameObject[] rocks;
    public GameObject[] grasses;

    public float seed = 2.0f;

    public GameObject water;
    public float waterLevel;

    public GameObject player;
    private Vector3 oldPosition;
    public Camera mainCamera;

    private void Start() {
        chunksVisibleDst = Mathf.RoundToInt(maxViewDst / chunkResolution);
        AddWater();

        UpdateChunksList();
    }

    private void Update() {
        if (Util.DistanceBtwPoints(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(oldPosition.x, oldPosition.z)) > 90.51) {
            UpdateChunksList();
            oldPosition = player.transform.position;
        }
    }

    private void UpdateVisibleChunks() {
        int chunkSize = (int)chunkResolution;

        int currentChunkX = Mathf.RoundToInt(player.transform.position.x / chunkSize);
        int currentChunkZ = Mathf.RoundToInt(player.transform.position.z / chunkSize);

        foreach (TerrainChunk chunk in loadedChunks) {
            chunk.SetVisible(false);
        }

        loadedChunks.Clear();
        grassChunks.Clear();

        for (int x = -chunksVisibleDst; x <= chunksVisibleDst; x++) {
            for (int z = -chunksVisibleDst; z <= chunksVisibleDst; z++) {
                Vector2Int chunkCoord = new Vector2Int(currentChunkX + x, currentChunkZ + z);

                if (terrainChunkDictionary.ContainsKey(chunkCoord)) {
                    terrainChunkDictionary[chunkCoord].UpdateVisibility(player.transform.position, (float)(chunksVisibleDst * 90.51 * 4));
                    if (terrainChunkDictionary[chunkCoord].IsVisible()) {
                        loadedChunks.Add(terrainChunkDictionary[chunkCoord]);
                        if (terrainChunkDictionary[chunkCoord].IsGrassVisible(player.transform.position, (float)(grassDst))) { 
                            grassChunks.Add(chunkCoord * chunkSize);
                        }
                    }

                } else {
                    terrainChunkDictionary.Add(chunkCoord, new TerrainChunk(chunkCoord, chunkSize, transform));
                    terrainChunkDictionary[chunkCoord].UpdateVisibility(player.transform.position, (float)(chunksVisibleDst * 90.51 * 4));
                    if (terrainChunkDictionary[chunkCoord].IsGrassVisible(player.transform.position, (float)(grassDst))) {
                        grassChunks.Add(chunkCoord * chunkSize);
                    }
                }
            }
        }
    }

    private void UpdateChunksList() {
        List<Vector2> currentVisible = new List<Vector2>();

        int chunkSize = (int)chunkResolution;
        int currentChunkX = Mathf.RoundToInt(player.transform.position.x / chunkSize);
        int currentChunkZ = Mathf.RoundToInt(player.transform.position.z / chunkSize);

        for (int x = -chunksVisibleDst; x <= chunksVisibleDst; x++) {
            for (int z = -chunksVisibleDst; z <= chunksVisibleDst; z++) {
                Vector2Int chunkCoord = new Vector2Int(currentChunkX + x, currentChunkZ + z);

                currentVisible.Add(chunkCoord);
            }
        }

        if (currentVisible != visibleChunks) {
            visibleChunks = currentVisible;
            UpdateVisibleChunks();
        }
    }

    private void AddWater() {
        if (water) {
            Vector3 waterPos = new Vector3(0, waterLevel, 0);
            GameObject current = Instantiate(water, waterPos, Quaternion.identity);
            current.transform.localScale = new Vector3(1000, 1, 1000);
        }
    }


    public class TerrainChunk {
        private GameObject terrainObj;
        private Vector2 position;
        private Bounds bounds;
        private int terrainSize;

        public TerrainChunk(Vector2 coord, int size, Transform parent) {
            position = coord * size;
            terrainSize = size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            terrainObj = new GameObject("Terrain" + new Vector2(position.x, position.y), typeof(TerrainGen), typeof(MeshRenderer), typeof(MeshCollider), typeof(MeshFilter));
            terrainObj.transform.parent = parent;
            terrainObj.transform.position = new Vector3(position.x, 0f, position.y);
            SetVisible(false);
        }

        public void UpdateVisibility(Vector3 playerPos, float dst) {
            int centerX = (int)position.x + (int)(terrainSize / 2);
            int centerY = (int)position.y + (int)(terrainSize / 2);

            float viewerDstFromCenter = (float)Mathf.Sqrt(Mathf.Pow(centerX - playerPos.x, 2) + Mathf.Pow(centerY - playerPos.z, 2));

            bool visible = viewerDstFromCenter <= dst;

            SetVisible(visible);
        }

        public bool IsGrassVisible(Vector3 playerPos, float dst) {
            int centerX = (int)position.x + (int)(terrainSize / 2);
            int centerY = (int)position.y + (int)(terrainSize / 2);

            float viewerDstFromCenter = (float)Mathf.Sqrt(Mathf.Pow(centerX - playerPos.x, 2) + Mathf.Pow(centerY - playerPos.z, 2));

            bool visible = viewerDstFromCenter <= dst;

            return visible;
        }

        public void SetVisible(bool visible) {
            terrainObj.SetActive(visible);
        }

        public bool IsVisible() {
            return terrainObj.activeSelf;
        }

        public Vector2 GetPosition() {
            return position;
        }
    }
}