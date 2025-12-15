using UnityEngine;

public class AbilityPickup : Pickup
{   
    // The type of ability this pickup symbolizes.
    // A good use case of the scriptable object type.  
    [SerializeField] AbilitySO abilitySO; 

    protected override void OnPickup(SoldierAbility soldierAbility)
    {   
        // See ActiveWeapon.CS
        soldierAbility.SwitchAbility(abilitySO);
    }
}
