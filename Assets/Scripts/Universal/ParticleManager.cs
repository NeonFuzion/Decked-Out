using System.Collections;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] Material[] particleMaterials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(SpawnParticles());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
