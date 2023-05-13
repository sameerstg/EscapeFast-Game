using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    int amountOfPlatfromAtATime = 6;
    public List<GameObject> platformsPrefabs = new();
    public List<GameObject> generatedPlatforms = new();
    public static PlatformGenerator _instance;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        GeneratePlatform();
    }
  
    public void PlatformPool(GameObject platform)
    {
        
        if (platform != generatedPlatforms[1] && platform != generatedPlatforms[0])
        {
                       GameObject g = generatedPlatforms[0];
            generatedPlatforms.Remove(generatedPlatforms[0]);
            Destroy(g);
            GeneratePlatform();
        }

    }
    public void GeneratePlatform()
    {
        while (generatedPlatforms.Count<amountOfPlatfromAtATime)
        {
            generatedPlatforms.Add(Instantiate(platformsPrefabs[Random.Range(0, platformsPrefabs.Count)], GetLastPlatformEndPosition(), Quaternion.identity,transform));
        }
    }
    Vector3 GetLastPlatformEndPosition()
    {
        if (generatedPlatforms.Count == 0)
        {
            return new Vector3();
        }
        else
        {
            return generatedPlatforms[^1].transform.position + (Vector3.forward * generatedPlatforms[^1].GetComponentInChildren<Renderer>().bounds.size.z);
        }
    }
}
