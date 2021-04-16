using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour { 

    System.Random rand;
    Droplet droplet;
    Brush brush; 

    public void Simulate(float[] map, int mapSize, int numRaindrops, int brushSize, int maximumCycles, float depositSpeed, float erodeSpeed) {

        //Various settings
        float inertia = .05f;
        float materialFactor = 4;
        float minMaterialCapacity = 0.01f;
        float gravity = 4;
        float evaporateSpeed = 0.1f;

        //Init
        rand = new System.Random();
        brush = new Brush(mapSize, brushSize); 


        //Simulate x number of droplets
        for (int i = 0; i < numRaindrops; i++) {

            //Create droplet object to simulate
            droplet = new Droplet(rand.Next(0, mapSize - 1), rand.Next(0, mapSize - 1), 1, 1, 0, 0, 0, brush, gravity, evaporateSpeed);

            //Simulate droplet until end of its life
            for (int currentCycle = 0; currentCycle < maximumCycles; currentCycle++) {
                int vertexX = (int)droplet.x;
                int vertexY = (int)droplet.y;
                int dropletIndex = vertexY * mapSize + vertexX; 

                //Calculate how far from vertices
                float offsetX = droplet.x - vertexX;
                float offsetY = droplet.y - vertexY;

                //Calculate height and gradient
                float[] values = droplet.Calculate(map, mapSize);

                float height = values[0];
                float gradX = values[1];
                float gradY = values[2];

                //Update droplet direction
                droplet.xDirection = (droplet.xDirection * inertia - gradX * (1 - inertia));
                droplet.yDirection = (droplet.yDirection * inertia - gradY * (1 - inertia));

                //Normalisation
                float len = Mathf.Sqrt(droplet.xDirection * droplet.xDirection + droplet.yDirection * droplet.yDirection);
                if (len != 0) {
                    droplet.xDirection /= len;
                    droplet.yDirection /= len;
                }

                //Update droplet's position
                droplet.x += droplet.xDirection;
                droplet.y += droplet.yDirection;

                //Kill the droplet if it has stopped moving or fallen off map
                if ((droplet.xDirection == 0 && droplet.yDirection == 0) || droplet.x < 0 || droplet.y < 0 || droplet.x >= mapSize - 1 || droplet.y >= mapSize - 1) {
                    break; 
                }

                //Find droplets new height
                values = droplet.Calculate(map, mapSize);
                float heightDifference = values[0] - height;

                //Material capacity (higher when moving quickly down a slope and having a lot of water)
                float materialCapacity = Mathf.Max(-heightDifference * droplet.speed * droplet.water * materialFactor, minMaterialCapacity);

                //Deposit or erode
                if (droplet.material > materialCapacity || heightDifference > 0) {
                    //Deposit if too much material or droplet has gone uphill
                    float depositAmount = (heightDifference > 0) ? Mathf.Min (heightDifference, droplet.material) : (droplet.material - materialCapacity) * depositSpeed;                   
                    droplet.Deposit(map, mapSize, depositAmount, offsetX, offsetY, dropletIndex);

                } else {
                    //Otherwise erode
                    float erodeAmount = Mathf.Min((materialCapacity - droplet.material) * erodeSpeed, -heightDifference);                  
                    droplet.Remove(map, mapSize, erodeAmount, dropletIndex);
                }

                //Update the droplet's water content and speed
                droplet.UpdateParameters(heightDifference);
            }
        }

    }
}

public class Droplet {
    public float x;
    public float y;
    public float speed;
    public float water;
    public float material;
    public float xDirection;
    public float yDirection;

    float gravity;
    float evaporateSpeed;
    Brush brush; 

    public Droplet(float x, float y, float speed, float water, float material, float xDirection, float yDirection, Brush brush, float gravity, float evaporateSpeed) {
        this.x = x;
        this.y = y;
        this.speed = speed;
        this.water = water;
        this.material = material;
        this.xDirection = xDirection;
        this.yDirection = yDirection;
        this.brush = brush;
        this.gravity = gravity;
        this.evaporateSpeed = evaporateSpeed;
    }

    public float[] Calculate(float[] map, int mapSize) {
        //Get location
        int vertexX = (int)x;
        int vertexY = (int)y;

        //Calculate how far from vertices
        float offsetX = x - vertexX;
        float offsetY = y - vertexY;

        float[] values = new float[3];

        //Get the heights of the vertices from the map
        int nwIndex = vertexY * mapSize + vertexX;
        float heightNW = map[nwIndex];
        float heightNE = map[nwIndex + 1];
        float heightSW = map[nwIndex + mapSize];
        float heightSE = map[nwIndex + mapSize + 1];

        //Calculate gradients
        float gradX = (heightNE - heightNW) * (1 - offsetY) + (heightSE - heightSW) * offsetY;
        float gradY = (heightSW - heightNW) * (1 - offsetX) + (heightSE - heightNE) * offsetX;

        //Calculate current height of droplet using interpolation
        float height = heightNW * (1 - offsetX) * (1 - offsetY) + heightNE * offsetX * (1 - offsetY) + heightSW * (1 - offsetX) * offsetY + heightSE * offsetX * offsetY;

        values[0] = height;
        values[1] = gradX;
        values[2] = gradY;

        return values;
    }


    public void UpdateParameters(float heightDifference) {
        speed = Mathf.Sqrt(speed * speed + heightDifference * gravity);
        water *= 1 - evaporateSpeed;
    }

    public void Deposit(float[] map, int mapSize, float amount, float offsetX, float offsetY, int index) {
        //Deposit specified amount to terrain
        //Droplet is inbetween 4 vertices, each need to be elevated by specific amount
        material -= amount;


        //Deposit the material to the four vertices, weighted by the distance to each.
        map[index] += amount * (1 - offsetX) * (1 - offsetY);
        map[index + 1] += amount * offsetX * (1 - offsetY);
        map[index + mapSize] += amount * (1 - offsetX) * offsetY;
        map[index + mapSize + 1] += amount * offsetX * offsetY;

    }

    public void Remove(float[] map, int mapSize, float amount, int index) {
        //Remove material smoothly from area around droplet
        for (int i = 0; i < brush.indices[index].Length; i++) {
            int vertexIndex = brush.indices[index][i];
            float weighedAmount = amount * brush.weights[index][i];

            if (map[vertexIndex] < weighedAmount) {
                weighedAmount = map[vertexIndex];
            }            

            map[vertexIndex] -= weighedAmount;
            material += weighedAmount; 
        }       
    }

}