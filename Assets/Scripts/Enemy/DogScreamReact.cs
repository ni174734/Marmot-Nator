using UnityEngine;

public class DogScreamReact : MonoBehaviour
{
    private EnemyVisionChase chase;

    private void Awake()
    {
        chase = GetComponent<EnemyVisionChase>();
    }

    public void OnScreamHeard(Vector2 screamOrigin)
    {
		Debug.Log("Dog heard scream");
		
		if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDogRetreat();
		
        if (chase != null)
            chase.StartRetreat(screamOrigin);
    }
}