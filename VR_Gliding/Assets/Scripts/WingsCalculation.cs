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

    public float PlyerWeight;
    void Awake()
    {
        // Get Components in Awake
        RightHand = GameObject.FindGameObjectWithTag("RHand");
        LeftHand  = GameObject.FindGameObjectWithTag("LHand");
        Core      = GameObject.FindGameObjectWithTag("Core");
        Head      = GameObject.FindGameObjectWithTag("Head");
        Player    = GameObject.FindGameObjectWithTag("Player");

        //
    }

    void Update()
    {
        GetHandDistance();
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

    public void GravityCalculation()
    {    
        

    }

}
