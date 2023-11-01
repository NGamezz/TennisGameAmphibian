using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs = new();
    [SerializeField] private float amountOfObjectsToSpawn;
    [SerializeField] private Vector2 positionBoundaryMax;
    [SerializeField] private Vector2 positionBoundaryMin;

    void Start()
    {
        if (objectPrefabs.Count < 1) { return; }

        for (int i = 0; i < amountOfObjectsToSpawn; i++)
        {
            GameObject currentObject = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Count)]);
            currentObject.transform.position = new Vector3(Random.Range(positionBoundaryMin.x, positionBoundaryMax.x), 0.0f, Random.Range(positionBoundaryMin.y, positionBoundaryMax.y));
        }
    }
}
