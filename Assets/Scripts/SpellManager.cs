using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
public enum ProjectileMovementType
{
    StraightLine,
    PathThenTarget
}
public struct CameraShakeConfig
{
    public bool useCameraShake;
    public ShakeType selectedShakeType;
    public float timeOffset;

    public float durationMultiplier;
    public float intensityMultiplier;
    public float frequencyMultiplier;

    public float customShakeDuration;
    public float customPositionMagnitude;
    public float customRotationMagnitude;
    public float customShakeFrequency;

    public bool overrideAdvancedOptions;
    public AnimationCurve falloff;
    public bool shakePosition;
    public bool shakeRotation;
    public Vector3 positionAxes;
    public Vector3 rotationAxes;
    public bool useUnscaledTime;
}

[System.Serializable]
public class SpellConfig
{
    public string spellName = "SpellName";

    [Header("1 REFERENCES AND CONTROLS")]
    public Key triggerKey = Key.Digit1;
    public Transform caster;
    public Transform target;
    public Animator casterAnimator;
    public Animator targetAnimator;

    [Header("2 PREPARATION VFX AND TIMING")]
    public List<GameObject> prepVfxPrefabs = new List<GameObject>();
    public List<float> prepVfxPrefabDelays = new List<float>();
    public float prepVfxDelay = 0.0f;

    [Header("2 PREPARATION- SHAKE")]
    public bool prepShakeEnabled = false;
    public ShakeType prepShakeType = ShakeType.HeavyExplosion;
    public float prepShakeTimeOffset = 0f;
    public float prepShakeDurationMult = 1f;
    public float prepShakeIntensityMult = 1f;
    public float prepShakeFrequencyMult = 1f;

    [Header("2 PREPARATION - CUSTOM SHAKE (Custom type only)")]
    public float prepShakeCustomDuration = 0.4f;
    public float prepShakeCustomPosition = 0.3f;
    public float prepShakeCustomRotation = 2.0f;
    public float prepShakeCustomFrequency = 25.0f;

    [Header("2 PREPARATION - ADVANCED SHAKE")]
    public bool prepShakeOverrideAdvanced = false;
    public AnimationCurve prepShakeFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public bool prepShakePosition = true;
    public bool prepShakeRotation = true;
    public Vector3 prepShakePositionAxes = new Vector3(1f, 1f, 0f);
    public Vector3 prepShakeRotationAxes = new Vector3(0.5f, 0.5f, 1f);
    public bool prepShakeUnscaledTime = false;

    [Header("3 CAST PROJECTILES AND TIMING")]
    public List<GameObject> projectilePrefabs = new List<GameObject>();
    public List<float> projectilePrefabDelays = new List<float>();
    public Transform castLocation;
    [FormerlySerializedAs("projectileCastDelay")] public float castDelay = 0.8f;

    [Header("3 CAST - SHAKE")]
    public bool castShakeEnabled = false;
    public ShakeType castShakeType = ShakeType.HeavyExplosion;
    public float castShakeTimeOffset = 0f;
    public float castShakeDurationMult = 1f;
    public float castShakeIntensityMult = 1f;
    public float castShakeFrequencyMult = 1f;

    [Header("3 CAST - CUSTOM SHAKE (Custom type only)")]
    public float castShakeCustomDuration = 0.4f;
    public float castShakeCustomPosition = 0.3f;
    public float castShakeCustomRotation = 2.0f;
    public float castShakeCustomFrequency = 25.0f;

    [Header("3 CAST - ADVANCED SHAKE")]
    public bool castShakeOverrideAdvanced = false;
    public AnimationCurve castShakeFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public bool castShakePosition = true;
    public bool castShakeRotation = true;
    public Vector3 castShakePositionAxes = new Vector3(1f, 1f, 0f);
    public Vector3 castShakeRotationAxes = new Vector3(0.5f, 0.5f, 1f);
    public bool castShakeUnscaledTime = false;

    [Header("4 FLIGHT - MOVEMENT")]
    public bool useProjectile = true;
    public ProjectileMovementType movementType = ProjectileMovementType.PathThenTarget;
    public PathCreator pathPrefab;
    public bool reversePath = false;
    public float pathSpeed = 10f;
    public float projectileSpeed = 15f;

