using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float steerSpeed = 0.3f;
    [SerializeField] float moveSpeed = 0.03f;
    [SerializeField] float moveSteerRatio = -5f;

    [Header("Acceleration")]
    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] AnimationCurve reverseCurve;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float decelerationSpeed;
    [SerializeField] float brakeSpeed;
    float accelerationTime;

    [Header("Drifting")]
    [SerializeField] [Range(0,1)] float DriftThreshold;
    [SerializeField] AnimationCurve slipAmount;
    [SerializeField] float driftDeceleration;

    [Header("Effects")]
    [SerializeField] List<ParticleSystem> TireParticleGO;
    [SerializeField] SoundFader DriftAudioSource;

    Rigidbody2D rb2d;

    bool isDrifting = false;
    bool tryingToSlowdown = false;
    bool isBraking = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (accelerationTime > 0)
        {
            AudioStateMachine.SetState(AudioStateID.Main);
        }

        SetDriftEffectsActive(ShouldShowTireEffects());
    }

    void SetDriftEffectsActive(bool active)
    {
        DriftAudioSource.SetPlaying(active);
        foreach (ParticleSystem gen in TireParticleGO)
        {
            if (active)
            {
                gen.Play();
            }
            else
            {
                gen.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 input = new Vector2(-Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        tryingToSlowdown = Mathf.Abs(input.y - accelerationTime) > Mathf.Abs(input.y);
        isBraking = Mathf.Abs(input.y - accelerationTime) > 1;

        //Use decelerationSpeed if we are currently trying to slow down
        float accelerationChange = (isBraking ? brakeSpeed : (tryingToSlowdown ? decelerationSpeed : accelerationSpeed)) * Time.deltaTime;
        accelerationChange = Mathf.Min(accelerationChange, Mathf.Abs(input.y - accelerationTime));
        if (input.y < accelerationTime)
        {
            //Moving in reverse
            accelerationChange *= -1;
        }
        accelerationTime += accelerationChange;
        if (isDrifting)
        {
            accelerationTime -= driftDeceleration * Time.deltaTime;
        }


        float moveAmount = getMovementCurve() * moveSpeed;
       
        float steerAmount = Mathf.Abs(getMovementCurve()) * (steerSpeed + (Mathf.Abs(getMovementCurve()) * moveSteerRatio)) * Time.fixedDeltaTime * input.x;

        Vector2 driveDir = (Vector2)transform.up;
        Vector2 slipVal = rb2d.linearVelocity * slipAmount.Evaluate(accelerationTime);
        float dot = Vector2.Dot(driveDir.normalized, slipVal.normalized);
        isDrifting = dot <= DriftThreshold && accelerationTime > 0.2f;
        if (isDrifting)
        {
            if (dot < 0)
            {
                accelerationTime -= 25 * driftDeceleration * Time.fixedDeltaTime;
            }
            
            //Not Accelerating mid drift
            if(input.y <= 0)
            {
                accelerationTime -= (1 - Mathf.Abs(dot)) * driftDeceleration * Time.fixedDeltaTime;
            }
        }
        Vector2 netDirection = (driveDir + slipVal).normalized;
        rb2d.MoveRotation(rb2d.rotation + steerAmount);
        rb2d.linearVelocity = netDirection * moveAmount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Slow the car down on collision.
        accelerationTime = Mathf.Min(0, Mathf.Abs(accelerationTime - 0.3f)) * Mathf.Sign(accelerationTime);
    }

    public float getMovementCurve()
    {
        float curveVal = 0;
        if (accelerationTime > 0)
        {
            curveVal = accelerationCurve.Evaluate(accelerationTime);
        } 
        else
        {
            curveVal = -reverseCurve.Evaluate(-accelerationTime);
        }

        return curveVal;
    }

    bool ShouldShowTireEffects()
    {
        return isDrifting || isBraking;
    }

}
