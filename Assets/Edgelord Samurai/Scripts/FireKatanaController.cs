using UnityEngine;

/// <summary>
/// Dynamic Pyrotechnic Katana System
/// Provides velocity-reactive fire VFX and audio feedback for premium VR immersion
/// Optimized for Meta Quest 3 (90 FPS target)
/// </summary>
public class FireKatanaController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Maximum velocity threshold (m/s) for full particle/audio intensity")]
    public float maxVelocity = 5.0f;

    [Tooltip("Smoothing time for velocity changes (lower = more responsive)")]
    public float smoothTime = 0.1f;

    [Header("Visuals - Particle Systems")]
    [Tooltip("Core fire particles (Local space) - moves with blade")]
    public ParticleSystem coreFireParticles;

    [Tooltip("Trail fire particles (World space) - stays in air")]
    public ParticleSystem trailFireParticles;

    [Tooltip("Minimum emission rate at rest")]
    public float minEmission = 10f;

    [Tooltip("Maximum emission rate at full velocity")]
    public float maxEmission = 100f;

    [Header("Audio")]
    [Tooltip("Audio source for swing whoosh sound")]
    public AudioSource swingAudioSource;

    [Tooltip("Minimum pitch at rest")]
    public float minPitch = 0.8f;

    [Tooltip("Maximum pitch at full velocity")]
    public float maxPitch = 1.3f;

    [Tooltip("Audio source for constant fire crackling (optional)")]
    public AudioSource idleCrackleSource;

    // Private state
    private Vector3 lastPosition;
    private float currentVelocity;
    private float velocityVelocity; // Used by SmoothDamp

    private ParticleSystem.EmissionModule coreEmission;
    private ParticleSystem.EmissionModule trailEmission;

    void Start()
    {
        // Cache particle emission modules for performance
        if (coreFireParticles != null)
        {
            coreEmission = coreFireParticles.emission;
        }

        if (trailFireParticles != null)
        {
            trailEmission = trailFireParticles.emission;
        }

        // Initialize position tracking
        lastPosition = transform.position;

        // Start idle crackling sound if assigned
        if (idleCrackleSource != null && !idleCrackleSource.isPlaying)
        {
            idleCrackleSource.Play();
        }

        // Validate setup
        ValidateSetup();
    }

    void Update()
    {
        CalculateVelocity();
        UpdateVisuals();
        UpdateAudio();
    }

    /// <summary>
    /// Calculate smoothed velocity based on position delta
    /// </summary>
    private void CalculateVelocity()
    {
        // Calculate instantaneous velocity
        float distance = Vector3.Distance(transform.position, lastPosition);
        float rawVelocity = distance / Time.deltaTime;

        // Smooth the velocity to prevent jitter
        currentVelocity = Mathf.SmoothDamp(
            currentVelocity,
            rawVelocity,
            ref velocityVelocity,
            smoothTime
        );

        // Update last position for next frame
        lastPosition = transform.position;
    }

    /// <summary>
    /// Update particle systems based on velocity
    /// </summary>
    private void UpdateVisuals()
    {
        if (coreFireParticles == null && trailFireParticles == null)
            return;

        // Normalize velocity to 0-1 range
        float intensity = Mathf.Clamp01(currentVelocity / maxVelocity);

        // Update core fire (constant emission based on velocity)
        if (coreFireParticles != null)
        {
            float coreRate = Mathf.Lerp(minEmission, maxEmission * 0.5f, intensity);
            coreEmission.rateOverTime = coreRate;
        }

        // Update trail fire (emission over distance - leaves fire trail in air)
        if (trailFireParticles != null)
        {
            // Use rateOverDistance for trails - particles spawn based on movement
            float trailRate = Mathf.Lerp(0f, 20f, intensity);
            trailEmission.rateOverDistance = trailRate;
        }
    }

    /// <summary>
    /// Update audio feedback based on velocity
    /// </summary>
    private void UpdateAudio()
    {
        if (swingAudioSource == null)
            return;

        // Normalize velocity to 0-1 range
        float intensity = Mathf.Clamp01(currentVelocity / maxVelocity);

        // Update volume (0 at rest, 1 at max velocity)
        swingAudioSource.volume = intensity;

        // Update pitch (lower at rest, higher at max velocity)
        swingAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, intensity);

        // Start/stop audio based on velocity threshold
        if (intensity > 0.1f && !swingAudioSource.isPlaying)
        {
            swingAudioSource.Play();
        }
        else if (intensity <= 0.05f && swingAudioSource.isPlaying)
        {
            swingAudioSource.Stop();
        }
    }

    /// <summary>
    /// Validate component setup and log warnings
    /// </summary>
    private void ValidateSetup()
    {
        if (coreFireParticles == null)
        {
            Debug.LogWarning("FireKatanaController: No core fire particle system assigned!", this);
        }
        else
        {
            var main = coreFireParticles.main;
            if (main.simulationSpace != ParticleSystemSimulationSpace.Local)
            {
                Debug.LogWarning("FireKatanaController: Core particles should use Local simulation space!", this);
            }
        }

        if (trailFireParticles == null)
        {
            Debug.LogWarning("FireKatanaController: No trail fire particle system assigned!", this);
        }
        else
        {
            var main = trailFireParticles.main;
            if (main.simulationSpace != ParticleSystemSimulationSpace.World)
            {
                Debug.LogWarning("FireKatanaController: Trail particles MUST use World simulation space!", this);
            }
        }

        if (swingAudioSource == null)
        {
            Debug.LogWarning("FireKatanaController: No swing audio source assigned!", this);
        }
    }

    /// <summary>
    /// Get current velocity (useful for debugging)
    /// </summary>
    public float GetCurrentVelocity()
    {
        return currentVelocity;
    }

    /// <summary>
    /// Get normalized intensity (0-1)
    /// </summary>
    public float GetIntensity()
    {
        return Mathf.Clamp01(currentVelocity / maxVelocity);
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw velocity vector
        Gizmos.color = Color.red;
        Vector3 velocityDirection = (transform.position - lastPosition).normalized;
        Gizmos.DrawRay(transform.position, velocityDirection * currentVelocity);

        // Draw intensity sphere (size represents current intensity)
        float intensity = Mathf.Clamp01(currentVelocity / maxVelocity);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange with transparency
        Gizmos.DrawWireSphere(transform.position, 0.1f + intensity * 0.2f);
    }
}