    [Header("5 FLIGHT - TRAJECTORY NOISE")]
    public bool useNoise = false;
    public float noiseFrequency = 5.0f;
    public float noiseMagnitude = 0.5f;

    [Header("6 FLIGHT - FLOOR LIMIT")]
    public bool clampMinimumY = true;
    public float minimumYValue = 0f;

    [Header("7 IMPACT - VFX AND TIMING")]
    public List<GameObject> impactExplosionPrefabs = new List<GameObject>();
    public List<float> impactExplosionPrefabDelays = new List<float>();
    public Transform impactLocation;
    public float summonImpactDelay = 0.5f;
    public float destroyImpactVfxAfter = 4f;

    [Header("7 IMPACT - SHAKE")]
    public bool impactShakeEnabled = true;
    public ShakeType impactShakeType = ShakeType.HeavyExplosion;
    public float impactShakeTimeOffset = 0f;
    public float impactShakeDurationMult = 1f;
    public float impactShakeIntensityMult = 1f;
    public float impactShakeFrequencyMult = 1f;

    [Header("7 IMPACT - CUSTOM SHAKE (Custom type only)")]
    public float impactShakeCustomDuration = 0.4f;
    public float impactShakeCustomPosition = 0.3f;
    public float impactShakeCustomRotation = 2.0f;
    public float impactShakeCustomFrequency = 25.0f;

    [Header("7 IMPACT - ADVANCED SHAKE")]
    public bool impactShakeOverrideAdvanced = false;
    public AnimationCurve impactShakeFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public bool impactShakePosition = true;
    public bool impactShakeRotation = true;
    public Vector3 impactShakePositionAxes = new Vector3(1f, 1f, 0f);
    public Vector3 impactShakeRotationAxes = new Vector3(0.5f, 0.5f, 1f);
    public bool impactShakeUnscaledTime = false;

    [Header("8 ANIMATIONS - TRIGGERS")]
    public string casterAnimTrigger = "TriggerName";
    public float casterAnimDelay = 0f;
    public string targetDeathAnimTrigger = "TriggerName";
    public float targetAnimDelay = 0f;

    [Header("9 COOLDOWN")]
    public float cooldownAfterImpact = 1.5f;

    public CameraShakeConfig GetPrepShake()
    {
        return new CameraShakeConfig
        {
            useCameraShake = prepShakeEnabled,
            selectedShakeType = prepShakeType,
            timeOffset = prepShakeTimeOffset,
            durationMultiplier = prepShakeDurationMult,
            intensityMultiplier = prepShakeIntensityMult,
            frequencyMultiplier = prepShakeFrequencyMult,
            customShakeDuration = prepShakeCustomDuration,
            customPositionMagnitude = prepShakeCustomPosition,
            customRotationMagnitude = prepShakeCustomRotation,
            customShakeFrequency = prepShakeCustomFrequency,
            overrideAdvancedOptions = prepShakeOverrideAdvanced,
            falloff = prepShakeFalloff,
            shakePosition = prepShakePosition,
            shakeRotation = prepShakeRotation,
            positionAxes = prepShakePositionAxes,
            rotationAxes = prepShakeRotationAxes,
            useUnscaledTime = prepShakeUnscaledTime
        };
    }

    public CameraShakeConfig GetCastShake()
    {
        return new CameraShakeConfig
        {
            useCameraShake = castShakeEnabled,
            selectedShakeType = castShakeType,
            timeOffset = castShakeTimeOffset,
            durationMultiplier = castShakeDurationMult,
            intensityMultiplier = castShakeIntensityMult,
            frequencyMultiplier = castShakeFrequencyMult,
            customShakeDuration = castShakeCustomDuration,
            customPositionMagnitude = castShakeCustomPosition,
            customRotationMagnitude = castShakeCustomRotation,
            customShakeFrequency = castShakeCustomFrequency,
            overrideAdvancedOptions = castShakeOverrideAdvanced,
            falloff = castShakeFalloff,
            shakePosition = castShakePosition,
            shakeRotation = castShakeRotation,
            positionAxes = castShakePositionAxes,
            rotationAxes = castShakeRotationAxes,
            useUnscaledTime = castShakeUnscaledTime
        };
    }

