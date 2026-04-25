using UnityEngine;

public class LevelChunk : MonoBehaviour
{
    public enum ChunkType
    {
        Ground,
        UpperGround,
        StairsUp,
        StairsDown,
        Platform
    }

    [Header("Type")]
    public ChunkType chunkType;
	
	[Header("Connectors")]
    public Transform startConnector;
    public Transform endConnector;
}