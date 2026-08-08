using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] ParticleSystem particleSystem;

    ParticleSystemRenderer particleRenderer;
    
    public ParticleSystem ParticleSystem => particleSystem;
    public Player Player => player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireParticles(Vector2 direction, float coneAngle, Material particleMaterial)
    {
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.arc = coneAngle;
        particleRenderer.material = particleMaterial;
        particleSystem.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - coneAngle / 2);
        particleSystem.Play();
    }
}
