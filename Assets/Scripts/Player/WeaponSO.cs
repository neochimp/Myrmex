using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public int Damage = 1; 

    public float FireRate = .5f; 

    public float ZoomAmount = 10f;

    public float ZoomRotationSpeed = 0.2f; 

    public float DefaultRotationSpeed = 1f; 

    public float DefaultFOV = 40f; 

    public int MagazineSize = 12; 

    public bool IsAutomatic = false; 

    public bool CanZoom = false; 

    public GameObject WeaponPrefab; 
    public GameObject HitEffect;
    public GameObject DamageEffect; 
}
