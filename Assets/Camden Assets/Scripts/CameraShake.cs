using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float amplitude = 0.05f; // EDIT VAL: How far the camera moves
    public float frequency = 1.5f;  // EDIT VAL: How fast the camera moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * frequency) * amplitude;
        float y = Mathf.Cos(Time.time * frequency * 0.8f) * amplitude;
        transform.localPosition = startPos + new Vector3(x, y, 0);
    }
}