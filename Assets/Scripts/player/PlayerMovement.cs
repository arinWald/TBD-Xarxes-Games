using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    enum INPUTS
    {
        UNKNOWN = -1,
        HORIZONTAL,
        VERTICAL,
        SPACE,
        SHIFT
    }

    float horizontalInput = 0f;
    float verticalInput = 0f;
    bool spaceInput = false;
    bool spaceInputUp = false;
    bool shiftInput = false;

    GameObject player;

    [Header("Movement")]
    public float movementSpeed;

    [Header("Ball")]
    public GameObject ball;
    public float kickStrenght;
    public float kickMaxTime;

    private float kickTime;
    private bool ballPossesion = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Kick ball
        if (ballPossesion)
        {
            if (spaceInput && kickTime < kickMaxTime)
            {
                kickTime++;
            }
            else if (spaceInputUp)
            {
                float normTime = kickTime / kickMaxTime;
                float strenght = normTime * kickStrenght;

                ball.GetComponent<Rigidbody>().AddForce(new Vector3(player.transform.localEulerAngles.x * kickStrenght, 0f, player.transform.localEulerAngles.z * kickStrenght));
            }
            else
            {
                kickTime = 0;
            }
        }

        //Move Player
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        transform.position += movement * movementSpeed * Time.deltaTime; ;
        
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("horizontal");
        verticalInput = Input.GetAxisRaw("vertical");
        spaceInput = Input.GetKeyDown(KeyCode.Space);
        spaceInputUp = Input.GetKeyDown(KeyCode.Space);
        shiftInput = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
