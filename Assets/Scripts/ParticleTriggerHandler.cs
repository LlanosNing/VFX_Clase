using System.Collections.Generic;
using UnityEngine;

public class ParticleTriggerHandler : MonoBehaviour
{
    private ParticleSystem ps;
    private List<ParticleSystem.Particle> enterParticles = new List<ParticleSystem.Particle>();

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Se ejecuta automáticamente cuando una partícula cumple la condición del Trigger
    void OnParticleTrigger()
    {
        // Obtener las partículas que entraron en la zona este frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterParticles);

        if (numEnter > 0)
        {
            // ¡Aquí llamas a tu función personalizada!
            MiFuncionPersonalizada();
        }
    }

    void MiFuncionPersonalizada()
    {
        Debug.Log("Script activado");
    }
}
