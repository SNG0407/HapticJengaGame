using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class pickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;

    [Header("Phusics Parameters")]
    [SerializeField] private float pickupRange = 15.0f;
    [SerializeField] private float pickupForce = 150.0f;
    public bool CheckClick = false;
    private void Update()
    {
        CheckClick = GameObject.Find("SerialReader").GetComponent<GyroV2>().ButtonClicked;
        if (CheckClick == false&& (heldObj != null))
        {
            //OnCollisionStay(Collision collision);
            //Drop
            DropObject();
            Debug.Log("DropObject");
        }
        else
        {
            if (CheckClick == true&&heldObj != null)
            {
                ///Move
                MoveObejct();
                Debug.Log("MoveObejct");

            }
        }
            
        //Debug.Log(CheckClick);
        //if (Input.GetMouseButtonDown(0)|| CheckClick == true)
        //{
        //    Debug.Log("GetMouseButtonDown");
        //    if (heldObj == null)
        //    {
        //        RaycastHit hit;
        //        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
        //        {
        //            Debug.DrawRay(transform.position, transform.forward * pickupRange, Color.red);
        //            //Pickup
        //            PickupObject(hit.transform.gameObject);
        //            Debug.Log("PickupObject");
        //        }
        //    }
        //    else
        //    {
        //        //Drop
        //        DropObject();
        //        Debug.Log("DropObject");
        //    }
        //}
        //if (heldObj != null)
        //{
        //    ///Move
        //    MoveObejct();
        //    Debug.Log("MoveObejct");

        //}
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " : Ãæµ¹ Áß");

        if (CheckClick == true)
        {
            Debug.Log("GetMouseButtonDown");
            if (heldObj == null)
            {
                //Pickup
                    PickupObject(collision.transform.gameObject);
                    Debug.Log("PickupObject");
            }
        //    else
        //    {
        //        //Drop
        //        DropObject();
        //        Debug.Log("DropObject");
        //    }
        //}
        //if (heldObj != null)
        //{
        //    ///Move
        //    MoveObejct();
        //    Debug.Log("MoveObejct");

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        try
        {
            heldObj.GetComponent<HapticMesh>().enabled = true;
        }
        catch (NullReferenceException)
        {
            Debug.Log("NullReferenceException");
        }
        catch (Exception)
        {
            Debug.Log("Exception");
        }

    }


    void MoveObejct()
    {
        //if (Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f)
          if (Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);
            //heldObjRB.AddForce(moveDirection * pickupForce);
            heldObj.transform.position = gameObject.transform.position;
        }
    }

    void PickupObject(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>())
        {
            //gameObject.transform.position = pickObj.GetComponent<Renderer>().bounds.center;
            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = holdArea;
            heldObj = pickObj;
            heldObj.transform.position = gameObject.transform.position;//GetComponent<Renderer>().bounds.center;
            heldObj.GetComponent<HapticMesh>().enabled = false;
        }
    }
    void DropObject()
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        //StartCoroutine(delay(heldObj, heldObjRB)) ;
        //heldObj.GetComponent<HapticMesh>().enabled = true;

        heldObjRB.transform.parent = null;
        heldObj = null;

    }

    private IEnumerator delay(GameObject heldObj, Rigidbody heldObjRB)
    {
        var wait = new WaitForSeconds(0.8f);
        yield return wait;
        heldObj.GetComponent<HapticMesh>().enabled = true;

        heldObjRB.transform.parent = null;
        heldObj = null;
    }
}
