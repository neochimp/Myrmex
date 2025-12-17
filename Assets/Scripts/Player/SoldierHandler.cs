using Cinemachine;
using UnityEngine;

public class SoldierHandler : MonoBehaviour
{

    // Handles the main functionality for all Soldier abilities
    // Such as: the ability to shoot acid. 
    // The ability to bite, etc. 
    ParticleSystem acidSplash;

    // Impulse source is for screen shake effects
    CinemachineImpulseSource impulseSource;

    // The interaction layers are how we adjust raycasting
    // For example if we don't want projectiles to shoot pickups, add them to their own protected layer
    [SerializeField] LayerMask interactionLayers;

    [SerializeField] AudioClip[] attackSound;
    private AudioSource sfx;
    void Start()
    {
        // Yes we could have serialized a field, but I don't want a bunch of serializations. 
        // This acid splash should be included in the prefab itself. 
        sfx = gameObject.GetComponent<AudioSource>();
        acidSplash = GetComponentInChildren<ParticleSystem>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void FireSoldierAbility(AbilitySO abilitySO)
    {
        // This is attached to the ability prefab itself, and called by the SoldierAbility script. 
        // On left click, the ant shoots acid
        // **A NOTE ON RAYCASTING**
        // Use the cameras position (reference is precached by unity as Camera, so no need to save variable)
        // Then, cast our from the forward direction of main camera
        // Use out keyword with raycast hit, :out: stores the information of objects we hit (among other data)
        // Raycast will travel for a distance of Infinity (essentially no distance cap on projectiles)

        // Note that RayCastHit will only return a value if we hit a collider. This prevents null ref (for example if player shoots at sky)
        RaycastHit hit;
        // Generate a screen shake
        impulseSource.GenerateImpulse();
        // Acid splash particle effect
        acidSplash.Play();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, abilitySO.range, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            sfx.PlayOneShot(attackSound[Random.Range(0, attackSound.Length - 1)]);

            if (hit.collider.tag == "Enemy")
            {
                EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                enemyHealth.TakeDamage(abilitySO.Damage);
                // A special particle effect to visualize damage. 
                Instantiate(abilitySO.DamageEffect, hit.point, Quaternion.identity);
            }
            else
            {
                // Just generate the standard visual for shooting an object. 
                Instantiate(abilitySO.HitEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
