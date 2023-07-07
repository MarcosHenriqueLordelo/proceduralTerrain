using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTexture : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float offsetX = 100f;
    public float offsetY = 100f;

    private Texture2D noiseTexture;

    void Start() {
        GenerateTexture();
    }

    void GenerateTexture() {
        noiseTexture = new Texture2D(width, height);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = new Color(sample, sample, sample);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();

        // Set the texture to a material's main texture
        GetComponent<Renderer>().material.SetTexture("_GrassMap", noiseTexture);
    }
}