    public CameraShakeConfig GetImpactShake()
    {
        return new CameraShakeConfig
        {
            useCameraShake = impactShakeEnabled,
            selectedShakeType = impactShakeType,
            timeOffset = impactShakeTimeOffset,
            durationMultiplier = impactShakeDurationMult,
            intensityMultiplier = impactShakeIntensityMult,
            frequencyMultiplier = impactShakeFrequencyMult,
            customShakeDuration = impactShakeCustomDuration,
            customPositionMagnitude = impactShakeCustomPosition,
            customRotationMagnitude = impactShakeCustomRotation,
            customShakeFrequency = impactShakeCustomFrequency,
            overrideAdvancedOptions = impactShakeOverrideAdvanced,
            falloff = impactShakeFalloff,
            shakePosition = impactShakePosition,
            shakeRotation = impactShakeRotation,
            positionAxes = impactShakePositionAxes,
            rotationAxes = impactShakeRotationAxes,
            useUnscaledTime = impactShakeUnscaledTime
        };
    }
}
public class SpellManager : MonoBehaviour
{
    public List<SpellConfig> spells = new List<SpellConfig>();
    private bool _isCasting = false;

    private readonly List<GameObject> _spawnedProjectiles = new List<GameObject>();
    private static readonly List<VisualEffect> _vfxBuffer = new List<VisualEffect>();
    private static readonly List<ParticleSystem> _psBuffer = new List<ParticleSystem>();

    private const float ArrivalDistanceSqr = 0.3f * 0.3f;

    private struct ShakeDefaults
    {
        public AnimationCurve falloff;
        public bool shakePosition;
        public bool shakeRotation;
        public Vector3 positionAxes;
        public Vector3 rotationAxes;
        public bool useUnscaledTime;
    }
    private ShakeDefaults _shakeDefaults;
    private bool _shakeDefaultsCached = false;

    void Update()
    {
        if (_isCasting) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        for (int i = 0; i < spells.Count; i++)
        {
            SpellConfig spell = spells[i];
            if (keyboard[spell.triggerKey].wasPressedThisFrame)
            {
                StartCoroutine(ExecuteSpellSequence(spell));
                return;
            }
        }
    }

    private void OnDisable()
    {
        _isCasting = false;
        _spawnedProjectiles.Clear();
    }

