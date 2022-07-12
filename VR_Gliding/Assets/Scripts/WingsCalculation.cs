//-------------------------------------------------------------------------------
//------------------------Isabel Bartelmus 06.07.22------------------------------
//--Calculating the hand distance from the core to get the wingspan for gliding--
//-------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WingsCalculation : MonoBehaviour
{
    //---------Variables-------------

    [Header ("Game object only ther for debugging. Will set in runtime")]
    [SerializeField] private GameObject RightHand;
    [SerializeField] private GameObject LeftHand;
    [SerializeField] private GameObject Core;
    [SerializeField] private GameObject Head;
    [SerializeField] private GameObject Player;

    private Rigidbody rb;

    private float distance;
    private float dragForce = 0;
    private float max_wingspan = 3f;
    private float flightDirection;
    private Vector3 rotationOfPlayer;
    public float PlyerWeight;
    void Awake()
    {
        // Get Components in Awake
        RightHand = GameObject.FindGameObjectWithTag("RHand");
        LeftHand  = GameObject.FindGameObjectWithTag("LHand");
        Core      = GameObject.FindGameObjectWithTag("Core");
        Head      = GameObject.FindGameObjectWithTag("Head");
        Player    = GameObject.FindGameObjectWithTag("Player");
        rb = Player.GetComponent<Rigidbody>();

        rb.AddForce(500f*rb.mass, 0, 0);

        //
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {   
        distance = GetHandDistance();
        distance = ClampWingspan(distance);
        dragForce = GravityCalculation(distance, rb.velocity.y);
        flightDirection = Player.transform.rotation.eulerAngles.y;
        directionOfFlight(flightDirection);
    }
    
    public float GetHandDistance()
    {
        float fullDist = 0f;

        float rightDistance = Vector3.Distance(RightHand.transform.position , Core.transform.position);
        float leftDistance = Vector3.Distance(LeftHand.transform.position, Core.transform.position);

        fullDist = rightDistance + leftDistance;

        #region Visualisation
        Debug.DrawLine(RightHand.transform.position , Core.transform.position,Color.magenta,rightDistance);
        Debug.DrawLine(LeftHand.transform.position, Core.transform.position, Color.blue ,leftDistance);
        Debug.DrawLine(Head.transform.position, Core.transform.position, Color.red , 2);

        #endregion

        return fullDist;
    }

    public float GravityCalculation(float wingspan, float falling_velocity)
    {    
        float area_coefficient = 40f;
        float density_Air = 1.225f;      
        float drag_coefficient = 2;         // depends on the object causing the drag. The factor 2 is similar to a rectangle

        // formula to calculate drag which is dependent on the velocity
        // this allows to simulate a terminal falling velocity
        float resultingForce = 0.5f * area_coefficient * wingspan * density_Air * drag_coefficient * (falling_velocity * falling_velocity);
        if (falling_velocity > 0)
        {
            resultingForce = -resultingForce;
        }
        return resultingForce;
    }

    public float ClampWingspan(float wingspan)
    {
        if(wingspan > max_wingspan)
        {
            wingspan = max_wingspan;
        }
        if(wingspan < 1.6f)
        {
            wingspan = 0.003f;
        }

        return wingspan/max_wingspan;
    }
        

    }


    public void directionOfFlight(float viewingDirection)
    {
        Vector3 tempVector = rb.velocity;
        float horizontalVelocity = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));
        float degreeToRadian = viewingDirection * Mathf.PI /180;
        tempVector.x = horizontalVelocity * Mathf.Sin(degreeToRadian);
        tempVector.z = horizontalVelocity * Mathf.Cos(degreeToRadian);
        rb.velocity = tempVector;
    }


}
