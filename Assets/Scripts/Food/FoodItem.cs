using Unity.VisualScripting;
using UnityEngine;

// Technically this script doesn't even need to exist.
// But it's a simple way to both retrieve FoodPickup GameObjects uniformally (they all have this script attached)
// And it also gives a clean, hierarchical destruction. (We risk destroying only the mesh object, or root etc.)
public class FoodItem : MonoBehaviour
{   
    public void DestroyFood()
    {
        Destroy(this.gameObject); 
    }
}
