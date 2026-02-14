using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;
    public Camera camera;
    public float flyingSpeed = 6f;
    public float maxFloatHeight = 6f;
    public float minFloatHeight = 6f;
    public Animator animator;
    public float currentHeight = 100f;
    private float xRotation;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHeight = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {


        if(Input.GetKey(KeyCode.W)){
            Fly();
        }
        else{
            DontFly();
        }
    }

    private void Fly(){
        Vector3 cameraForward = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
        transform.rotation = Quaternion.LookRotation(cameraForward);
        xRotation = camera.transform.rotation.eulerAngles.x;

        transform.Rotate(new Vector3(xRotation,0,0), Space.Self);//to make sure that the orientation of the player is correct
        animator.SetBool("isFlying", true);
        Vector3 forward = camera.transform.forward;
        Vector3 flyDirection = forward.normalized;

        transform.position += flyDirection * flyingSpeed * Time.deltaTime;
    }

    private void DontFly(){
        animator.SetBool("isFlying", false);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        



    }
}
