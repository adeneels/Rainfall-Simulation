using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

   
    public int seed = 0;
    public float noiseScale = 2;
    public int octaves = 6;
    public float persistence = .5f;

    public float heightMultiplier = 5;
    public int mapSize = 750;
    public int scale = 20;

    //Be careful with the number of droplets, increase compute time heavily.
    public int numDroplets = 1000000;
    public int brushSize = 3;
    public int dropletLifeCycles = 15;
    public float depositionSpeed = 0.6f;
    public float erosionSpeed = 0.6f;


    //Used for showing noise
    public Material material;
    public Renderer noiseRenderer;

    //For texturing mesh
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    //Locals
    Mesh mesh;
    float[] map;
    int mapPlusBorder;


    //Called to generate a heightmap
    public void Generate() {
        mapPlusBorder = mapSize + brushSize * 2;
        map = FindObjectOfType<Noise>().GenerateHeightMap(mapPlusBorder, persistence, octaves, noiseScale, seed);
    }


    //Called to generate a mesh from heightmap data
    public void BuildMesh() {
        MeshData meshData = MeshGenerator.CreateMeshData(map, mapPlusBorder, heightMultiplier, scale);
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
    }

    //Called to simulate rain on mesh
    public void Rainfall() {
        FindObjectOfType<Rain>().Simulate(map, mapPlusBorder, numDroplets, brushSize, dropletLifeCycles, depositionSpeed, erosionSpeed);
    }

    //Discontinued method used to visualise noise.
    //To use this set MapGenerator's noiseRenderer as a 2D plane object, and then call this method.

    public void drawNoise(float[] map, int mapSize) {
        Texture texture = FindObjectOfType<Noise>().noiseToTexture(map, mapSize);
        noiseRenderer.sharedMaterial.mainTexture = texture;
        noiseRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }


    //Setters for User interface...
    public void setRaindrops() {
        numDroplets = (int)GameObject.Find("rainSlider").GetComponent<Slider>().value;
        GameObject.Find("numRaindrops").GetComponent<Text>().text = numDroplets.ToString();
    }
    
    public void setWaterLife() {
        dropletLifeCycles = (int)GameObject.Find("cyclesSlider").GetComponent<Slider>().value;
        GameObject.Find("currentCycles").GetComponent<Text>().text = dropletLifeCycles.ToString();
    }
    public void setErodeSpeed() {
        erosionSpeed = GameObject.Find("erosionSpeedSlider").GetComponent<Slider>().value;
        GameObject.Find("currentErosionSpeed").GetComponent<Text>().text = erosionSpeed.ToString();
    }
    public void setSeed() {
        seed = (int)GameObject.Find("seedSlider").GetComponent<Slider>().value;
        GameObject.Find("currentSeed").GetComponent<Text>().text = seed.ToString();
    }
    public void setMapSize() {
        mapSize = (int)GameObject.Find("mapSizeSlider").GetComponent<Slider>().value;
        GameObject.Find("currentMapSize").GetComponent<Text>().text = mapSize.ToString();
    }
    public void setHeightMultiplier() {
        heightMultiplier = (int)GameObject.Find("heightMultiplierSlider").GetComponent<Slider>().value;
        GameObject.Find("currentHeightMultiplier").GetComponent<Text>().text = heightMultiplier.ToString();
    }

    public void setNoiseScale() {
        noiseScale = (int)GameObject.Find("noiseScaleSlider").GetComponent<Slider>().value;
        GameObject.Find("currentNoiseScale").GetComponent<Text>().text = noiseScale.ToString();
    }

    public void setDepositSpeed() {
        depositionSpeed = GameObject.Find("depositSpeedSlider").GetComponent<Slider>().value;
        GameObject.Find("currentDepositSpeed").GetComponent<Text>().text = depositionSpeed.ToString();
    }

}
