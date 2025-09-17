using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioStateMachine : MonoBehaviour
{
    [SerializeField] List<AudioState> States;
    AudioStateID currentState = AudioStateID.Intro;

    AudioStateID targetState = AudioStateID.Intro;

    float timeSinceLastTransitionPoint;
    bool isTransitioning;


    static AudioStateMachine Instance;
    AudioSource Source;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            Source = GetComponent<AudioSource>();
            Source.loop = true;
            Source.Stop();
            OnTransitionEnd();
        } else
        {
            Debug.LogError("Multiple AudioStateMachines defined!");
            Destroy(gameObject);
        }
    }


    void Update()
    {
        if(!isTransitioning)
        {
            timeSinceLastTransitionPoint += Time.deltaTime;
            if(timeSinceLastTransitionPoint > States[(int)currentState].TransitionPoint)
            {
                timeSinceLastTransitionPoint = 0;
                OnTransitionStart();
            }
        } else
        {
            if(!Source.isPlaying)
            {
                OnTransitionEnd();
                timeSinceLastTransitionPoint = 0;
            }
        }
    }
    void OnTransitionStart()
    {
        if (currentState == targetState) return;
        AudioStateTransiton transition = 
            States[(int)currentState].Transitions.FirstOrDefault(x => x.EndingState == targetState);

        Source.loop = false;
        Source.clip = transition.TransitionClip;
        Source.Play();

        currentState = targetState;
        isTransitioning = true;
    }
    void OnTransitionEnd()
    {
        Source.clip = Instance.States[(int)currentState].MusicClip;
        Source.loop = true;
        Instance.Source.Play();
        isTransitioning = false;
    }
    public static void SetState(AudioStateID stateID)
    {
        if (Instance.targetState == stateID) return;
        Instance.targetState = stateID;
    }
}

[System.Serializable]
public class AudioState
{
    public AudioClip MusicClip;
    public float TransitionPoint;
    public List<AudioStateTransiton> Transitions;
}

[System.Serializable]
public class AudioStateTransiton
{
    public AudioClip TransitionClip;
    public AudioStateID EndingState;
}


public enum AudioStateID
{
    None = -1,
    Intro,
    Main,
}