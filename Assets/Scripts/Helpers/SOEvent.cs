using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Event", menuName = "SOEvent")]
public class SOEvent : ScriptableObject
{
    List<SOEventListener> listeners = new List<SOEventListener>();

    public void Invoke()
    {
        foreach(var listener in listeners)
        {
            listener.action?.Invoke();
        }
    }
    public void AddListener(SOEventListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(SOEventListener listener)
    {
        listeners.Remove(listener);
    }
}
