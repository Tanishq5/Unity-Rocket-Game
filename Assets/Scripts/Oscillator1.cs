using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator1 : MonoBehaviour
{
    [SerializeField] Vector3 MovementVector;
    [SerializeField] float period = 2f;
    [SerializeField] float amplitude = 1f;

    [Range(0,1)]
   [SerializeField] float MovementScroll;

    private Vector3 StartPos;

    void Start()
    {
        StartPos = transform.position;
    }

    void Update()
    {

        float rawsinwave = Mathf.Sin(Time.time*period) * amplitude;

        MovementScroll = rawsinwave / 2f + 0.5f;

        Vector3 offset = MovementScroll * MovementVector ;

        transform.position = StartPos + offset;


    }
}
