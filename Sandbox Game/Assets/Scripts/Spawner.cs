using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int MaxCarInstances = 30;
    public GameObject[] carPrefabs;
    private GameObject[] spawnedCars;

    public int MaxEnemyInstances = 30;
    public GameObject[] emeyPrefabs;
    private GameObject[] spawnedEnemies;

    public int MaxPedestrianInstances = 30;
    public GameObject[] pedestrianPrefabs;
    private GameObject[] spawnedPedestrians;

    public float MaxDistance;
    public float MinSpawnDistance;

    private GameObject player;

    void Start()
    {
        float vol = ((float)PlayerPrefs.GetInt("Volume", 100)) / 100f;
        AudioListener.volume = vol;


        player = GameObject.FindGameObjectWithTag("Player");
        spawnedCars = new GameObject[MaxCarInstances];
        spawnedEnemies = new GameObject[MaxEnemyInstances];
        spawnedPedestrians = new GameObject[MaxPedestrianInstances];

        Spawn(0, MaxDistance);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < spawnedPedestrians.Length; i++)
        {
            float distance = (player.transform.position - spawnedPedestrians[i].transform.position).magnitude;
            if(distance > MaxDistance || !spawnedPedestrians[i].activeSelf) 
            {
                respawnPedestrian(i);
            }
        }

        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            float distance = (player.transform.position - spawnedEnemies[i].transform.position).magnitude;
            if (distance > MaxDistance || !spawnedEnemies[i].activeSelf)
            {
                respawnEnemy(i);
            }
        }

        for (int i = 0; i < spawnedCars.Length; i++)
        {
            float distance = (player.transform.position - spawnedCars[i].transform.position).magnitude;
            if (distance > MaxDistance || !spawnedCars[i].activeSelf)
            {
                respawnCar(i);
            }
        }

    }

    void Spawn(float minDistance, float maxDistance)
    {
        List<Road> roadTargetList = getRoadList(minDistance, maxDistance);

        List<GameObject> sidewalksTargetList = getSidewalkList(minDistance, maxDistance);

        var random = new System.Random();

        for (int i = 0; i < MaxCarInstances; i++)
        {
            int prefabIndex = random.Next(carPrefabs.Length);


            int targetIndex = random.Next(roadTargetList.Count);
            Road targetRoad = roadTargetList[targetIndex];

            int side = (random.Next(2) > 0) ? 1 : -1;
            Vector3 spawnPos = targetPosFromRoad(targetRoad, side);
            while (Physics.OverlapBox(spawnPos, new Vector3(2.5f, 0, 2.5f)).Length > 1)
            {
                side = (random.Next(2) > 0) ? 1 : -1;
                targetIndex = random.Next(roadTargetList.Count);
                targetRoad = roadTargetList[targetIndex];
                spawnPos = targetPosFromRoad(targetRoad, side);
            }
            Quaternion rot = Quaternion.Euler(0, targetRoad.transform.rotation.eulerAngles.y + 90 + side * 90, 0);

            var prefab = carPrefabs[prefabIndex];
            prefab.GetComponent<CarManager>().lastRoad = targetRoad;
            prefab.GetComponent<CarManager>().CarStatus = CarManager.CarMode.npc;
            spawnedCars[i] = Instantiate(carPrefabs[prefabIndex], spawnPos, rot); 
        }

        for (int i = 0; i < MaxPedestrianInstances; i++)
        {
            int index = random.Next(sidewalksTargetList.Count);
            Collider targetSidewalk = sidewalksTargetList[index].GetComponent<Collider>();
            int prefabIndex = random.Next(pedestrianPrefabs.Length);
            Vector3 spawnPos = RandomPointInBounds(targetSidewalk.bounds);
            spawnPos.y = 0.5f;
            spawnedPedestrians[i] = Instantiate(pedestrianPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        }

        for (int i = 0; i < MaxEnemyInstances; i++)
        {
            int index = random.Next(roadTargetList.Count);
            Road targetRoad = roadTargetList[index];
            int prefabIndex = random.Next(emeyPrefabs.Length);
            Vector3 spawnPos = RandomPointInBounds(targetRoad.roadA.bounds);
            spawnPos.y = 0.5f;
            spawnedEnemies[i] = Instantiate(emeyPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        }
    }

    public void respawnPedestrian(int index)
    {
        spawnedPedestrians[index].SetActive(false);
        List<GameObject> sidewalksTargetList = getSidewalkList(MinSpawnDistance, MaxDistance);
        var random = new System.Random();
        int targetIndex = random.Next(sidewalksTargetList.Count);
        Collider targetSidewalk = sidewalksTargetList[targetIndex].GetComponent<Collider>();

        Vector3 spawnPos = RandomPointInBounds(targetSidewalk.bounds);
        spawnPos.y = 0.5f;
        spawnedPedestrians[index].transform.position =  spawnPos;
        spawnedPedestrians[index].SetActive(true);
    }

    public void respawnEnemy(int index) 
    {
        spawnedEnemies[index].GetComponent<DropItem>().dontDrop = true;
        spawnedEnemies[index].SetActive(false);
        List<Road> roadTargetList = getRoadList(MinSpawnDistance, MaxDistance);
        var random = new System.Random();
        int targetIndex = random.Next(roadTargetList.Count);
        Road targetRoad = roadTargetList[targetIndex];
        Vector3 spawnPos = RandomPointInBounds(targetRoad.roadA.bounds);
        spawnPos.y = 0.5f;
        spawnedEnemies[index].transform.position = spawnPos;
        spawnedEnemies[index].GetComponent<DropItem>().dontDrop = false;
        spawnedEnemies[index].SetActive(true);
    }

    public void respawnCar(int index)
    {
        spawnedCars[index].GetComponent<Rigidbody>().isKinematic = true;
        spawnedCars[index].SetActive(false);

        List<Road> roadTargetList = getRoadList(MinSpawnDistance, MaxDistance);
        var random = new System.Random();
        int targetIndex = random.Next(roadTargetList.Count);
        Road targetRoad = roadTargetList[targetIndex];


        int side = (random.Next(2) > 0) ? 1 : -1;
        Vector3 spawnPos = targetPosFromRoad(targetRoad,side);


        while (Physics.OverlapBox(spawnPos, new Vector3(2.5f, 0, 2.5f)).Length > 1)
        {
            side = (random.Next(2) > 0) ? 1 : -1;
            targetIndex = random.Next(roadTargetList.Count);
            targetRoad = roadTargetList[targetIndex];
            spawnPos = targetPosFromRoad(targetRoad, side);       
        }
        Quaternion rot = Quaternion.Euler(0, targetRoad.transform.rotation.eulerAngles.y + 90 + side * 90, 0);

        spawnedCars[index].transform.position = spawnPos;
        spawnedCars[index].transform.rotation = rot;
        spawnedCars[index].GetComponent<CarManager>().lastRoad = targetRoad;
        spawnedCars[index].GetComponent<CarManager>().isRoadA = true;
        spawnedCars[index].GetComponent<Rigidbody>().isKinematic = false;
        spawnedCars[index].GetComponent<CarManager>().CarStatus = CarManager.CarMode.npc;
        spawnedCars[index].SetActive(true);
    }

    private Vector3 targetPosFromRoad(Road targetRoad, int side)
    {
        Vector3 spawnPos = RandomPointInBounds(targetRoad.roadA.bounds);
        Vector3 centerRoad = transform.TransformPoint(targetRoad.roadA.bounds.center + side * targetRoad.roadA.bounds.extents / 2);
        Vector3 localPos = transform.TransformPoint(spawnPos);
        localPos.x = centerRoad.x;
        spawnPos = transform.InverseTransformPoint(localPos);
        spawnPos.y = 0.1f;
        return spawnPos;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private List<GameObject> getSidewalkList(float minDistance, float maxDistance)
    {
        List<GameObject> sidewalksTargetList = new List<GameObject>();
        GameObject[] sidewalks = GameObject.FindGameObjectsWithTag("Sidewalk");
        foreach (GameObject sidewalk in sidewalks)
        {
            float distance = (player.transform.position - sidewalk.transform.position).magnitude;
            if (distance > minDistance && distance < maxDistance)
            {
                sidewalksTargetList.Add(sidewalk);
            }
        }
        return sidewalksTargetList;
    }


    private List<Road> getRoadList(float minDistance, float maxDistance)
    {
        List<Road> roadTargetList = new List<Road>();
        Road[] roads = FindObjectsOfType<Road>();
        foreach (Road road in roads)
        {
            float distance = (player.transform.position - road.transform.position).magnitude;
            if (!road.esCruce && (distance > minDistance))
            {
                roadTargetList.Add(road);
            }
        }
        return roadTargetList;
    }
}