    private IEnumerator ExecuteSpellSequence(SpellConfig spell)
    {
        _isCasting = true;

        if (spell.casterAnimator != null && !string.IsNullOrEmpty(spell.casterAnimTrigger))
        {
            spell.casterAnimator.ResetTrigger(spell.casterAnimTrigger);
        }
        if (spell.targetAnimator != null && !string.IsNullOrEmpty(spell.targetDeathAnimTrigger))
        {
            spell.targetAnimator.ResetTrigger(spell.targetDeathAnimTrigger);
        }

        StartCoroutine(TriggerAnimationWithDelay(spell.casterAnimator, spell.casterAnimTrigger, spell.casterAnimDelay));

        CameraShakeConfig prepShake = spell.GetPrepShake();
        CameraShakeConfig castShake = spell.GetCastShake();
        CameraShakeConfig impactShake = spell.GetImpactShake();

        float prepTime = Mathf.Max(0f, spell.prepVfxDelay);
        float castTime = Mathf.Max(prepTime, spell.castDelay);
        ScheduleShake(prepShake, prepTime);
        ScheduleShake(castShake, castTime);
        if (!spell.useProjectile) ScheduleShake(impactShake, castTime + Mathf.Max(0f, spell.summonImpactDelay));
        bool impactShakeFired = false;

        if (spell.prepVfxDelay > 0) yield return new WaitForSeconds(spell.prepVfxDelay);

        if (spell.prepVfxPrefabs != null)
        {
            for (int i = 0; i < spell.prepVfxPrefabs.Count; i++)
            {
                GameObject prepObj = spell.prepVfxPrefabs[i];
                if (prepObj == null) continue;
                PlayParticlesWithDelay(prepObj, GetPrefabDelay(spell.prepVfxPrefabDelays, i), false);
            }
        }

        float remainingWait = spell.castDelay - spell.prepVfxDelay;
        if (remainingWait > 0) yield return new WaitForSeconds(remainingWait);

        Vector3 targetImpactPos = GetImpactPosition(spell);

        if (spell.useProjectile)
        {
            if (spell.projectilePrefabs != null && spell.projectilePrefabs.Count > 0 && spell.castLocation != null)
            {
                _spawnedProjectiles.Clear();

                for (int i = 0; i < spell.projectilePrefabs.Count; i++)
                {
                    GameObject projPrefab = spell.projectilePrefabs[i];
                    if (projPrefab == null) continue;
                    GameObject projectile = Instantiate(projPrefab, spell.castLocation.position, spell.castLocation.rotation);
                    PlayParticlesWithDelay(projectile, GetPrefabDelay(spell.projectilePrefabDelays, i), true);
                    _spawnedProjectiles.Add(projectile);
                }

                if (_spawnedProjectiles.Count > 0)
                {
                    float seed = Random.Range(0f, 100f);

                    if (spell.movementType == ProjectileMovementType.PathThenTarget && spell.pathPrefab != null)
                    {
                        PathCreator pathInstance = Instantiate(spell.pathPrefab, spell.castLocation.position, spell.castLocation.rotation);
                        VertexPath path = pathInstance.path;
                        float distanceTravelled = 0f;

                        while (AnyProjectileAlive() && distanceTravelled < path.length)
                        {
                            distanceTravelled += spell.pathSpeed * Time.deltaTime;

                            float currentDistance = spell.reversePath ? (path.length - distanceTravelled) : distanceTravelled;

                            Vector3 pathPos = path.GetPointAtDistance(currentDistance, EndOfPathInstruction.Stop);
                            Quaternion pathRot = path.GetRotationAtDistance(currentDistance, EndOfPathInstruction.Stop);

                            MoveProjectiles(spell, pathPos + CalculateNoiseOffset(spell, seed), pathRot, true);
                            yield return null;
                        }

                        if (pathInstance != null) Destroy(pathInstance.gameObject);
                    }

                    Vector3 logicalPosition = _spawnedProjectiles[0] != null ? _spawnedProjectiles[0].transform.position : spell.castLocation.position;

                    while (AnyProjectileAlive() && (logicalPosition - targetImpactPos).sqrMagnitude > ArrivalDistanceSqr)
                    {
                        targetImpactPos = GetImpactPosition(spell);

                        logicalPosition = Vector3.MoveTowards(logicalPosition, targetImpactPos, spell.projectileSpeed * Time.deltaTime);

                        if (!impactShakeFired && impactShake.useCameraShake && impactShake.timeOffset < 0f && spell.projectileSpeed > 0f &&
                            Vector3.Distance(logicalPosition, targetImpactPos) / spell.projectileSpeed <= -impactShake.timeOffset)
                        {
                            TriggerCameraShake(impactShake);
                            impactShakeFired = true;
                        }

                        Vector3 finalPos = logicalPosition + CalculateNoiseOffset(spell, seed);

                        Vector3 dir = targetImpactPos - finalPos;
                        bool hasDir = dir.sqrMagnitude > 0.0001f;

                        MoveProjectiles(spell, finalPos, hasDir ? Quaternion.LookRotation(dir) : Quaternion.identity, hasDir);
                        yield return null;
                    }

                    foreach (GameObject projectile in _spawnedProjectiles)
                    {
                        if (projectile != null) Destroy(projectile);
                    }
                    _spawnedProjectiles.Clear();
                }
            }
            else
            {
                Debug.LogWarning($"Spell '{spell.spellName}': useProjectile is enabled but projectile prefabs or castLocation are missing. Impact happens immediately.");
            }
        }
        else if (spell.summonImpactDelay > 0f)
        {
            yield return new WaitForSeconds(spell.summonImpactDelay);
            targetImpactPos = GetImpactPosition(spell);
        }

        if (spell.useProjectile && !impactShakeFired) ScheduleShake(impactShake, 0f);

        if (spell.impactExplosionPrefabs != null && spell.impactExplosionPrefabs.Count > 0)
        {
            Transform originalRef = spell.impactLocation != null ? spell.impactLocation : spell.target;

            Transform parentRef = !spell.useProjectile ? originalRef : null;

            for (int i = 0; i < spell.impactExplosionPrefabs.Count; i++)
            {
                GameObject impactPrefab = spell.impactExplosionPrefabs[i];
                if (impactPrefab == null) continue;

                float delay = GetPrefabDelay(spell.impactExplosionPrefabDelays, i);
                if (delay <= 0f) SpawnImpactVfx(impactPrefab, parentRef, targetImpactPos, spell.destroyImpactVfxAfter);
                else StartCoroutine(SpawnImpactVfxAfterDelay(impactPrefab, parentRef, targetImpactPos, spell.destroyImpactVfxAfter, delay));
            }
        }

        StartCoroutine(TriggerAnimationWithDelay(spell.targetAnimator, spell.targetDeathAnimTrigger, spell.targetAnimDelay));

        yield return new WaitForSeconds(spell.cooldownAfterImpact);
        _isCasting = false;
    }

