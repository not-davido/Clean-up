using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Footsteps))]
public class TerrainTextureCheck : MonoBehaviour
{
    private List<TerrainLayer> terrainLayers = new();
    private Terrain terrain;
    private float[] textureValues; //The amount of terrain layer textures our Terrain has
    private int posX;
    private int posZ;

    public int Length => textureValues.Length;
    public IList<TerrainLayer> Layers => terrainLayers.AsReadOnly();

    public float this[int index] {
        get {
            if (index < 0 || index > textureValues.Length) {
                throw new IndexOutOfRangeException();
            }

            return textureValues[index];
        }
    }

    void Start() {
        terrain = Terrain.activeTerrain;

        int i = 0;

        foreach (var layer in terrain.terrainData.terrainLayers) {
            terrainLayers.Add(layer);
            i++;
        }

        textureValues = new float[i];
    }

    public void GetTerrainTexture() {
        ConvertPosition(transform.position);
        CheckTexture();
    }

    void ConvertPosition(Vector3 playerPosition) {
        Vector3 terrainPosition = playerPosition - terrain.transform.position;

        Vector3 mapPosition = new(terrainPosition.x / terrain.terrainData.size.x, 0 ,terrainPosition.z / terrain.terrainData.size.z);

        float xCoord = mapPosition.x * terrain.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * terrain.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    void CheckTexture() {
        float[,,] aMap = terrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        for (int i = 0; i < textureValues.Length; i++) {
            textureValues[i] = aMap[0, 0, i];
        }
    }
}
