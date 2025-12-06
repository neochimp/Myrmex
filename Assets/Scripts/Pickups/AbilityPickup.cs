using UnityEngine;

public class AbilityPickup : Pickup
{   
    // The type of ability this pickup symbolizes.
    // A good use case of the scriptable object type.  
    [SerializeField] AbilitySO abilitySO; 

    protected override void OnPickup(ActiveAbility activeAbility)
    {   
        // See ActiveWeapon.CS
        activeAbility.SwitchAbility(abilitySO);
    }
}
