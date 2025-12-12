using UnityEngine;

public class FoodItem : MonoBehaviour
{   
    public float DistanceToTarget(Transform target)
    {
        Transform currentPosition = gameObject.GetComponent<Transform>(); 

        return (target.position - currentPosition.position).sqrMagnitude; 
    }

    public Transform foodLocation()
    {
        return gameObject.transform; 
    }
}
