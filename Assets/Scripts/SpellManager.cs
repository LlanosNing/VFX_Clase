using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;
using PathCreation;

public enum ProjectileMovementType
{
    StraightLine,
    PathThenTarget
}

[System.Serializable]
public class CameraShakeConfig
{
    public bool useCameraShake = false;
    public ShakeType selectedShakeType = ShakeType.HeavyExplosion;

    [Header("CUSTOM SHAKE SETTINGS")]
    public float customShakeDuration = 0.4f;
    public float customPositionMagnitude = 0.3f;
    public float customRotationMagnitude = 2.0f;
    public float customShakeFrequency = 25.0f;
}

[System.Serializable]
public class SpellConfig
{
    public string spellName = "Thunder Sequence";

    [Header("MAIN CONTROLS AND REFERENCES")]
    public Key triggerKey = Key.Digit1;
    public Transform caster;
    public Transform target;
    public Animator casterAnimator;
    public Animator targetAnimator;

    [Header("PREPARATION")]
    public List<GameObject> prepVfxPrefabs = new List<GameObject>();
    public float prepVfxDelay = 0.0f;
    public CameraShakeConfig prepCameraShake = new CameraShakeConfig();

    [Header("PROJECTILE CAST")]
    public List<GameObject> projectilePrefabs = new List<GameObject>();
    public Transform castLocation;
    public float projectileCastDelay = 0.8f;
    public CameraShakeConfig castCameraShake = new CameraShakeConfig();

    [Header("PROJECTILE MOVEMENT")]
    [Tooltip("Si se desactiva, el hechizo es una invocación (sin proyectil) y el impacto ocurre directamente sobre el objetivo.")]
    public bool useProjectile = true;
    public ProjectileMovementType movementType = ProjectileMovementType.PathThenTarget;
    public PathCreator pathPrefab;
    public bool reversePath = false;
    public float pathSpeed = 10f;
    public float projectileSpeed = 15f;

    [Header("TRAJECTORY NOISE")]
    public bool useNoise = false;
    public float noiseFrequency = 5.0f;
    public float noiseMagnitude = 0.5f;

    [Header("FLOOR RESTRICTION")]
    [Tooltip("Evita que el proyectil baje de un valor Y específico en el transform (ej: 0 para que no atraviese el suelo con el ruido).")]
    public bool clampMinimumY = true;
    public float minimumYValue = 0f;

    [Header("IMPACT AND DEATH")]
    public List<GameObject> impactExplosionPrefabs = new List<GameObject>();
    public Transform impactLocation;
    [Tooltip("Tiempo en segundos tras el cual se borrarán los clones de las explosiones para no dejar basura en la escena.")]
    public float destroyImpactVfxAfter = 4f;
    public CameraShakeConfig impactCameraShake = new CameraShakeConfig() { useCameraShake = true };

    [Header("ANIMATION TRIGGERS")]
    public string casterAnimTrigger = "CastSpell";
    public float casterAnimDelay = 0f;
    public string targetDeathAnimTrigger = "Die";
    public float targetAnimDelay = 0f;

    [Header("COOLDOWN")]
    public float cooldownAfterImpact = 1.5f;
}

public class SpellManager : MonoBehaviour
{
    public List<SpellConfig> spells = new List<SpellConfig>();
    private bool _isCasting = false;

