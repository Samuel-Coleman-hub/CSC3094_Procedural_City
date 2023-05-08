using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private bool shaking;
    private Transform originalCameraPos;
    private void Start()
    {
        originalCameraPos = this.transform;
    }

    public IEnumerator CameraShake(float magnitude, float duration)
    {
        if (shaking)
        {
            yield return null;
        }

        shaking = true;
        float time = 0f;
        Debug.Log("Shaking");
        while (time < duration)
        { 
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, -10f);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = originalCameraPos.position;
        shaking = false;
    }
}
