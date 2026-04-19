using UnityEngine;

public class PestControlScreamReact : MonoBehaviour
{
    private EnemyVisionChase chase;

    private void Awake()
    {
        chase = GetComponent<EnemyVisionChase>();
    }

    public void OnScreamHeard(Vector2 screamOrigin)
    {
		Debug.Log("Pest Control heard scream");
		
		if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPestStun();
		
        if (chase != null)
            chase.StartStun();
    }
}