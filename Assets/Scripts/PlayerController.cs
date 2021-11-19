using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float forwardSpeed, TurnSpeed;
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    Transform BallTransform;
    [SerializeField]
    Transform PlayerTailPos;
    public VariableJoystick joystick;
    [SerializeField]
    bool isTurnBallActive;
    [SerializeField]
    Transform BallParent;
    public Canvas HeadCanvas;
    public Animator DriverAnimator;
    public GameObject TireRight, TireLeft;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        HeadCanvas.transform.LookAt(Camera.main.transform, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, PlayerTailPos.position);
        line.SetPosition(1, BallTransform.position);
        HeadCanvas.transform.LookAt(Camera.main.transform, Vector3.up);

        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            rb.velocity = Vector3.down;
            return;
        }

        if (joystick.Horizontal != 0)
        {
            transform.Rotate(transform.up, joystick.Horizontal * TurnSpeed * Time.deltaTime);

            TireLeft.transform.localEulerAngles += new Vector3(0, joystick.Horizontal * TurnSpeed * Time.deltaTime, 0);
            TireRight.transform.localEulerAngles += new Vector3(0, joystick.Horizontal * TurnSpeed * Time.deltaTime, 0);

            TireLeft.transform.localEulerAngles = new Vector3(TireLeft.transform.localEulerAngles.x, Mathf.Clamp(TireLeft.transform.localEulerAngles.y, 50, 130), TireLeft.transform.localEulerAngles.z);
            TireRight.transform.localEulerAngles = new Vector3(TireRight.transform.localEulerAngles.x, Mathf.Clamp(TireRight.transform.localEulerAngles.y, 50, 130), TireRight.transform.localEulerAngles.z);
        }
        else
        {
            TireRight.transform.DOLocalRotate(new Vector3(TireRight.transform.localEulerAngles.x, 90, TireRight.transform.localEulerAngles.z), .2f);
            TireLeft.transform.DOLocalRotate(new Vector3(TireLeft.transform.localEulerAngles.x, 90, TireLeft.transform.localEulerAngles.z), .2f);
        }

        if (isTurnBallActive)
        {
            BallParent.transform.RotateAround(transform.position, Vector3.up, 720 * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        rb.AddForce(transform.forward * forwardSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PowerupBox"))
        {
            switch (collision.gameObject.GetComponent<PowerUpBox>().myType)
            {
                case PowerUpBox.PowerUpType.TurnBall:
                    isTurnBallActive = true;
                    BallParent.GetComponentInChildren<TrailRenderer>().enabled = true;
                    BallParent.GetComponentInChildren<Rigidbody>().isKinematic = true;
                    BallParent.GetComponentInChildren<ConfigurableJoint>().connectedBody = null;
                    line.enabled = false;
                    break;
                default:

                    break;
            }
            Destroy(collision.gameObject);
            StartCoroutine(WaitAndClosePowerUp(collision.gameObject.GetComponent<PowerUpBox>().myType));
        }
    }

    IEnumerator WaitAndClosePowerUp(PowerUpBox.PowerUpType type)
    {
        yield return new WaitForSeconds(3f);
        switch (type)
        {
            case PowerUpBox.PowerUpType.TurnBall:
                isTurnBallActive = false;
                line.enabled = true;
                BallParent.GetComponentInChildren<TrailRenderer>().enabled = false;
                BallParent.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                BallParent.GetComponentInChildren<Rigidbody>().isKinematic = false;
                BallParent.GetComponentInChildren<ConfigurableJoint>().connectedBody = GetComponent<Rigidbody>();
                GetComponent<Rigidbody>().WakeUp();

                break;
            default:

                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            forwardSpeed = 0;
            TurnSpeed = 0;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            BallParent.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
            BallParent.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
            StartCoroutine(GameManager.Instance.WaitAndGameLose());
        }
    }
}
