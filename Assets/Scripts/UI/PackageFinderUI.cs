using UnityEngine;
using UnityEngine.UI;

public class PackageFinderUI : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] Image Img;

    Transform TargetPosition;

    private void Awake()
    {
        PickupSpawner.OnPickupSpawned += PickupSpawner_OnPickupSpawned;
    }

    private void PickupSpawner_OnPickupSpawned(Transform target, string spawnType)
    {
        if (spawnType == "Package" ||  spawnType == "Customer")
        {
            TargetPosition = target;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Img.enabled = TargetPosition;
        if (!TargetPosition)
        {
            return;
        }
        Vector2 camDims = GetCameraDimensions();
        Vector2 dir = (TargetPosition.position - Cam.transform.position).normalized;

        float DistY = Mathf.Abs(camDims.y / dir.y);
        float DistX = Mathf.Abs(camDims.x / dir.x);
        float DistT = Vector2.Distance(Cam.transform.position, TargetPosition.position);

        float dist = Mathf.Min(DistT,Mathf.Min(DistX, DistY));

        Vector3 finalPos = Cam.WorldToScreenPoint((Vector2)Cam.transform.position + (dir * dist));
        finalPos -= (Vector3)(dir * Img.rectTransform.rect.height / 2);

        //Image is pointing up, so vector2.up is default 
        float targetRotation = Vector2.SignedAngle(Vector2.up, dir);

        Img.rectTransform.SetPositionAndRotation(
            finalPos, 
            Quaternion.Euler(0, 0, targetRotation)
        );
    }

    Vector2 GetCameraDimensions()
    {
        return new Vector2(Cam.orthographicSize * Screen.width / Screen.height, Cam.orthographicSize);
    }
}
