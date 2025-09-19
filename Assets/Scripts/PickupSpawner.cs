using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] GameObject PackagePrefab;
    [SerializeField] GameObject CustomerPrefab;
    [SerializeField] GameObject BoostPrefab;

    [SerializeField] Transform SpawnPointParent;
    List<Transform> SpawnPoints;

    //Events
    public delegate void TargetSpawnDelegate(Transform target, string spawnType);
    public static event TargetSpawnDelegate OnPickupSpawned;

    private void Awake()
    {
        SpawnPoints = SpawnPointParent.GetComponentsInChildren<Transform>().ToList();
        Delivery.OnPickup += Delivery_OnPickup;
    }

    private void Start()
    {
        SpawnPickup(PackagePrefab, "Package");
    }

    private void Delivery_OnPickup(string pickupType)
    {
        switch (pickupType)
        {
            case "Package":
                //Spawn a customer for this package to go to
                SpawnPickup(CustomerPrefab, "Customer");
                break;
            case "Customer":
                SpawnPickup(PackagePrefab, "Package");
                break;

        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        return SpawnPoints[Random.Range(0, SpawnPoints.Count - 1)].position;
    }
    void SpawnPickup(GameObject prefab, string spawnType)
    {
        GameObject newPackage = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);
        OnPickupSpawned?.Invoke(newPackage.transform, spawnType);
    }
}
