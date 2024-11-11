using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    //enum INPUTS
    //{
    //    UNKNOWN = -1,
    //    HORIZONTAL,
    //    VERTICAL,
    //    SPACE,
    //    SHIFT
    //}

    float horizontalInput = 0f;
    float verticalInput = 0f;
    bool spaceInput = false;
    bool spaceInputUp = false;
    bool shiftInput = false;

    [Header("Movement")]
    public float movementSpeed;

    [Header("Ball")]
    public GameObject ball;
    public float kickStrenght;
    public float kickMaxTime;

    private Rigidbody ballRb;
    private float kickTime;
    private bool ballPossesion = true;
    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        GetInput();

        //Kick ball
        if (ballPossesion)
        {
            if (spaceInput)
            {

                Debug.Log("Space: " + kickTime);

                if (kickTime < kickMaxTime) kickTime++;
            }
            else if (spaceInputUp)
            {
                Debug.Log("SpaceUp: " + kickTime);
                float normTime = kickTime / kickMaxTime;
                float strenght = normTime * kickStrenght;

                //ball.GetComponent<Rigidbody>().AddForce(new Vector3(player.transform.localEulerAngles.x * kickStrenght, 0f, player.transform.localEulerAngles.z * kickStrenght));
                ballRb.AddForce(gameObject.transform.forward * strenght);
                Debug.Log("kick: " + gameObject.transform.forward * strenght);
            }
            else
            {
                kickTime = 0;
            }
        }

        //Move Player
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        transform.position += movement * movementSpeed * Time.deltaTime; ;
        
    }

    void GetInput()
    {
        // 4 bytes
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // 1 bytes
        spaceInput = Input.GetKey(KeyCode.Space);
        spaceInputUp = Input.GetKeyUp(KeyCode.Space);
        shiftInput = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
