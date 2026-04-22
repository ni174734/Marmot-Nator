using UnityEngine;

public class MarmotScreamReact : MonoBehaviour
{
    private EnemyPatrol patrol;

    private void Awake()
    {
        patrol = GetComponent<EnemyPatrol>();
    }

    public void OnScreamHeard(Vector2 screamOrigin)
    {
        if (patrol != null)
            patrol.StartRetreat(screamOrigin);
    }
}