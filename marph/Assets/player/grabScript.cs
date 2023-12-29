using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabScript : MonoBehaviour
{

    //OBJ'S
    [SerializeField] private GameObject Player;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform throwPos;
    private GameObject leftHeldObj;
    private GameObject rightHeldObj;


    //values
    [SerializeField] private float throwForce;
    [SerializeField] private float grabRange;
    public bool canDrop = true;
    private int LayerNumber;

    //keybinds
    /*
     * have it set up so that you can set custom kebinds for the left hand and right hand. 
     */

    //KeyCode rightHand 





    // Start is called before the first frame update
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }

    // Update is called once per frame
    void Update()
    {

        //left hand obj
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(leftHeldObj == null)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, grabRange))
                {
                    if(hit.transform.gameObject.tag == "grabbable")
                    {
                        leftHeldObj = hit.transform.gameObject;
                        grab(hit.transform.gameObject, leftHand);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    CeaseClipping(leftHeldObj);
                    dropItem(leftHeldObj, throwPos);
                    leftHeldObj = null;
                }
            }
        }

        //right obj hand
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (rightHeldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, grabRange))
                {
                    if (hit.transform.gameObject.tag == "grabbable")
                    {
                        rightHeldObj = hit.transform.gameObject;
                        grab(hit.transform.gameObject, rightHand);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    CeaseClipping(rightHeldObj);
                    dropItem(rightHeldObj, throwPos);
                    rightHeldObj = null;
                }
            }
        }


        if (leftHeldObj != null)
        {
            MoveObj(leftHeldObj,leftHand);
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true)
            {
                CeaseClipping(leftHeldObj);
                throwItem(leftHeldObj, throwPos);
                leftHeldObj = null;
            }
        }


        if (rightHeldObj != null)
        {
            MoveObj(rightHeldObj, rightHand);
            if (Input.GetKeyDown(KeyCode.Mouse1) && canDrop == true)
            {
                CeaseClipping(rightHeldObj);
                throwItem(rightHeldObj, throwPos);
                rightHeldObj = null;
            }
        }
    }

    void grab(GameObject grabable, Transform handPos)
    {
        Rigidbody grabableRB;
        if (grabable.GetComponent<Rigidbody>())
        {
            grabableRB = grabable.GetComponent<Rigidbody>();
            grabableRB.isKinematic = true;
            grabableRB.transform.parent = handPos;
            grabable.layer = LayerNumber;

            //This purportedly fixes the collision issues between held objs and the player.
            Physics.IgnoreCollision(grabable.GetComponent<Collider>(), Player.GetComponent<Collider>(), true);
        }
    }

    void dropItem(GameObject heldItem, Transform handPos)
    {
        Rigidbody heldItemRB = heldItem.GetComponent<Rigidbody>();

        //This purportedly fixes the collision issues between held objs and the player.
        Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), Player.GetComponent<Collider>(), false);
        heldItem.transform.position = handPos.position;
        heldItem.layer = 0;
        heldItemRB.isKinematic = false;
        heldItem.transform.parent = null;
    }

    void throwItem(GameObject heldItem, Transform handPos)
    {
        Rigidbody heldItemRB = heldItem.GetComponent<Rigidbody>();

        //This purportedly fixes the collision issues between held objs and the player.
        Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), Player.GetComponent<Collider>(), false);
        heldItem.transform.position = handPos.position;
        heldItem.layer = 0;
        heldItemRB.isKinematic = false;
        heldItemRB.AddForce(transform.forward * throwForce);
        heldItem.transform.parent = null;
    }


    void MoveObj(GameObject heldItem, Transform handPos)
    {
        heldItem.transform.position = handPos.transform.position;
    }

    void CeaseClipping(GameObject heldItem)
    {
        var clipRange = Vector3.Distance(heldItem.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1)
        {
            heldItem.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
        }
    }
}