    private IEnumerator TriggerAnimationWithDelay(Animator anim, string triggerName, float delay)
    {
        if (anim == null || string.IsNullOrEmpty(triggerName)) yield break;

        if (delay > 0) yield return new WaitForSeconds(delay);

        anim.SetTrigger(triggerName);
    }

    private Vector3 GetImpactPosition(SpellConfig spell)
    {
        if (spell.impactLocation != null) return spell.impactLocation.position;
        if (spell.target != null) return spell.target.position;
        return Vector3.zero;
    }

    private bool AnyProjectileAlive()
    {
        for (int i = 0; i < _spawnedProjectiles.Count; i++)
        {
            if (_spawnedProjectiles[i] != null) return true;
        }
        return false;
    }

    private void MoveProjectiles(SpellConfig spell, Vector3 finalPos, Quaternion rotation, bool applyRotation)
    {
        if (spell.clampMinimumY && finalPos.y < spell.minimumYValue)
        {
            finalPos.y = spell.minimumYValue;
        }

        for (int i = 0; i < _spawnedProjectiles.Count; i++)
        {
            GameObject projectile = _spawnedProjectiles[i];
            if (projectile == null) continue;

            if (applyRotation) projectile.transform.SetPositionAndRotation(finalPos, rotation);
            else projectile.transform.position = finalPos;
        }
    }

    private Vector3 CalculateNoiseOffset(SpellConfig spell, float seed)
    {
        if (!spell.useNoise) return Vector3.zero;

        float time = Time.time * spell.noiseFrequency;
        Vector3 noiseOffset = Vector3.zero;

        noiseOffset.x = (Mathf.PerlinNoise(seed, time) - 0.5f) * 2f * spell.noiseMagnitude;
        noiseOffset.y = (Mathf.PerlinNoise(seed + 10f, time) - 0.5f) * 2f * spell.noiseMagnitude;
        noiseOffset.z = (Mathf.PerlinNoise(seed + 20f, time) - 0.5f) * 2f * spell.noiseMagnitude;

        return noiseOffset;
    }

    private void TriggerCameraShake(CameraShakeConfig config)
    {
        CameraShake cam = CameraShake.Instance;
        if (!config.useCameraShake || cam == null) return;

        float duration, posMagnitude, rotMagnitude, frequency;

        if (config.selectedShakeType == ShakeType.Custom)
        {
            duration = config.customShakeDuration;
            posMagnitude = config.customPositionMagnitude;
            rotMagnitude = config.customRotationMagnitude;
            frequency = config.customShakeFrequency;
        }
        else
        {
            ShakeProfile profile = FindShakeProfile(cam, config.selectedShakeType);
            if (profile == null)
            {
                Debug.LogWarning($"Shake profile for {config.selectedShakeType} not found in CameraShake.");
                return;
            }

            duration = profile.duration;
            posMagnitude = profile.positionMagnitude;
            rotMagnitude = profile.rotationMagnitude;
            frequency = profile.frequency;
        }

        ApplyAdvancedOptions(cam, config);

        cam.Shake(
            duration * config.durationMultiplier,
            posMagnitude * config.intensityMultiplier,
            rotMagnitude * config.intensityMultiplier,
            frequency * config.frequencyMultiplier
        );
    }

    private ShakeProfile FindShakeProfile(CameraShake cam, ShakeType type)
    {
        List<ShakeProfile> profiles = cam.shakeProfiles;
        for (int i = 0; i < profiles.Count; i++)
        {
            if (profiles[i].shakeType == type) return profiles[i];
        }
        return null;
    }

