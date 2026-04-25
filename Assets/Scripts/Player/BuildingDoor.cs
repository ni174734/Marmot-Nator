using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class BuildingDoor : MonoBehaviour, IInteractable
{
    [Header("Teleport Points")]
    [SerializeField] private Transform interiorPoint;
    [SerializeField] private Transform exteriorPoint;

    [Header("Interior Walls")]
    [SerializeField] private Collider2D[] interiorWalls;

    [Header("Prompt UI")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TextMeshProUGUI promptText;

    private playerController playerInRange;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        /*
		if (promptObject != null)
            promptObject.SetActive(false);
		*/

        // Start with walls disabled (player is outside)
        SetInteriorWalls(false);
    }

    public void Interact(Transform interactor)
    {
        PlayerBuildingState state = interactor.GetComponent<PlayerBuildingState>();
        if (state == null) return;

        bool goingInside = !state.IsInsideBuilding;

        Transform target = goingInside ? interiorPoint : exteriorPoint;
        if (target == null) return;

        Rigidbody2D rb = interactor.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        interactor.position = target.position;

        if (goingInside)
        {
            state.EnterBuilding();
            SetInteriorWalls(true);
        }
        else
        {
            state.ExitBuilding();
            SetInteriorWalls(false);
        }

        UpdatePrompt();
    }

    private void SetInteriorWalls(bool active)
    {
        for (int i = 0; i < interiorWalls.Length; i++)
        {
            if (interiorWalls[i] != null)
                interiorWalls[i].enabled = active;
        }
    }

    private void UpdatePrompt()
    {
        if (promptText == null) return;

        if (PlayerBuildingState.Instance != null &&
            PlayerBuildingState.Instance.IsInsideBuilding)
        {
            promptText.text = "Press Enter to Exit";
        }
        else
        {
            promptText.text = "Press Enter to Enter";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerController pc = other.GetComponent<playerController>();
        if (pc == null) return;
		
		Debug.Log("Player ENTERED door trigger → " + gameObject.name);

        playerInRange = pc;
        playerInRange.SetInteractable(this);

        
		if (promptObject != null)
            promptObject.SetActive(true);

        UpdatePrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerController pc = other.GetComponent<playerController>();
        if (pc == null) return;
		
		Debug.Log("Player ENTERED door trigger → " + gameObject.name);

        pc.ClearInteractable(this);

        if (playerInRange == pc)
            playerInRange = null;

        /*
		if (promptObject != null)
            promptObject.SetActive(false);*/
    }
}