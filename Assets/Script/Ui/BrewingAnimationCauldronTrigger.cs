using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingAnimationCauldronTrigger : MonoBehaviour
{
    [SerializeField] ParticleSystem SmokeParticle;
    [SerializeField] ParticleSystem StirParticle;
    private void BrewAnimComplete()
    {
        PotionMakingUi.Instance.CauldronBrewAnimCompleted();
        StopAllParticles();
    }
    private void ResetAnimComplete()
    {
        PotionMakingUi.Instance.CauldronResetAnimCompleted();
    }

    private void StopAllParticles()
    {
        if (SmokeParticle != null)
            SmokeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (StirParticle != null)
            StirParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void StartParticle()
    {
        //StopAllParticles();
        if (SmokeParticle != null)
        {
            SmokeParticle.Play();
        }
        
    }

    private void StartStirParticle()
    {
        StopAllParticles();
        if (StirParticle != null)
        {
            StirParticle.Play();
        }
    }
}
