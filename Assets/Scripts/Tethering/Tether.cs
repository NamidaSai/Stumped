using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*Actual connective element between tethered object*/
public class Tether : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] float MinRange;
    [SerializeField] float MaxRange;
    [SerializeField] float PullBackDistanceOffset = 2f;
    [SerializeField] float PullbackForce= 10f;
    [SerializeField] float GrabRange = 1f;
    [SerializeField] Rigidbody2D StartRB;
    [SerializeField] Transform StartTransform;
    [SerializeField] Rigidbody2D EndRB;
    [SerializeField] Transform EndTransform;
    private void Start()
    {
        UnityEngine.Assertions.Assert.IsTrue(MinRange < MaxRange, "minimum tether range can't be larger that maximum one");
     
    }
    Vector2 CurrentInput;
    internal void RecieveMoveInput(Vector2 InputAxis)
    {

        CurrentInput = InputAxis;
    }
    bool grabbing = false;
    bool Controlled = false;
    internal void ToggleGrab()
    {
        if (grabbing)
        {
            var joint = EndTransform.GetComponent<FixedJoint2D>();
            joint.connectedBody = null;
            joint.enabled = false;
            grabbing = false;
            //animating stuff
        }
        else
        {
            var ReachableTetherables = Physics2D.OverlapCircleAll(EndTransform.position, GrabRange, LayerMask.GetMask("Tetherables")) ;
            
            var minDistance = GrabRange + 1f;
            Collider2D selectedCollider = null;
            var currentPosition = EndTransform.position;
            if(ReachableTetherables.Length == 0)
            {
                Debug.Log("no tetherables detected");
                return;
            }
            foreach(var tetherableCollider in ReachableTetherables)
            {
                var dist = Vector2.Distance(currentPosition, tetherableCollider.transform.position);
                if (dist < minDistance)
                {
                    selectedCollider = tetherableCollider;
                    minDistance = dist;
                }
            }
            if(selectedCollider == null)
            {
                // could have some feedback here
                return;
            }
            var joint = EndTransform.GetComponent<FixedJoint2D>();
            joint.connectedBody = selectedCollider.attachedRigidbody;
            joint.enabled = true;
            grabbing = true;
            Debug.Log("Tethered babey!");
        }

      
    }
    private void FixedUpdate()
    {
        var feedbackForceCoefficient = Mathf.InverseLerp(MinRange, MaxRange, Vector2.Distance(StartTransform.position, EndTransform.position));
        feedbackForceCoefficient *= feedbackForceCoefficient;
        EndRB.AddForce(CurrentInput * Speed + (Vector2)(StartTransform.position-EndTransform.position)* Mathf.Clamp01(feedbackForceCoefficient)*PullbackForce);//add math operation for range
        feedbackForceCoefficient = Mathf.InverseLerp(MinRange+ PullBackDistanceOffset, MaxRange+PullBackDistanceOffset, Vector2.Distance(StartTransform.position, EndTransform.position));
        StartRB.AddForce((EndTransform.position - StartTransform.position) * feedbackForceCoefficient*PullbackForce);
    }

    internal void EnterControlMode()
    {
        Controlled = true;
        CurrentInput = Vector2.zero;
    }

    internal void ExitControlMode()
    {
        Controlled = false;
        CurrentInput = Vector2.zero;
    }
}
