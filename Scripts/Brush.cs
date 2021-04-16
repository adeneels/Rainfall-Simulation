using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush {

    public int[][] indices;
    public float[][] weights;


    //Brush object, logic & math taken from http://ranmantaru.com/blog/2011/10/08/water-erosion-on-heightmap-terrain/ by E-DOG

    public Brush(int mapSize, int radius) {
        indices = new int[mapSize * mapSize][];
        weights = new float[mapSize * mapSize][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] localWeights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < indices.GetLength(0); i++) {
            int centreX = i % mapSize;
            int centreY = i / mapSize;

            if (centreY <= radius || centreY >= mapSize - radius || centreX <= radius + 1 || centreX >= mapSize - radius) {
                weightSum = 0;
                addIndex = 0;
                for (int y = -radius; y <= radius; y++) {
                    for (int x = -radius; x <= radius; x++) {
                        float sqrDst = x * x + y * y;
                        if (sqrDst < radius * radius) {
                            int coordX = centreX + x;
                            int coordY = centreY + y;

                            if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize) {
                                float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                weightSum += weight;
                                localWeights[addIndex] = weight;
                                xOffsets[addIndex] = x;
                                yOffsets[addIndex] = y;
                                addIndex++;
                            }
                        }
                    }
                }
            }

            int numEntries = addIndex;
            indices[i] = new int[numEntries];
            weights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++) {
                indices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                weights[i][j] = localWeights[j] / weightSum;
            }
        }
    }
}


