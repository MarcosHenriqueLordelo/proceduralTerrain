using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noises {
    public static float Grass(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
            new Vector2(0, 0f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1, 1)
        };

        float noise = Util.Perlin(x + 1, z + 7, 0.005f, 1, 1f, 1.0f);

        noise = Util.ApplySpline(noise, splinePoints);

        return noise;
    }

    public static float GrassDensity(float x, float z) {
        float noise = Util.Perlin(x + 2, z + 6, 0.098f, 2, 1f, 1.0f);

        return noise;
    }

    //Rock and Trees Noises

    public static float Forest(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
            new Vector2(0, 0f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1, 1)
        };

        float noise = Util.Perlin(x, z, 0.0005f, 1, 1f, 1.0f);

        noise = Util.ApplySpline(noise, splinePoints);

        return noise;
    }

    public static float TreeDensity(float x, float z) {
        float noise = Util.Perlin(x, z, 0.95f, 1, 1f, 1.0f);

        return noise;
    }

    public static float Rock(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
            new Vector2(0, 0f),
            new Vector2(0.5f, 0.3f),
            new Vector2(1, 1)
        };

        float noise = Util.Perlin(x, z, 0.015f, 1, 1f, 1.0f);

        noise = Util.ApplySpline(noise, splinePoints);

        return noise;
    }

    public static float RockDensity(float x, float z) {
        float noise = Util.Perlin(x, z, 0.7f, 1, 1f, 1.0f);

        return noise;
    }

    public static float Rotation(float x, float z) {
        float y = Util.CalculateY(x, z, 360);

        return y;
    }

    public static float Scale(float x, float z, int maxScale, int minScale) {
        float y = Util.CalculateY(x, z, maxScale, minScale);

        return y;
    }

    public static int Variation(float x, float z, int lenght) {
        int y = (int)Util.CalculateY(x, z, lenght);

        if (y > lenght) return lenght;

        return y;
    }

    //Terrain height noises

    public static float Mountains(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0.6f, 0.6f),
            new Vector2(1, 1)
        };

        float y = Util.Perlin(x, z, 0.00005f, 1, 0.1f, 1f);

        y = Util.ApplySpline(y, splinePoints);

        return y;
    }

    public static float Continentalness(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
            new Vector2(0, -1f),
            new Vector2(0.25f, -0.8f),
            new Vector2(0.35f, -0.6f),
            new Vector2(0.65f, 0.6f),
            new Vector2(0.75f, 0.8f),
            new Vector2(1, 1)
        };

        float y = Util.Perlin(x, z, 0.00008f, 2, 2.38f, 1f);

        y = Util.ApplySpline(y, splinePoints);

        return y;

    }

    public static float Erosion(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
            new Vector2(0, 1),
            new Vector2(0.4f, -0.7f),
            new Vector2(1f, -1)
        };

        float y = Util.Perlin(x, z, 0.00015f, 2, 2.38f, 1f);

        y = Util.ApplySpline(y, splinePoints);

        return y;
    }

    //temperature
    // float y = Util.Perlin(x, z, 0.0009f, 3 10, 1f);

    // umidity
    // float y = Util.Perlin(x, z, 0.0009f, 3, 10, 1f);

    public static float PeaksValleys(float x, float z) {
        Vector2[] splinePoints = new Vector2[] {
        new Vector2(0, -0.5f),
        new Vector2(0.25f, -0.1f),
        new Vector2(0.5f, -0.5f),
        new Vector2(0.65f, 0.82f),
        new Vector2(1f, 1f)
    };

        float y = Util.Perlin(x, z, 0.00007f, 3, 0.45f, 1f);

        y = Util.ApplySpline(y, splinePoints);

        return y;
    }

}
