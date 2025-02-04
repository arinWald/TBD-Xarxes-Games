using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public float horizontalInput = 0f;
    public float verticalInput = 0f;
    public bool spaceInput = false;
    public bool spaceInputUp = false;
    public bool spaceInputDown = false;
    public bool shiftInputDown = false;
    public float rotationAngle = 0f;

    [Header("Movement")]
    public float movementSpeed;

    [Header("Dash")]
    public float dashImpulse;
    public float dashCD;
    private float dashCDTimer = 0f;
    public float dashDuration;
    private float dashDurationTimer = 0f;

    [Header("Ball")]
    public GameObject ball;
    public float kickStrenght;
    public float kickMaxTime;

    private Rigidbody ballRb;
    private Ball ball_c;
    private float kickTime;
    public bool ballPossesion = false;
    private GameObject ballSensor;

    [Header("Snatch")]
    public float snatchMaxTime;

    private float snatchTime;

    [HideInInspector]
    public bool snatch = false;

    public bool IAmRemote;


    public Slider kickSlider;
    public GameObject sliderTarget;

    public Rigidbody playerRB;

    private bool dashing = false;

    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();


        ballRb = ball.GetComponent<Rigidbody>();
        ball_c = ball.GetComponent<Ball>();

        ballSensor = transform.Find("BallSensor").gameObject;

        ballPossesion = false;

        //Handle
        kickSlider.minValue = 0;
        kickSlider.maxValue = kickMaxTime;
    }

    void Update()
    {
        if(!IAmRemote)
        {
            GetInput();
        }

        //Kick ball
        if (ballPossesion)
        {
            if (spaceInput)
            {

                if (kickTime < kickMaxTime) kickTime += Time.deltaTime;
            }
            else if (spaceInputUp)
            {
                //Debug.Log("SpaceUp: " + kickTime);
                float normTime = kickTime / kickMaxTime;
                float strenght = normTime * kickStrenght;

                ballRb.AddForce(gameObject.transform.forward * strenght);
                ball_c.ChangePossesion(null);
                ballPossesion = false;
            }
            else
            {
                kickTime = 0;
            }
        }


        //Move Player

        if (!dashing)
        {
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }

            //transform.position += movement * movementSpeed * Time.deltaTime;

            playerRB.velocity = movement * movementSpeed;
            //Debug.Log(movement * movementSpeed);
            //Debug.Log(playerRB.velocity);
        }

        //Player Orientation

        transform.rotation = Quaternion.Euler(new Vector3(0, -rotationAngle + 90, 0));

        //Snatch Ball

        if (spaceInputDown && ballPossesion == false && snatchTime <= 0)
        {

            snatch = true;
            snatchTime = snatchMaxTime;
        }

        if (snatch)
        {
            if (snatchTime <= 0)
            {
                snatch = false;
            }
            else
            {
                snatchTime--;
            }
        }

        // Dash input logic
        if (shiftInputDown && dashCDTimer <= 0f && dashDurationTimer <= 0f)
        {
            //Debug.Log("Dash");

            // Apply the dash force
            playerRB.AddForce(gameObject.transform.forward * dashImpulse, ForceMode.Impulse);

            // Reset the cooldown and dash duration timers
            dashCDTimer = dashCD;
            dashDurationTimer = dashDuration;

            // Set the dashing state
            dashing = true;
        }

        // Decrease cooldown timer
        if (dashCDTimer > 0)
        {
            dashCDTimer -= Time.deltaTime;
        }

        // Decrease dash duration timer and stop dashing when the duration is over
        if (dashDurationTimer > 0)
        {
            dashDurationTimer -= Time.deltaTime;
        }
        else
        {
            // Stop dashing once the duration ends
            if (dashing)
            {
                dashing = false;
                //Debug.Log("Dash Ended");
            }
        }

        //Update Slider

        kickSlider.value = kickTime;

        kickSlider.transform.LookAt(sliderTarget.transform);
        kickSlider.transform.position = new Vector3(kickSlider.transform.parent.transform.position.x, 0.2f, kickSlider.transform.parent.transform.position.z + 2);
        kickSlider.transform.rotation = sliderTarget.transform.rotation;

        bool hideSlider = spaceInput && ballPossesion ? true : false;
        kickSlider.transform.parent.gameObject.SetActive(hideSlider);


        //Debug force missmatch
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameObject.transform.position += new Vector3(2f, 0f, 2f);

        }//Debug force missmatch ball
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ball.gameObject.transform.position += new Vector3(-2f, 0f, -2f);
        }
    }

    void GetInput()
    {
        Vector3 mouse_pos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        
        rotationAngle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        spaceInput = Input.GetKey(KeyCode.Space);
        spaceInputUp = Input.GetKeyUp(KeyCode.Space);
        spaceInputDown = Input.GetKeyDown(KeyCode.Space);
        shiftInputDown = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
