using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public List<GameObject> obstacles;
    public void GenerateObstacle()
    {
        int ran = Random.Range(0, 2);
        obstacles[ran].gameObject.SetActive(false);
        obstacles[ran ^ 1].gameObject.SetActive(true);
    }
}
