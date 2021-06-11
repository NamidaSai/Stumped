using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SOEventListener : MonoBehaviour
{
    public UnityEvent action;
    public SOEvent SOEvent;
    private void OnEnable()
    {
        SOEvent.AddListener(this);
    }
    private void OnDisable()
    {
        SOEvent.RemoveListener(this);
    }
}