    void Update()
    {
        if (Keyboard.current == null) return;

        foreach (SpellConfig spell in spells)
        {
            if (Keyboard.current[spell.triggerKey].wasPressedThisFrame)
            {
                if (!_isCasting)
                {
                    StartCoroutine(ExecuteSpellSequence(spell));
                }
                else
                {
                    Debug.Log($"System busy. Ignoring spell: {spell.spellName}");
                }
            }
        }
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

        if (spell.prepVfxDelay > 0) yield return new WaitForSeconds(spell.prepVfxDelay);

        TriggerCameraShake(spell.prepCameraShake);

        if (spell.prepVfxPrefabs != null && spell.prepVfxPrefabs.Count > 0)
        {
            foreach (GameObject prepObj in spell.prepVfxPrefabs)
            {
                if (prepObj == null) continue;
                PlayParticles(prepObj);
            }
        }

        float remainingWait = spell.projectileCastDelay - spell.prepVfxDelay;
        if (remainingWait > 0) yield return new WaitForSeconds(remainingWait);

        TriggerCameraShake(spell.castCameraShake);

        Vector3 targetImpactPos = spell.impactLocation != null ? spell.impactLocation.position : (spell.target != null ? spell.target.position : Vector3.zero);

        if (spell.useProjectile && spell.projectilePrefabs != null && spell.projectilePrefabs.Count > 0 && spell.castLocation != null)
        {
            List<GameObject> spawnedProjectiles = new List<GameObject>();

            foreach (GameObject projPrefab in spell.projectilePrefabs)
            {
                if (projPrefab == null) continue;
                GameObject projectile = Instantiate(projPrefab, spell.castLocation.position, spell.castLocation.rotation);
                PlayParticles(projectile);
                spawnedProjectiles.Add(projectile);
            }

            if (spawnedProjectiles.Count > 0)
            {
                float seed = Random.Range(0f, 100f);

                if (spell.movementType == ProjectileMovementType.PathThenTarget && spell.pathPrefab != null)
                {
                    PathCreator pathInstance = Instantiate(spell.pathPrefab, spell.castLocation.position, spell.castLocation.rotation);
                    VertexPath path = pathInstance.path;
                    float distanceTravelled = 0f;

                    while (spawnedProjectiles.Exists(p => p != null) && distanceTravelled < path.length)
                    {
                        distanceTravelled += spell.pathSpeed * Time.deltaTime;

                        float currentDistance = spell.reversePath ? (path.length - distanceTravelled) : distanceTravelled;

                        Vector3 pathPos = path.GetPointAtDistance(currentDistance, EndOfPathInstruction.Stop);
                        Quaternion pathRot = path.GetRotationAtDistance(currentDistance, EndOfPathInstruction.Stop);

                        Vector3 noiseOffset = CalculateNoiseOffset(spell, seed);
                        Vector3 finalPos = pathPos + noiseOffset;

                        if (spell.clampMinimumY && finalPos.y < spell.minimumYValue)
                        {
                            finalPos.y = spell.minimumYValue;
                        }

                        foreach (GameObject projectile in spawnedProjectiles)
                        {
                            if (projectile != null)
                            {
                                projectile.transform.position = finalPos;
                                projectile.transform.rotation = pathRot;
                            }
                        }
                        yield return null;
                    }

                    if (pathInstance != null) Destroy(pathInstance.gameObject);
                }

                Vector3 logicalPosition = spawnedProjectiles[0] != null ? spawnedProjectiles[0].transform.position : spell.castLocation.position;

                while (spawnedProjectiles.Exists(p => p != null) && Vector3.Distance(logicalPosition, targetImpactPos) > 0.3f)
                {
                    if (spell.impactLocation != null) targetImpactPos = spell.impactLocation.position;
                    else if (spell.target != null) targetImpactPos = spell.target.position;

                    logicalPosition = Vector3.MoveTowards(logicalPosition, targetImpactPos, spell.projectileSpeed * Time.deltaTime);

                    Vector3 noiseOffset = CalculateNoiseOffset(spell, seed);
                    Vector3 finalPos = logicalPosition + noiseOffset;

                    if (spell.clampMinimumY && finalPos.y < spell.minimumYValue)
                    {
                        finalPos.y = spell.minimumYValue;
                    }

                    foreach (GameObject projectile in spawnedProjectiles)
                    {
                        if (projectile != null)
                        {
                            projectile.transform.position = finalPos;
                            projectile.transform.LookAt(targetImpactPos);
                        }
                    }
                    yield return null;
                }

                foreach (GameObject projectile in spawnedProjectiles)
                {
                    if (projectile != null) Destroy(projectile);
                }
            }
        }

        TriggerCameraShake(spell.impactCameraShake);

        // FASE 3: IMPACTO
        if (spell.impactExplosionPrefabs != null && spell.impactExplosionPrefabs.Count > 0)
        {
            Transform originalRef = spell.impactLocation != null ? spell.impactLocation : spell.target;

            foreach (GameObject impactPrefab in spell.impactExplosionPrefabs)
            {
                if (impactPrefab == null) continue;

                GameObject explosion;

                if (spell.useProjectile)
                {
                    explosion = Instantiate(impactPrefab, targetImpactPos, impactPrefab.transform.rotation);
                }
                else
                {
                    if (originalRef != null)
                    {
                        // 1. Instanciamos emparentado al objetivo
                        explosion = Instantiate(impactPrefab, originalRef);
                        // 2. Forzamos su posición local a 0 para que aparezca exactamente en el centro del objetivo
                        explosion.transform.localPosition = Vector3.zero;
                        // 3. Mantenemos la rotación original del prefab
                        explosion.transform.localRotation = impactPrefab.transform.rotation;
                    }
                    else
                    {
                        explosion = Instantiate(impactPrefab, targetImpactPos, impactPrefab.transform.rotation);
                    }
                }

                PlayParticles(explosion);
                Destroy(explosion, spell.destroyImpactVfxAfter);
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
        if (config != null && config.useCameraShake && CameraShake.Instance != null)
        {
            if (config.selectedShakeType == ShakeType.Custom)
            {
                CameraShake.Instance.Shake(
                    config.customShakeDuration,
                    config.customPositionMagnitude,
                    config.customRotationMagnitude,
                    config.customShakeFrequency
                );
            }
            else
            {
                CameraShake.Instance.TriggerPresetShake(config.selectedShakeType);
            }
        }
    }

    private void PlayParticles(GameObject vfxInstance)
    {
        vfxInstance.SetActive(true);

        VisualEffect[] vfxGraphs = vfxInstance.GetComponentsInChildren<VisualEffect>(true);
        foreach (VisualEffect vfx in vfxGraphs)
        {
            vfx.gameObject.SetActive(true);
            vfx.SendEvent("OnPlay");
        }

        ParticleSystem[] particles = vfxInstance.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem ps in particles)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }
}