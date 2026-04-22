using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ScreamWave : MonoBehaviour
{
    [SerializeField] private float maxRadius = 4f;
    [SerializeField] private float expandSpeed = 12f;
    [SerializeField] private float lifetime = 0.35f;

    private CircleCollider2D circle;
    private float timer;

    private void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        circle.isTrigger = true;
        circle.radius = 0.1f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / lifetime);
		float radius = Mathf.Lerp(0.1f, maxRadius, t);
		circle.radius = radius;
		
		transform.localScale = Vector3.one * radius * 2f;

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 screamOrigin = transform.position;
		
		Debug.Log("Scream hit " + other.name);

        MarmotScreamReact marmot = other.GetComponentInParent<MarmotScreamReact>();
        if (marmot != null)
        {
			Debug.Log("Marmot scream reaction found on: " + marmot.name);
            marmot.OnScreamHeard(screamOrigin);
            return;
        }

        DogScreamReact dog = other.GetComponentInParent<DogScreamReact>();
        if (dog != null)
        {
			Debug.Log("Dog scream reaction found on: " + dog.name);
            dog.OnScreamHeard(screamOrigin);
            return;
        }

        PestControlScreamReact pest = other.GetComponentInParent<PestControlScreamReact>();
        if (pest != null)
        {
			Debug.Log("Pest control scream reaction found on: " + pest.name);
            pest.OnScreamHeard(screamOrigin);
        }
    }
}