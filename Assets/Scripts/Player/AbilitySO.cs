using UnityEngine;

// This is the scriptable object for abilities used by the worker/soldier ant (the player)
// It is intended to be attached to the prefabs of the abilities themselves, acting like external appendages/modules
// They can be equipped or unequipped, their information/data is contained within this scriptable object. 

// TLDR: ScriptableObject == Data
// MnonehaviorScript == Functionality

// PROTOTYPE:This creates an enum so we know if the ability is primary or secondary. 
//public enum AbilityType { Primary, Secondary }

// This creates a menu option in the unity editors,
// So that we can quickly create scriptable objects in the editor. 
[CreateAssetMenu(fileName = "AbilitySO", menuName = "Scriptable Objects/AbilitySO")]

public class AbilitySO : ScriptableObject
{   
    public int Damage = 1; 

    public float FireRate = .5f; 

    public float ZoomAmount = 10f;

    public float ZoomRotationSpeed = 0.2f; 
    public int MagazineSize = 12; 

    public float range = Mathf.Infinity; // Important for raycasting, such as creating melee objects or short range abilities

    public bool IsAutomatic = false; 

    public bool CanZoom = false; 
    public GameObject AbilityPrefab; // The prefab for the model itself. 
    public GameObject HitEffect; // Visual effects for ability activation. 
    public GameObject DamageEffect; 

    
}
