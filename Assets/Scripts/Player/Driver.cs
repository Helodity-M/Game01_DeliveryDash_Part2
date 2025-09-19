using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float steerSpeed = 0.3f;
    [SerializeField] float moveSpeed = 0.03f;
    [SerializeField] float moveSteerRatio = -5f;

    [Header("Boost")]
    [SerializeField] float BoostDuration = 2;
    [SerializeField] float BoostMultiplier = 1.5f;
    float boostTimeRemaining = 0;

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
    bool isSpinningOut = false;

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
        Vector2 input = GetInput();
        float accelerationChange = GetAccelerationChange(input.y);
        accelerationTime += accelerationChange;

        float moveAmount = getMovementCurve() * moveSpeed;
        float steerAmount = Mathf.Abs(getMovementCurve()) * (steerSpeed + (Mathf.Min(1,Mathf.Abs(getMovementCurve())) * moveSteerRatio)) * Time.fixedDeltaTime * input.x;

        Vector2 netDirection = GetAccelerationDirection();

        rb2d.MoveRotation(rb2d.rotation + steerAmount);
        rb2d.linearVelocity = netDirection * moveAmount;

        UpdateBrakingStatus();
        boostTimeRemaining -= Time.fixedDeltaTime;
    }
    private Vector2 GetInput()
    {
        return new Vector2(-Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void UpdateBrakingStatus()
    {
        Vector2 input = GetInput();
        tryingToSlowdown = Mathf.Abs(input.y - accelerationTime) > Mathf.Abs(input.y);
        isBraking = Mathf.Abs(input.y - accelerationTime) > 1;

        Vector2 driveDir = (Vector2)transform.up;
        Vector2 slipVal = rb2d.linearVelocity * slipAmount.Evaluate(accelerationTime);
        float dot = Vector2.Dot(driveDir.normalized, slipVal.normalized);
        isDrifting = dot <= DriftThreshold && accelerationTime > 0.2f;
        isSpinningOut = dot < 0;
    }
    private float GetAccelerationChange(float targetTime)
    {
        //Determine which base speed to use
        float accelerationChange = (isBraking ? brakeSpeed : (tryingToSlowdown ? decelerationSpeed : accelerationSpeed)) * Time.fixedDeltaTime;
        //Clamp the speed change to not overshoot
        accelerationChange = Mathf.Min(accelerationChange, Mathf.Abs(targetTime - accelerationTime));

        //Moving in reverse
        if (targetTime < accelerationTime)
        {
            accelerationChange *= -1;
        }

        //Drifting deceleration
        if (isDrifting)
        {
            //Base drift deceleration
            accelerationChange -= driftDeceleration * Time.fixedDeltaTime;

            //Spinning out, apply a strong slowdown.
            if (isSpinningOut)
            {
                accelerationTime -= 25 * driftDeceleration * Time.fixedDeltaTime;
            }
        }
        return accelerationChange;
    }

    private Vector2 GetAccelerationDirection()
    {
        Vector2 driveDir = (Vector2)transform.up;
        Vector2 slipVal = rb2d.linearVelocity * slipAmount.Evaluate(accelerationTime);

        return (driveDir + slipVal).normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Slow the car down on collision.
        accelerationTime = Mathf.Min(0, Mathf.Abs(accelerationTime - 0.3f)) * Mathf.Sign(accelerationTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boost"))
        {
            //If you are going really slow, get up to speed
            accelerationTime = Mathf.Max(0.5f, accelerationTime) * Mathf.Sign(accelerationTime);
            boostTimeRemaining = BoostDuration;
            Destroy(collision.gameObject);
        }
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
        if(boostTimeRemaining > 0)
        {
            curveVal *= BoostMultiplier;
        }
        return curveVal;
    }

    bool ShouldShowTireEffects()
    {
        return isDrifting || isBraking;
    }

}
