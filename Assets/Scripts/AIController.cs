using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float forwardSpeed;
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    Transform BallTransform;
    [SerializeField]
    Transform PlayerTailPos;
    [SerializeField]
    Transform BallParent;

    bool isNearEdge;
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

        if (Physics.Raycast(CarFront.position, -transform.up * 5, out hit))
        {
            if (hit.collider.CompareTag("Ground") && isNearEdge)
            {
                isNearEdge = false;
            }
            else if (!isNearEdge)
            {
                isNearEdge = true;
                transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Random.Range(-2, 3) * 90, transform.eulerAngles.z), 3f);
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            forwardSpeed = 0;
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
            transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Random.Range(-2, 3) * 90, transform.eulerAngles.z), 3f);
        }
    }
}
