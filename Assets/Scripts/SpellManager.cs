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

    [Header("Custom Shake Settings")]
    public float customShakeDuration = 0.4f;
    public float customPositionMagnitude = 0.3f;
    public float customRotationMagnitude = 2.0f;
    public float customShakeFrequency = 25.0f;
}

[System.Serializable]
public class SpellConfig
{
    public string spellName = "Thunder Sequence";

    [Header("Main Controls and References")]
    public Key triggerKey = Key.Digit1;
    public Transform caster;
    public Transform target;
    public Animator casterAnimator;
    public Animator targetAnimator;

    [Header("Preparation")]
    public GameObject prepVfxPrefab;
    public Transform prepLocation;
    public float prepVfxDelay = 0.0f;
    public CameraShakeConfig prepCameraShake = new CameraShakeConfig();

    [Header("Projectile Cast")]
    public GameObject projectilePrefab;
    public Transform castLocation;
    public float projectileCastDelay = 0.8f;
    public float pathSpeed = 10f;
    public float projectileSpeed = 15f;
    public CameraShakeConfig castCameraShake = new CameraShakeConfig();

    [Header("Projectile Movement")]
    public ProjectileMovementType movementType = ProjectileMovementType.PathThenTarget;
    [Tooltip("Prefab del PathCreator que define la ruta inicial (ej. la órbita)")]
    public PathCreator pathPrefab;

    [Header("Impact and Death")]
    public GameObject impactExplosionPrefab;
    public Transform impactLocation;
    public float destroyImpactVfxAfter = 4f;
    public CameraShakeConfig impactCameraShake = new CameraShakeConfig() { useCameraShake = true };

    [Header("Animation Triggers")]
    public string casterAnimTrigger = "CastSpell";
    public string targetDeathAnimTrigger = "Die";

    [Header("Cooldown")]
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
            spell.casterAnimator.SetTrigger(spell.casterAnimTrigger);
        }

        if (spell.targetAnimator != null && !string.IsNullOrEmpty(spell.targetDeathAnimTrigger))
        {
            spell.targetAnimator.ResetTrigger(spell.targetDeathAnimTrigger);
        }

        if (spell.prepVfxDelay > 0) yield return new WaitForSeconds(spell.prepVfxDelay);

        TriggerCameraShake(spell.prepCameraShake);

        if (spell.prepVfxPrefab != null)
        {
            Vector3 prepPos = spell.prepLocation != null ? spell.prepLocation.position : spell.caster.position;
            Quaternion prepRot = spell.prepLocation != null ? spell.prepLocation.rotation : spell.caster.rotation;

            GameObject prepVfx = Instantiate(spell.prepVfxPrefab, prepPos, prepRot);
            PlayParticles(prepVfx);
            Destroy(prepVfx, 5f);
        }

        float remainingWait = spell.projectileCastDelay - spell.prepVfxDelay;
        if (remainingWait > 0) yield return new WaitForSeconds(remainingWait);

        TriggerCameraShake(spell.castCameraShake);

        Vector3 targetImpactPos = spell.impactLocation != null ? spell.impactLocation.position : (spell.target != null ? spell.target.position : Vector3.zero);

        if (spell.projectilePrefab != null && spell.castLocation != null)
        {
            GameObject projectile = Instantiate(spell.projectilePrefab, spell.castLocation.position, spell.castLocation.rotation);
            PlayParticles(projectile);

            if (spell.movementType == ProjectileMovementType.PathThenTarget && spell.pathPrefab != null)
            {
                PathCreator pathInstance = Instantiate(spell.pathPrefab, spell.castLocation.position, spell.castLocation.rotation);
                VertexPath path = pathInstance.path;
                float distanceTravelled = 0f;

                while (projectile != null && distanceTravelled < path.length)
                {
                    distanceTravelled += spell.pathSpeed * Time.deltaTime;
                    projectile.transform.position = path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
                    projectile.transform.rotation = path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
                    yield return null;
                }

                if (pathInstance != null) Destroy(pathInstance.gameObject);
            }

            while (projectile != null && Vector3.Distance(projectile.transform.position, targetImpactPos) > 0.3f)
            {
                if (spell.impactLocation != null) targetImpactPos = spell.impactLocation.position;
                else if (spell.target != null) targetImpactPos = spell.target.position;

                projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetImpactPos, spell.projectileSpeed * Time.deltaTime);
                projectile.transform.LookAt(targetImpactPos);
                yield return null;
            }

            if (projectile != null) Destroy(projectile);
        }

        TriggerCameraShake(spell.impactCameraShake);

        if (spell.impactExplosionPrefab != null)
        {
            GameObject explosion = Instantiate(spell.impactExplosionPrefab, targetImpactPos, Quaternion.identity);
            PlayParticles(explosion);
            Destroy(explosion, spell.destroyImpactVfxAfter);
        }

        if (spell.targetAnimator != null && !string.IsNullOrEmpty(spell.targetDeathAnimTrigger))
        {
            spell.targetAnimator.SetTrigger(spell.targetDeathAnimTrigger);
        }

        yield return new WaitForSeconds(spell.cooldownAfterImpact);
        _isCasting = false;
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

        VisualEffect[] vfxGraphs = vfxInstance.GetComponentsInChildren<VisualEffect>();
        foreach (VisualEffect vfx in vfxGraphs)
        {
            vfx.Reinit();
            vfx.SendEvent("OnPlay");
        }

        ParticleSystem[] particles = vfxInstance.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
        {
            ps.Clear();
            ps.Play();
        }
    }
}