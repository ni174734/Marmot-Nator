using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    float elapsedTime = 90.0f;


    // Update is called once per frame
    void Update()
    {

        elapsedTime -= Time.deltaTime;

        // Exit once we run out of time
        if (elapsedTime <= 0.0f)
        {
            return;
        }

    }
}
