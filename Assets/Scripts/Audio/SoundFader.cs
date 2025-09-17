using UnityEngine;

public class SoundFader : MonoBehaviour
{
    [SerializeField] AudioSource Source;
    [SerializeField] float FadeDuration = 1.0f;

    private float targetVolume;
    private float maxVolume;


    private void Awake()
    {
        maxVolume = Source.volume;
        Source.volume = 0;
        SetPlaying(false);
    }

    void Update()
    {
        float amtToChange = Mathf.Min(Mathf.Abs(targetVolume - Source.volume), Time.deltaTime * maxVolume / FadeDuration);
        amtToChange *= Mathf.Sign(targetVolume - Source.volume);
        Source.volume += amtToChange;
    }



    public void SetPlaying(bool isPlaying)
    {
        targetVolume = isPlaying ? maxVolume : 0.0f;
    }
}
