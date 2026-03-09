using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public enum FoodType
    {
        Healthy,
        Junk,
        Boost
    }

    public FoodType foodType;
}