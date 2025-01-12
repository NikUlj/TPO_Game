using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectPool : MonoBehaviour
{

    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] private int poolSize = 20;

    private readonly Queue<ParticleSystem> _pool = new();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(particleEffect);
            effect.gameObject.SetActive(false);
            _pool.Enqueue(effect);
        }
    }

    public ParticleSystem GetEffect()
    {
        if (_pool.Count > 0)
        {
            ParticleSystem effect = _pool.Dequeue();
            effect.gameObject.SetActive(true);
            return effect;
        }

        ParticleSystem newEffect = Instantiate(particleEffect);
        return newEffect;
    }

    public void ReturnEffect(ParticleSystem effect)
    {
        effect.gameObject.SetActive(false);
        _pool.Enqueue(effect);
    }
}
