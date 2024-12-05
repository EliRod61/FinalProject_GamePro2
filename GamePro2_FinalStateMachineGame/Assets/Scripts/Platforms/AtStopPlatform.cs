using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static StateRotation;
using System.Linq;

public class AtStopPlatform : MonoBehaviour
{
    //Will NOT MOVE at start, activated by player collision, waits ATSTOP until player leaves 
    //It will MOVE between destinations (back and forth) as long as player is on it
    //Will RETURN to its starting point when no one is on it

    public List<Vector3> Destinations;
    private int CurrentDest;
    public float Speed = 0.1f;
    private List<Transform> Riders = new List<Transform>();
    public enum platformMode
    {
        IDLE,
        MOVING,
        RETURN,
        ATSTOP
    }
    private void Start()
    {
        platMode = platformMode.IDLE;
    }
    public platformMode platMode;

    void FixedUpdate()
    {
        switch (platMode)
        {
            case platformMode.IDLE:
                break;
            case platformMode.MOVING:
                PlatformActive();
                break;
            case platformMode.RETURN:
                PlatformMoveBack();
                break;
            case platformMode.ATSTOP:
                break;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!Riders.Contains(other.transform))
            Riders.Add(other.transform);

        if (platMode == platformMode.RETURN)
        {
            CurrentDest++;
            if (CurrentDest >= Destinations.Count)
            {
                CurrentDest = 0;
            }
        }
            platMode = platformMode.MOVING;
    }

    private void OnCollisionExit(Collision other)
    {
        Riders.Remove(other.transform);

        platMode = platformMode.RETURN;

        CurrentDest--;
        if (CurrentDest < 0)
            CurrentDest = Destinations.Count - 1;
    }

    void PlatformActive()
    {
        if (Destinations.Count == 0) return;
        Vector3 dest = Destinations[CurrentDest];
        Vector3 old = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
        Vector3 movement = transform.position - old;
        Vector3 lastStop = Destinations.Last();
        foreach (Transform tra in Riders)
        {
            tra.position += movement;
        }

        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            CurrentDest++;
            
            if (CurrentDest >= Destinations.Count) 
            { 
                CurrentDest = 0;
            }
        }
        if (Vector3.Distance(transform.position, lastStop) < 0.01f)
        {
            platMode = platformMode.ATSTOP;
        }
    }

    void PlatformMoveBack()
    {
        if (Destinations.Count == 0) return;
        Vector3 dest = Destinations[CurrentDest];
        Vector3 old = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
        Vector3 movement = transform.position - old;

        foreach (Transform tra in Riders)
        {
            tra.position += movement;
        }

        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            CurrentDest--;
            if (CurrentDest < 0)
            {
                CurrentDest = 0;
                platMode = platformMode.IDLE;
            }
        }
    }
}