using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseToThBeat : MonoBehaviour
{
    [SerializeField] float _pulseSize = 1.05f;
    [SerializeField] float _returnSpeed = 5f;
    private Vector3 _startSize;

    private void Start() {
        _startSize = transform.localScale;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * _returnSpeed);
    }

    public void Pulse()
    {
        transform.localScale = _startSize * _pulseSize;
    }
}
