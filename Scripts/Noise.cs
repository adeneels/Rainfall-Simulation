using UnityEngine;

public class Noise : MonoBehaviour {
    public ComputeShader noiseShader;

    public float[] GenerateHeightMap (int mapSize, float persistence, int octaves, float scale, int seed) {
        float[] map = new float[mapSize * mapSize];     

        //setup of buffer used to retrieve data
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(int));
        mapBuffer.SetData(map);
        noiseShader.SetBuffer(0, "heightMap", mapBuffer);

        //setup of buffer used to retrieve data
        int multiplier = 1000;
        int[] minMaxHeight = { multiplier * octaves, 0 };
        ComputeBuffer minMaxBuffer = new ComputeBuffer(minMaxHeight.Length, sizeof(int));
        minMaxBuffer.SetData(minMaxHeight);
        noiseShader.SetBuffer(0, "minMax", minMaxBuffer);

        //Generating random offsets using the seet for randomised heightmap
        System.Random rand = new System.Random(seed);
        Vector2[] offsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            offsets[i] = new Vector2(rand.Next(-10000, 10000), rand.Next(-10000, 10000));
        }
        ComputeBuffer offsetBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 2);
        offsetBuffer.SetData(offsets);
        noiseShader.SetBuffer(0, "offsets", offsetBuffer);

        //setting the variables inside the compute shader
        noiseShader.SetInt("mapSize", mapSize);
        noiseShader.SetInt("octaves", octaves);
        noiseShader.SetFloat("lacunarity", 2f);
        noiseShader.SetFloat("persistence", persistence);
        noiseShader.SetFloat("scaleFactor", scale);
        noiseShader.SetInt("multiplier", multiplier);

        //executing the shader
        //Shader taken from Noise Shader Library for Unity - https://github.com/keijiro/NoiseShader
        noiseShader.Dispatch(0, Mathf.CeilToInt(map.Length / 64f), 1, 1);

        //gets the data from shader into map, minMaxHeight
        mapBuffer.GetData(map);
        minMaxBuffer.GetData(minMaxHeight);
        mapBuffer.Release();
        minMaxBuffer.Release();
        offsetBuffer.Release();

        float minValue = (float)minMaxHeight[0] / (float)multiplier;
        float maxValue = (float)minMaxHeight[1] / (float)multiplier;

        for (int i = 0; i < map.Length; i++) {
            map[i] = Mathf.InverseLerp(minValue, maxValue, map[i]);
        }

        return map; 

    }


    public Texture2D noiseToTexture(float[] map, int mapSize) {
        int width = mapSize;
        int height = mapSize;

        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];

        for (int i = 0; i < map.Length; i++) {
            colourMap[i] = Color.Lerp(Color.black, Color.white, map[i]);
        }
        return colourMapToTexture(colourMap, width, height);
    }

    public Texture2D colourMapToTexture(Color[] colourMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

}
