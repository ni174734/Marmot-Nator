using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [Header("Spawn Area")]
    public Vector2 size = new Vector2(3f, 1f);

    [Header("Amount")]
    public int minSpawns = 1;
    public int maxSpawns = 3;

    public Vector3 GetRandomPoint()
    {
        Vector3 center = transform.position;

        float x = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
        float y = Random.Range(center.y - size.y * 0.5f, center.y + size.y * 0.5f);

        return new Vector3(x, y, center.z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, size);
    }
#endif
}