    private void ApplyAdvancedOptions(CameraShake cam, CameraShakeConfig config)
    {
        if (!_shakeDefaultsCached)
        {
            _shakeDefaults = new ShakeDefaults
            {
                falloff = cam.falloff,
                shakePosition = cam.shakePosition,
                shakeRotation = cam.shakeRotation,
                positionAxes = cam.positionAxes,
                rotationAxes = cam.rotationAxes,
                useUnscaledTime = cam.useUnscaledTime
            };
            _shakeDefaultsCached = true;
        }

        if (config.overrideAdvancedOptions)
        {
            cam.falloff = config.falloff ?? _shakeDefaults.falloff;
            cam.shakePosition = config.shakePosition;
            cam.shakeRotation = config.shakeRotation;
            cam.positionAxes = config.positionAxes;
            cam.rotationAxes = config.rotationAxes;
            cam.useUnscaledTime = config.useUnscaledTime;
        }
        else
        {
            cam.falloff = _shakeDefaults.falloff;
            cam.shakePosition = _shakeDefaults.shakePosition;
            cam.shakeRotation = _shakeDefaults.shakeRotation;
            cam.positionAxes = _shakeDefaults.positionAxes;
            cam.rotationAxes = _shakeDefaults.rotationAxes;
            cam.useUnscaledTime = _shakeDefaults.useUnscaledTime;
        }
    }

    private void ScheduleShake(CameraShakeConfig config, float phaseTime)
    {
        if (!config.useCameraShake) return;

        float delay = phaseTime + config.timeOffset;
        if (delay <= 0f) TriggerCameraShake(config);
        else StartCoroutine(ShakeAfterDelay(config, delay));
    }

    private IEnumerator ShakeAfterDelay(CameraShakeConfig config, float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerCameraShake(config);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < spells.Count; i++)
        {
            SpellConfig spell = spells[i];
            if (spell == null) continue;

            SyncDelays(spell.prepVfxPrefabs, ref spell.prepVfxPrefabDelays);
            SyncDelays(spell.projectilePrefabs, ref spell.projectilePrefabDelays);
            SyncDelays(spell.impactExplosionPrefabs, ref spell.impactExplosionPrefabDelays);
        }
    }

    private static void SyncDelays(List<GameObject> prefabs, ref List<float> delays)
    {
        if (prefabs == null) return;
        if (delays == null) delays = new List<float>();

        while (delays.Count < prefabs.Count) delays.Add(0f);
        if (delays.Count > prefabs.Count) delays.RemoveRange(prefabs.Count, delays.Count - prefabs.Count);
    }
#endif

    private static float GetPrefabDelay(List<float> delays, int index)
    {
        if (delays == null || index >= delays.Count) return 0f;
        return Mathf.Max(0f, delays[index]);
    }

    private void PlayParticlesWithDelay(GameObject vfxInstance, float delay, bool hideUntilPlay)
    {
        if (delay <= 0f)
        {
            PlayParticles(vfxInstance);
            return;
        }

        if (hideUntilPlay) vfxInstance.SetActive(false);
        StartCoroutine(PlayParticlesAfterDelay(vfxInstance, delay));
    }

    private IEnumerator PlayParticlesAfterDelay(GameObject vfxInstance, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (vfxInstance != null) PlayParticles(vfxInstance);
    }

    private void SpawnImpactVfx(GameObject prefab, Transform parentRef, Vector3 position, float destroyAfter)
    {
        GameObject explosion;

        if (parentRef != null)
        {
            explosion = Instantiate(prefab, parentRef);
            explosion.transform.localPosition = Vector3.zero;
            explosion.transform.localRotation = prefab.transform.rotation;
        }
        else
        {
            explosion = Instantiate(prefab, position, prefab.transform.rotation);
        }

        PlayParticles(explosion);
        Destroy(explosion, destroyAfter);
    }

    private IEnumerator SpawnImpactVfxAfterDelay(GameObject prefab, Transform parentRef, Vector3 position, float destroyAfter, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnImpactVfx(prefab, parentRef, position, destroyAfter);
    }

    private void PlayParticles(GameObject vfxInstance)
    {
        vfxInstance.SetActive(true);

        vfxInstance.GetComponentsInChildren(true, _vfxBuffer);
        for (int i = 0; i < _vfxBuffer.Count; i++)
        {
            VisualEffect vfx = _vfxBuffer[i];
            vfx.gameObject.SetActive(true);
            vfx.SendEvent("OnPlay");
        }

        vfxInstance.GetComponentsInChildren(true, _psBuffer);
        for (int i = 0; i < _psBuffer.Count; i++)
        {
            ParticleSystem ps = _psBuffer[i];
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }
}