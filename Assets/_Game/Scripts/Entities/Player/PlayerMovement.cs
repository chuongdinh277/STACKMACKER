using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private LayerMask obstacleLayer;

    private Vector3 mousePosDown;
    private bool isMoving = false;
    private Vector3 moveVec;
    private Animator anim;
    private Istate currentState;
    private PlayerMoveState moveState;
    private PlayerWinState winState;

    private readonly float rayDistance = 0.7f;
    private readonly Vector3 rayOffset = Vector3.up * 0.5f;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        moveState = new PlayerMoveState(anim);
        winState = new PlayerWinState(anim);
        ChangePlayerState(moveState);
        Invoke(nameof(InitStartPosition), 0.1f);
    }
    private void Update()
    {
        if (currentState != null) currentState.OnExecute();
        HandleInput();
        if (isMoving)
        {
            ExecuteMove();
        }
    }

    
    public void ChangePlayerState(Istate newState)
    {
        
        if (currentState != null) 
        {
            if (currentState != newState) currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosDown = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && !isMoving) 
        {
            Vector3 diff = Input.mousePosition - mousePosDown;

            if (diff.magnitude > 50)
            {
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                {
                    moveVec = diff.x > 0 ? Vector3.right : Vector3.left;
                }
                else
                {
                    moveVec = diff.y > 0 ? Vector3.forward : Vector3.back;
                }
                isMoving = true;
                ChangePlayerState(moveState);
                
            }
        }
    }

    private void ExecuteMove()
    {
        if (CheckObstacle())
        {
            StopMoving();
            return;
        }

        transform.position += moveVec * speed * Time.deltaTime;
        
    }

    private bool CheckObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, moveVec, out hit, 0.7f, obstacleLayer))
        {
            return true;
        }
        return false;
    }

    private void StopMoving()
    {
        isMoving = false;
        
        float snapX = Mathf.Floor(transform.position.x) + 0.5f;
        float snapZ = Mathf.Floor(transform.position.z) + 0.5f;
        transform.position = new Vector3(snapX, transform.position.y, snapZ);

        moveVec = Vector3.zero;
        mousePosDown = Input.mousePosition; 
    }
    private void InitStartPosition()
    {
        GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
        if (startPoint != null)
        {
            Renderer brickRenderer = startPoint.GetComponentInChildren<Renderer>();
            if (brickRenderer != null)
            {
                Vector3 realCenter = brickRenderer.bounds.center;
                float topY = brickRenderer.bounds.max.y;
                
                transform.position = new Vector3(realCenter.x, topY, realCenter.z);
            }
        }
    }
}