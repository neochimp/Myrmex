using UnityEngine;

// This creates a menu option in the unity editors,
// So that we can quickly create scriptable objects in the editor. 
[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
// A scriptable object
// This is a modular way to store data which is common to all instances of a class
// BUT will differ between those classes
// In this way we can create all manner of differing weapons. 
// We likely want to create other types of scriptable objects in the future, not just weapons.
// This will happen as serialized data fields start to stack up, as development continues.  

// TLDR: ScriptableObject == Data
// MnonehaviorScript == Functionality
{
    public int Damage = 1; 

    public float FireRate = .5f; 

    public float ZoomAmount = 10f;

    public float ZoomRotationSpeed = 0.2f; 
    public int MagazineSize = 12; 

    public float range = Mathf.Infinity; // PROTOTYPE

    public bool IsAutomatic = false; 

    public bool CanZoom = false; 

    public GameObject WeaponPrefab; 
    public GameObject HitEffect;
    public GameObject DamageEffect; 
}
