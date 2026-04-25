using UnityEngine;

public class PlayerBuildingState : MonoBehaviour
{
    public static PlayerBuildingState Instance;

    [Header("State")]
    public bool IsInsideBuilding { get; private set; }

    [Header("Sprite Sorting")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private int outsideSortingOrder = 10;
    [SerializeField] private int insideSortingOrder = -10;

    private void Awake()
    {
        Instance = this;

        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void EnterBuilding()
    {
        IsInsideBuilding = true;

        if (playerSprite != null)
            playerSprite.sortingOrder = insideSortingOrder;
    }

    public void ExitBuilding()
    {
        IsInsideBuilding = false;

        if (playerSprite != null)
            playerSprite.sortingOrder = outsideSortingOrder;
    }
}