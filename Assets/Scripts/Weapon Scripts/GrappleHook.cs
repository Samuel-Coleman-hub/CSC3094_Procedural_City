using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] float jointSpring;
    [SerializeField] float jointDamper;
    [SerializeField] float jointMassScale;

    public int mouseButtonInt;
    public LayerMask whatCanGrappleOn;
    public Transform grapplePoint;
    public Transform cameraPosition;
    public Transform player;
    public float maxDistance = 100f;

    private SpringJoint joint;
    private LineRenderer lineRenderer;
    private Vector3 grappleDes;

    private GameObject emptyObj;
    private bool attachedToRigidbody;


    private void Awake()
    {
        cameraPosition = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform; 
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, grapplePoint.position);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(mouseButtonInt))
        {
            StartGrappling();
        }
        else if (Input.GetMouseButtonUp(mouseButtonInt))
        {
            StopGrappling();
        }
    }

    private void LateUpdate()
    {
        DrawGrappleLine();
    }

    private void StartGrappling()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out hit, maxDistance, whatCanGrappleOn))
        {
            grappleDes = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;

            if (hit.transform.gameObject.GetComponent<Rigidbody>())
            {
                attachedToRigidbody = true;
                joint.connectedBody = hit.transform.gameObject.GetComponent<Rigidbody>();
                emptyObj = new GameObject("grapplePoint");
                emptyObj.transform.position = grappleDes;
                emptyObj.transform.parent = hit.transform.gameObject.transform;
            }
            else
            {
                attachedToRigidbody=false;
                joint.connectedAnchor = grappleDes;
            }
            
            float distanceFromPoint = Vector3.Distance(player.position, grappleDes);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            lineRenderer.positionCount = 2;
        }
    }

    private void StopGrappling()
    {
        lineRenderer.positionCount = 0;
        attachedToRigidbody = false;
        emptyObj = null;
        Destroy(joint);
    }

    private void DrawGrappleLine()
    {
        if (!joint)
        {
            return;
        }

        lineRenderer.SetPosition(0, grapplePoint.position);
        lineRenderer.SetPosition(1, attachedToRigidbody ? emptyObj.transform.position : grappleDes);
    }


}
