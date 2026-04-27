using UnityEngine;

public class parallaxScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;
        [Range(0f, 1f)] public float parallaxAmount = 0.5f;
        public bool followCameraY = false;
    }

    [Header("Camera")]
    [SerializeField] private Transform cam;

    [Header("Sprite Background Layers")]
    [SerializeField] private ParallaxLayer[] layers;

    private Vector3 camStartPos;
    private Vector3[] layerStartPositions;

    private void Start()
    {
        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        if (cam == null)
        {
            Debug.LogError("No camera assigned to parallaxScroller.");
            enabled = false;
            return;
        }

        camStartPos = cam.position;

        layerStartPositions = new Vector3[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layer != null)
                layerStartPositions[i] = layers[i].layer.position;
        }
    }

    private void LateUpdate()
    {
        Vector3 camDelta = cam.position - camStartPos;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layer == null) continue;

            Vector3 start = layerStartPositions[i];

            float x = start.x + camDelta.x * layers[i].parallaxAmount;
            float y = layers[i].followCameraY
                ? start.y + camDelta.y * layers[i].parallaxAmount
                : start.y;

            layers[i].layer.position = new Vector3(x, y, start.z);
        }
    }
}