using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum ShakeType
{
    LightImpact,
    HeavyExplosion,
    SubtleRumble,
    Custom
}

[System.Serializable]
public class ShakeProfile
{
    public string profileName = "New Shake Profile";
    public ShakeType shakeType;

    [Header("Values")]
    public float duration = 0.4f;
    public float positionMagnitude = 0.3f;
    public float rotationMagnitude = 2.0f;
    public float frequency = 25.0f;
}

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Referencia")]
    [Tooltip("Transform de la cámara que vibrará. Si está vacío usa Camera.main")]
    public Transform cameraTransform;

    [Header("Perfiles Predefinidos (Presets)")]
    public List<ShakeProfile> shakeProfiles = new List<ShakeProfile>()
    {
        new ShakeProfile { profileName = "Light Impact", shakeType = ShakeType.LightImpact, duration = 0.2f, positionMagnitude = 0.15f, rotationMagnitude = 1f, frequency = 30f },
        new ShakeProfile { profileName = "Heavy Explosion", shakeType = ShakeType.HeavyExplosion, duration = 0.5f, positionMagnitude = 0.4f, rotationMagnitude = 3f, frequency = 20f },
        new ShakeProfile { profileName = "Subtle Rumble", shakeType = ShakeType.SubtleRumble, duration = 0.8f, positionMagnitude = 0.08f, rotationMagnitude = 0.5f, frequency = 15f }
    };

    [Header("Valores por defecto")]
    [Tooltip("Duración del shake en segundos")]
    public float duration = 0.5f;

    [Tooltip("Desplazamiento máximo en unidades")]
    public float positionMagnitude = 0.3f;

    [Tooltip("Rotación máxima en grados")]
    public float rotationMagnitude = 2f;

    [Tooltip("Velocidad del temblor (más alto = más rápido)")]
    public float frequency = 25f;

    [Tooltip("Intensidad a lo largo del shake (x: 0-1 tiempo, y: 0-1 intensidad)")]
    public AnimationCurve falloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [Header("Ejes")]
    public bool shakePosition = true;
    public bool shakeRotation = true;
    public Vector3 positionAxes = new Vector3(1f, 1f, 0f);
    public Vector3 rotationAxes = new Vector3(0.5f, 0.5f, 1f);

    [Header("Otros")]
    [Tooltip("Si está activo, el shake sigue funcionando con Time.timeScale = 0")]
    public bool useUnscaledTime = false;

    [Header("Eventos de Partícula")]
    [Tooltip("Evento visual para arrastrar y soltar la función que quieras ejecutar cuando las partículas entren en el Trigger")]
    public UnityEvent onParticleTriggerEvent;

    private bool _shaking;
    private float _elapsed;
    private float _seed;

    // Valores activos del shake en curso
    private float _activeDuration;
    private float _activePosMagnitude;
    private float _activeRotMagnitude;
    private float _activeFrequency;

    // Último offset aplicado, para poder retirarlo cada frame
    private Vector3 _lastPosOffset;
    private Quaternion _lastRotOffset = Quaternion.identity;

    // Lista auxiliar para almacenar las partículas del Trigger
    private ParticleSystem _ps;
    private List<ParticleSystem.Particle> _enterParticles = new List<ParticleSystem.Particle>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (cameraTransform == null)
        {
            Camera cam = Camera.main;
            cameraTransform = cam != null ? cam.transform : transform;
        }

        _ps = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Callback automático de Unity cuando el módulo Trigger del Particle System está en modo Callback.
    /// </summary>
    private void OnParticleTrigger()
    {
        if (_ps != null)
        {
            int numEnter = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _enterParticles);
            if (numEnter > 0)
            {
                onParticleTriggerEvent?.Invoke();
            }
        }
        else
        {
            onParticleTriggerEvent?.Invoke();
        }
    }

    /// <summary>Activa un shake usando el enum Preset seleccionado.</summary>
    public void TriggerPresetShake(ShakeType type)
    {
        ShakeProfile profile = shakeProfiles.Find(p => p.shakeType == type);
        if (profile != null)
        {
            Shake(profile.duration, profile.positionMagnitude, profile.rotationMagnitude, profile.frequency);
        }
        else
        {
            Debug.LogWarning($"Shake profile for {type} not found in CameraShake.");
        }
    }

    /// <summary>Shake con los valores configurados en el inspector.</summary>
    [ContextMenu("Test Shake")]
    public void Shake()
    {
        Shake(duration, positionMagnitude, rotationMagnitude, frequency);
    }

    /// <summary>Shake con duración y magnitud personalizadas (la magnitud escala posición y rotación).</summary>
    public void Shake(float customDuration, float magnitudeMultiplier)
    {
        Shake(customDuration, positionMagnitude * magnitudeMultiplier, rotationMagnitude * magnitudeMultiplier, frequency);
    }

    /// <summary>Shake con todos los parámetros personalizados.</summary>
    public void Shake(float customDuration, float posMagnitude, float rotMagnitude, float customFrequency)
    {
        _activeDuration = Mathf.Max(0.01f, customDuration);
        _activePosMagnitude = posMagnitude;
        _activeRotMagnitude = rotMagnitude;
        _activeFrequency = customFrequency;

        _elapsed = 0f;
        _seed = Random.Range(0f, 1000f);
        _shaking = true;
    }

    /// <summary>Detiene el shake inmediatamente.</summary>
    public void Stop()
    {
        _shaking = false;
        RemoveLastOffset();
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        RemoveLastOffset();

        if (!_shaking) return;

        _elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float t = _elapsed / _activeDuration;

        if (t >= 1f)
        {
            _shaking = false;
            return;
        }

        float strength = falloff.Evaluate(t);
        float time = _elapsed * _activeFrequency;

        if (shakePosition)
        {
            Vector3 noise = new Vector3(Noise(_seed, time), Noise(_seed + 10f, time), Noise(_seed + 20f, time));
            _lastPosOffset = Vector3.Scale(noise, positionAxes) * (_activePosMagnitude * strength);
            cameraTransform.localPosition += _lastPosOffset;
        }

        if (shakeRotation)
        {
            Vector3 noise = new Vector3(Noise(_seed + 30f, time), Noise(_seed + 40f, time), Noise(_seed + 50f, time));
            Vector3 euler = Vector3.Scale(noise, rotationAxes) * (_activeRotMagnitude * strength);
            _lastRotOffset = Quaternion.Euler(euler);
            cameraTransform.localRotation *= _lastRotOffset;
        }
    }

    private void OnDisable()
    {
        _shaking = false;
        RemoveLastOffset();
    }

    private void RemoveLastOffset()
    {
        if (cameraTransform == null) return;

        cameraTransform.localPosition -= _lastPosOffset;
        cameraTransform.localRotation *= Quaternion.Inverse(_lastRotOffset);
        _lastPosOffset = Vector3.zero;
        _lastRotOffset = Quaternion.identity;
    }

    private static float Noise(float seed, float time)
    {
        return Mathf.PerlinNoise(seed, time) * 2f - 1f;
    }
}