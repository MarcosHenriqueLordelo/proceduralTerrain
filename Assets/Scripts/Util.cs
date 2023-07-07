using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    public static float fBM(float x, float y, int oct, float persistance) {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < oct; i++) {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= 2;
        }

        return total / maxValue;
    }

    public static float Perlin(float x, float y, float scale, int octaves, float persistance, float heightScale) {
        return fBM(x * scale, y * scale, octaves, persistance) * heightScale;
    }

    public static float ApplySpline(float t, Vector2[] points) {
        int n = points.Length - 1;
        int i = Mathf.FloorToInt(t * n);

        // Clamp the index to the valid range of points
        i = Mathf.Clamp(i, 0, n - 1);

        // Compute the interval parameter
        float ti = t * n - i;

        // Compute the spline coefficients for the interval
        float a = points[i].y;
        float b = points[i + 1].y;
        float c = (points[i + 1].y - points[i].y) * 3f - points[i].x * 2f - points[i + 1].x;

        // Evaluate the cubic Hermite spline
        float y = a + ti * (c + ti * (b - a - c));

        return y;
    }

    public static float DistanceBtwPoints(Vector2 a, Vector2 b) {
        return (float)Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    public static float CalculateY(float x, float z, int maxY, int minY = 0) {
        float angle = (float)Mathf.PI * 1 * (x + z) / (maxY * 2);
        float sineValue = (float)Mathf.Sin(angle);
        float y = sineValue * maxY;

        if (y < minY) {
            return minY;
        }

        if (y > maxY) {
            return maxY;
        }

        return y;
    }

}
