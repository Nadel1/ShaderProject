using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 1;
    public float stopSpeed = 1;
    public float rotationSpeed = 1;
    public float maxSpeed = 10;
    public float stopPaddleSpeed = 1;
    public float minSpeed = -5;
    private float forwardVel=0;

    private GameObject paddleLinks;
    private GameObject paddleRechts;
    private GameObject boot;

    //for determining wether or not the actual paddle part is within the water
    public GameObject paddleObjectLinks;
    public GameObject paddleObjectRechts;

    private Animator animatorLinks;
    private Animator animatorRechts;
    private Animator animatorBoot;

    private bool linksturning;
    private bool rechtsturning;
    private bool isStopping;
    private bool isForward;
    private bool isBackward;

    private float xRot, yRot, zRot;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //only rotation around y allowed
        xRot = transform.rotation.x;
        zRot = transform.rotation.z;
        //find the paddles, needed for animtions and to determine wether or not the paddles are in the water,
        //if that is the case, acceleration has to be applied
        paddleLinks = GameObject.Find("paddlelinks");
        paddleRechts = GameObject.Find("paddlerechts");
        boot = GameObject.Find("bootblend");
        //get the animators to handle the animations of the paddles
        animatorLinks = paddleLinks.GetComponent<Animator>();
        animatorRechts = paddleRechts.GetComponent<Animator>();
        animatorBoot = boot.GetComponent<Animator>();

        linksturning = false;
        rechtsturning = false;
        isStopping = false;
        isForward = false;
        isBackward = false;
        
    }
    void FixedUpdate()
    {
        //animation starters
        //seperated forward movement and rotation, more true to paddling and solves the issue of animations colliding
        if (Input.GetKey(KeyCode.W))
        {
            animatorRechts.SetBool("isPaddling", true);
            animatorLinks.SetBool("isPaddling", true);
            //animatorBoot.SetBool("isPaddling", true);
            isForward = true;
            isStopping = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            animatorRechts.SetBool("isBack", true);
            animatorLinks.SetBool("isBack", true);
            isForward = false;
            isBackward = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            //start the animation of the right paddle (to turn left you need to use the right paddle)
            animatorRechts.SetBool("isPaddling", true);
            rechtsturning = true;


        }
        if (Input.GetKey(KeyCode.D))
        {
            //start the animation of the left paddle (to turn right you need to use the left paddle)
            animatorLinks.SetBool("isPaddling", true);
            linksturning = true;

        }
        //animation stopers
        //stops the animation of both paddles when the w key is no longer hold
        if (!Input.GetKey(KeyCode.W) && !rechtsturning && !linksturning&&isForward)
        {
            
            animatorLinks.SetBool("isPaddling", false);
            animatorRechts.SetBool("isPaddling", false);

        }
        //stops the animation of the right paddle if it was turning
        if (!Input.GetKey(KeyCode.A) && rechtsturning)
        {
            rechtsturning = false;
            animatorRechts.SetBool("isPaddling", false);
        }
        //stops the animation of the left paddle if it was turning
        if (!Input.GetKey(KeyCode.D) && linksturning)
        {
            linksturning = false;
            animatorLinks.SetBool("isPaddling", false);
        }
        if (!Input.GetKey(KeyCode.S))
        {
            animatorRechts.SetBool("isBack",false);
            animatorLinks.SetBool("isBack", false);
        }

        //velocity applieres
        //only accelerate forwards if both paddles are in the water
        if (paddleObjectLinks.GetComponent<inWater>().touchesWater && paddleObjectLinks.GetComponent<inWater>().touchesWater&&!isStopping&&isForward)
        {
            forwardVel += speed;
        }
        //backward acceleration
        if (paddleObjectLinks.GetComponent<inWater>().touchesWater && paddleObjectLinks.GetComponent<inWater>().touchesWater && !isStopping &&isBackward)
        {
            forwardVel -= speed*0.5f;
        }

        //only rotates if right paddle in water
        if (Input.GetKey(KeyCode.A) && rechtsturning && paddleObjectRechts.GetComponent<inWater>().touchesWater)
        {

            yRot = -rotationSpeed;
            Vector3 target = new Vector3(0, yRot, 0);
            Quaternion deltaRot = Quaternion.Euler(target * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRot);
        }
        //only rotates if left paddle in water
        if (Input.GetKey(KeyCode.D) && linksturning && paddleObjectLinks.GetComponent<inWater>().touchesWater)
        {
            yRot = rotationSpeed;
            Vector3 target = new Vector3(0, yRot, 0);
            Quaternion deltaRot = Quaternion.Euler(target * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRot);

        }

        //constant resitance, changes depending on the direction the boat is going
        if (Vector3.Dot(transform.forward, rb.velocity) > 0)
        {
            forwardVel -= stopSpeed;
        }
        if ( Vector3.Dot(transform.forward, rb.velocity) < 0)
        {
            forwardVel += stopSpeed;
        }
        forwardVel = Mathf.Clamp(forwardVel, minSpeed, maxSpeed);

        //apply the velocity: this type of movement allows to move through turning
        //eventhough this is not fully realistic, it is the most satisfactory in the used context
        rb.velocity = transform.forward * forwardVel;
        //Debug.Log(forwardVel);
    }
    private void OnCollisionEnter(Collision collision)
    {
        forwardVel = 0;
    }
}/*stopping mechanic
        if (Input.GetKey(KeyCode.X))
        {
            //start stopping animation
            animatorRechts.SetBool("isStopping", true);
            animatorLinks.SetBool("isStopping", true);
            
            isStopping = true;
        }
            //pure stopping when both paddles are in the water, no backward turning (prevented by last condition)
        if (paddleObjectLinks.GetComponent<inWater>().touchesWater && paddleObjectLinks.GetComponent<inWater>().touchesWater&& isStopping)
        {
            forwardVel=0;
            
        }
            //stops the stopping animation
        if (!Input.GetKey(KeyCode.X) && isStopping)
        {
            isStopping = false;
            animatorRechts.SetBool("isStopping", false);
            animatorLinks.SetBool("isStopping", false);
        }*/