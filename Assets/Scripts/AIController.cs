using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIController : MonoBehaviour
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
    [SerializeField]
    bool isTurnBallActive;
    [SerializeField]
    Transform BallParent;

    bool isPlayerInRange, isNearEdge;
    public Transform CarFront;

    RaycastHit hit;

    public Canvas HeadCanvas;
    public Animator DriverAnimator;

    public GameObject AngryEmoji, LaughEmoji;

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

        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }

        //AI goes here
        if (isPlayerInRange)
        {
            transform.Rotate(transform.up, 1 * TurnSpeed);
        }

        if (Physics.Raycast(CarFront.position, -transform.up * 5, out hit))
        {
            if (hit.collider.CompareTag("Ground") && isNearEdge)
            {
                isNearEdge = false;
            }
            else if (!isNearEdge)
            {
                isNearEdge = true;
                transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Random.Range(-2, 3) * 90, transform.eulerAngles.z), 2f);
            }
        }

        if (isTurnBallActive)
        {
            BallParent.transform.RotateAround(transform.position, Vector3.up, 5 * Time.deltaTime);
        }
        HeadCanvas.transform.LookAt(Camera.main.transform, Vector3.up);
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
        if (collision.collider.CompareTag("Ball"))
        {
            if (collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 3)
            {
                Transform colisionObjectParent = collision.transform.parent.parent;
                rb.AddForce(((transform.position - collision.transform.position) + new Vector3(0, Random.Range(0f, 2.5f), 0)) * collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude, ForceMode.Impulse);
                Instantiate(AngryEmoji, HeadCanvas.transform.position, Quaternion.identity);
                if (colisionObjectParent.GetComponentInChildren<PlayerController>())
                {
                    PlayerController pc = colisionObjectParent.GetComponentInChildren<PlayerController>();
                    pc.DriverAnimator.SetTrigger("Cheer");
                    Instantiate(LaughEmoji, pc.HeadCanvas.transform.position, Quaternion.identity);
                }
                else if (colisionObjectParent.GetComponentInChildren<AIController>())
                {
                    AIController ai = colisionObjectParent.GetComponentInChildren<AIController>();
                    ai.DriverAnimator.SetTrigger("Cheer");
                    Instantiate(LaughEmoji,ai.HeadCanvas.transform.position, Quaternion.identity);
                }
            }
        }
        else if (collision.collider.CompareTag("PowerupBox"))
        {
            Instantiate(LaughEmoji, HeadCanvas.transform.position, Quaternion.identity);
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
        yield return new WaitForSeconds(3);
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
            GameManager.Instance.players.Remove(transform.parent.gameObject);
            GameManager.Instance.CheckPlayerCountForWin();
            transform.parent.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
