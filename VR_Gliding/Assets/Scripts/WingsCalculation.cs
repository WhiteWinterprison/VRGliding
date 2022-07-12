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

#region  AirResistanceVariables
    private Rigidbody rb; 

    private float distance; // aka Wing span. From hand to hand
    private float dragForce = 0; //resistance of the aire on y (falling and upwards falling)
    private float max_wingspan = 3f; //max wingspan humans can have

#endregion

    private float angleOfAttack = -30f; //Pitch up|down
    private float flightDirection;  //yawn 
    private Vector3 rotationOfPlayer;   //role

    private float startTime;  //debug. funktionality

    void Awake()
    {
        // Get Components in Awake
        RightHand = GameObject.FindGameObjectWithTag("RHand");
        LeftHand  = GameObject.FindGameObjectWithTag("LHand");
        Core      = GameObject.FindGameObjectWithTag("Core");
        Head      = GameObject.FindGameObjectWithTag("Head");
        Player    = GameObject.FindGameObjectWithTag("Player");
        
        rb = Player.GetComponent<Rigidbody>();

        rb.AddForce(500f*rb.mass, 0, 0); //Start force in x & z
        startTime = Time.time;

    }

    void Update()
    {
        
    }

    void FixedUpdate() //need Fix update to work with Unity Physic engine part
    {   
        distance = GetHandDistance();
        distance = ClampWingspan(distance);
        dragForce = GravityCalculation(distance, rb.velocity.y);//how fast do i fall  & distnace of my hands
        flightDirection = Player.transform.rotation.eulerAngles.y; //get current player direction(rotation on y)
        directionOfFlight(flightDirection); 
        
        ReduceSpeed(angleOfAttack, flightDirection); //get controler rotation & flight direction(headset)
        IncreaseSpeed(angleOfAttack,dragForce, flightDirection);
        rb.AddForce(0, dragForce, 0); // use force magic

        // Print Debug
        Debug.Log(rb.velocity.x + "\n"+ rb.velocity.z);

        if((Time.time -startTime) > 3)
        {
            angleOfAttack = 0f;
            rotationOfPlayer = Player.transform.eulerAngles;
            rotationOfPlayer.y = 45f;

            Player.transform.eulerAngles = rotationOfPlayer;
        }
        if((Time.time -startTime) > 5)
        {
            angleOfAttack = 30f;
        }
        if((Time.time -startTime) > 7)
        {
            angleOfAttack = 0f;
        }
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

    public float GravityCalculation(float wingspan, float falling_velocity) //calcualtes the plays drag while falling
    {    
        //Numbers based on Reallive data :D Physics and magic
        //Formel : dragForce = Coef * wingSpan * speed^2 
        //area_coefficient+density_Air+drag_coefficient = Coef
        float area_coefficient = 40f;
        float density_Air = 1.225f;      
        float drag_coefficient = 2;         // depends on the object causing the drag. The factor 2 is similar to a rectangle

        // formula to calculate drag which is dependent on the velocity
        // this allows to simulate a terminal falling velocity
        float resultingForce = 0.5f * area_coefficient * wingspan * density_Air * drag_coefficient * (falling_velocity * falling_velocity);
        if (falling_velocity > 0)// if falling upwards change direction of drag
        {
            resultingForce = -resultingForce;
        }
        return resultingForce;
    }

    public float ClampWingspan(float wingspan) //unity funktion exsist
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

    public void ReduceSpeed(float climbingAngle, float viewingDirection)
    {   
        
        float tempVelocity = 0;
        float horizontalVelocity = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2)); //speed mit Phytagoras
        float degreeToRadian = climbingAngle * Mathf.PI /180; //get radian
        float viewingDirectionRadian = viewingDirection * Mathf.PI /180;

        Vector3 tempVector = rb.velocity; //save so easy to change
    
        float Force = 0;

        if (horizontalVelocity > 0 && climbingAngle >= 0)//if moving foward && angle >= 0 //only if moving forward i can go up
        {   
            tempVelocity = horizontalVelocity - 9.81f * 0.02f * Mathf.Sin(degreeToRadian); //9.81 = gravaty , 0.02f time between fixed updates //physic 
            Force = 40f * Mathf.Sin(degreeToRadian)* rb.mass;
            rb.AddForce(0,Force,0);
            if (tempVelocity < 0) 
            {
                tempVector.x = 0;
                tempVector.z = 0;
                rb.velocity = tempVector;
            }
            else 
            {
                tempVector.x = tempVelocity * Mathf.Sin(viewingDirectionRadian);
                tempVector.z = tempVelocity * Mathf.Cos(viewingDirectionRadian);
                rb.velocity = tempVector;
            }

        }
    }

    public void IncreaseSpeed(float climbingAngle, float dragForce, float viewingDirection)
    {   
        if(climbingAngle < 0) //angelofattack < 0 //controler points down

        {
            float degreeToRadian = climbingAngle * Mathf.PI /180; //radian claculation magic
            float directionRadian = viewingDirection * Mathf.PI /180;
            //part you fall fast will move you forward (ex: 5 = 2+3), (sin = x;)(Cos = z)
            rb.AddForce(-Mathf.Sin(degreeToRadian)*dragForce * Mathf.Sin(directionRadian), Mathf.Sin(degreeToRadian)*dragForce, -Mathf.Sin(degreeToRadian)*dragForce * Mathf.Cos(directionRadian));
        }
        
    }

    public void directionOfFlight(float viewingDirection)
    {
        //each loop check view direction and update velocity vectore 
        Vector3 tempVector = rb.velocity;
        float horizontalVelocity = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2)); //Phytagoras :3
        float degreeToRadian = viewingDirection * Mathf.PI /180; //calculate degree to radian
        tempVector.x = horizontalVelocity * Mathf.Sin(degreeToRadian);
        tempVector.z = horizontalVelocity * Mathf.Cos(degreeToRadian);
        rb.velocity = tempVector;
    }


}
