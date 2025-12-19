using System;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private int _gateIndex;

    private void OnTriggerEnter(Collider other)
    {
        var kartControl = other.gameObject.GetComponent<KartController>();
        if (kartControl != null)
        {
            if (_gateIndex != kartControl.LastGateIndex)
            {
                kartControl.PassGate(_gateIndex);
            }
        }
    }
}
