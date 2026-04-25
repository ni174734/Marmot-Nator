using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExitTrigger : MonoBehaviour
{
    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.HasMetQuota())
        {
            GameManager.Instance.CompleteLevel();
			GameManager.Instance.ProceedToNextLevel();
        }
    }
}