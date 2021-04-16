using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator {
    public static MeshData CreateMeshData(float[] map, int mapSize, float heightMultiplier, int scale) {
        MeshData meshData = new MeshData(mapSize, mapSize);
        int t = 0; 

        for (int i = 0; i < mapSize * mapSize; i++) {
            int x = i % mapSize;
            int y = i / mapSize;

            Vector2 percent = new Vector2(x / (mapSize - 1f), y / (mapSize - 1f));
            Vector3 position = new Vector3(percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;

            position += Vector3.up * map[i] * heightMultiplier;
            meshData.vertices[i] = position;

            if (x < mapSize - 1 && y < mapSize - 1) {
                meshData.AddTriangles(t, i + mapSize, i + mapSize + 1, i,
                    i + mapSize + 1, i + 1, i);
                t += 6;
            }
        }
        return meshData;
    }

}

public class MeshData {

    public Vector3[] vertices;
    public int[] triangles;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangles(int triangleIndex, int a, int b, int c, int d, int e, int f) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex + 2] = c;
        triangles[triangleIndex+3] = d;
        triangles[triangleIndex+4] = e;
        triangles[triangleIndex+5] = f;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

}
