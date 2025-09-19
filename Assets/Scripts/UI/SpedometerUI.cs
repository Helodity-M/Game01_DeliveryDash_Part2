using UnityEngine;

public class SpedometerUI : MonoBehaviour
{
    [SerializeField] Driver Driver;
    [SerializeField] RectTransform BarImg;
    [SerializeField] float snapSpeed;
    [SerializeField] float minRotation = 0;
    [SerializeField] float maxRotation = 180;

    float curRotation = 0;

    // Update is called once per frame
    void Update()
    {
        float targetRotation = Mathf.Lerp(minRotation, maxRotation, Mathf.Abs(Driver.getMovementCurve()));

        curRotation = Mathf.Lerp(curRotation, targetRotation, Time.deltaTime * snapSpeed);
        BarImg.SetPositionAndRotation(BarImg.position, Quaternion.Euler(0, 0, curRotation));
    }
}
