using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class AnimalController : MonoBehaviour
{
    private enum AnimalState
    {
        Idle = 0,
        Patrol = 1,
        Eat = 2,
    }

    [SerializeField]private SpriteRenderer spriteRenderer;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float idleTime = 2f;


    private AnimalState currentState = AnimalState.Idle;
    private Vector3 startPostion;
    private float idleDuration;
    private Vector3 patrolTarget;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        startPostion = transform.position;
        idleDuration = idleTime;
        cam = Camera.main;
        SetPatrolTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == AnimalState.Idle)
        {
            HandleIdle();
        }
        else if(currentState == AnimalState.Patrol)
        {
            HandlePatrol();
        }
        else if(currentState == AnimalState.Eat)
        {
            HandleEat();
        }
    }

    private void HandleIdle()
    {
        idleDuration -= Time.deltaTime;
        if (idleDuration <= 0f)
        {
            currentState = AnimalState.Patrol;
            SetPatrolTarget();
        }
    }
    private void HandlePatrol()
    {
        if (transform.position.x > patrolTarget.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position,patrolTarget) < 0.1f)
        {
            currentState = AnimalState.Idle;
            idleDuration = idleTime;
        }
    }

    private void HandleEat()
    {

    }
    private void SetPatrolTarget()
    {
        float newX = GameManager.Instance.GetRandomCameraPosition();

        patrolTarget = new Vector3(newX, startPostion.y, startPostion.z);
    }

    
}
