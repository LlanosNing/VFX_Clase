using UnityEngine;

public enum CameraShakePreset
{
    Custom,
    ErraticThunder,
    LightVibration,
    ViolentTremor,
    SmoothImpact
}

[RequireComponent(typeof(ParticleSystem))]
public class IndividualCameraShake : MonoBehaviour
{
    [Header("CAMERA SHAKE PRESET")]
    [Tooltip("Elige qué tipo de temblor de cámara provocará este objeto al emitir partículas.")]
    public CameraShakePreset preset = CameraShakePreset.ErraticThunder;

    [Header("CUSTOM SETTINGS (Only used if Custom)")]
    public float duration = 0.5f;
    public float positionMagnitude = 0.3f;
    public float rotationMagnitude = 2.0f;
    public float frequency = 30f;

    private ParticleSystem _particleSystem;
    private bool _hasTriggered = false;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        // Resetea el estado por si el objeto se apaga y se vuelve a encender (Object Pooling)
        _hasTriggered = false;
    }

    private void Update()
    {
        // Detecta el momento exacto en el que el sistema de partículas empieza a reproducirse
        if (_particleSystem.isPlaying && !_hasTriggered)
        {
            TriggerCameraShake();
            _hasTriggered = true; // Evita que envíe la orden en cada frame
        }
        else if (!_particleSystem.isPlaying && _hasTriggered)
        {
            // Se resetea cuando las partículas terminan, listo para la siguiente vez que se reproduzca
            _hasTriggered = false;
        }
    }

    private void TriggerCameraShake()
    {
        if (CameraShake.Instance == null)
        {
            Debug.LogWarning("No se ejecutó el temblor: No hay ningún script CameraShake en la escena.");
            return;
        }

        switch (preset)
        {
            case CameraShakePreset.ErraticThunder:
                CameraShake.Instance.Shake(0.4f, 0.2f, 3.0f, 45f);
                break;

            case CameraShakePreset.ViolentTremor:
                CameraShake.Instance.Shake(1.2f, 0.6f, 2.0f, 25f);
                break;

            case CameraShakePreset.LightVibration:
                CameraShake.Instance.Shake(0.2f, 0.05f, 0.5f, 35f);
                break;

            case CameraShakePreset.SmoothImpact:
                CameraShake.Instance.Shake(0.8f, 0.4f, 1.0f, 10f);
                break;

            case CameraShakePreset.Custom:
                CameraShake.Instance.Shake(duration, positionMagnitude, rotationMagnitude, frequency);
                break;
        }
    }
}