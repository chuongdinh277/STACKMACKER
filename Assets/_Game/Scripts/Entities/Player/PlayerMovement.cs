

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float boostSpeed = 20f;

    private float currentSpeed;
    private Vector3 mousePosDown;
    private bool isMoving = false;
    private Vector3 moveVec;
    public Vector3 MoveVec => moveVec;
    public bool IsMoving => isMoving;
    private Vector3 targetPos;
    private Animator anim;
    private Istate currentState;
    private PlayerMoveState moveState;
    private PlayerWinState winState;

    private readonly float rayDistance = 0.7f;
    private readonly Vector3 rayOffset = Vector3.up * 0.5f;
    private PlayerStack playerStack;
    
    private void Start()
    {
        playerStack = GetComponent<PlayerStack>();
        anim = GetComponentInChildren<Animator>();

        moveState = new PlayerMoveState(anim);
        winState = new PlayerWinState(anim);
        ChangePlayerState(moveState);
        targetPos = transform.position;
        
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
    public void Redirect(Vector3 direction, Vector3 cornerPosition)
    {
        Vector3[] checkDirection = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

        foreach (Vector3 dir in checkDirection)
        {
            if (dir == -direction) continue;
            Vector3 potentialTarget = FindNextStopPoint(cornerPosition, dir);

            if (Vector3.Distance(cornerPosition, potentialTarget) > 0.1f)
            {
                moveVec = dir;
                targetPos = new Vector3(potentialTarget.x, transform.position.y, potentialTarget.z);
                isMoving = true;
                return;
            }
        }
        isMoving = false;
    }
    private void HandleInput()
    {
        if (isMoving) return;

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

                targetPos = FindNextStopPoint(transform.position, moveVec);
                targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                if (Vector3.Distance(transform.position, targetPos) > 0.1f)
                {
                    isMoving = true;
                    ChangePlayerState(moveState);
                }
            }
        }
    }

    private void ExecuteMove()
    {
        RaycastHit hit;
        bool isOverBridge = false;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1f))
        {
            if (hit.collider.CompareTag("BridgeStep")) isOverBridge = true;
        }
        if (isOverBridge && playerStack.CollectedBrickCount <= 0)
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            StopMoving();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            StopMoving();
        }
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
        float snapX = Mathf.Floor(transform.position.x * 2.0f) / 2.0f;
        float snapZ = Mathf.Floor(transform.position.z * 2.0f) / 2.0f;
        if (currentSpeed >= boostSpeed)
        {
            transform.position = new Vector3(snapX, transform.position.y, snapZ);
            return;
        }
        isMoving = false;
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
   private Vector3 FindNextStopPoint(Vector3 currentCheckPos, Vector3 direction)
    {
        float maxDistance = 100f;
        RaycastHit[] hits = Physics.RaycastAll(currentCheckPos + Vector3.up * 0.5f, direction, maxDistance);
        
        RaycastHit closestStopHit = new RaycastHit();
        RaycastHit furthestBridgeHit = new RaycastHit(); 
        
        float minDistance = float.MaxValue;
        float maxBridgeDist = -1f;
        
        bool foundStop = false;
        bool foundBridge = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance < 0.5f) continue;

            bool isObstacle = ((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0;
            bool isCorner = hit.collider.CompareTag("BrickCorner");
            bool isBridge = hit.collider.CompareTag("BridgeStep");
            bool mustStopAtBridge = isBridge && playerStack.CollectedBrickCount <= 0;
            bool isWinPos = hit.collider.CompareTag("Win");

            if (isWinPos) Debug.Log("đã tìm thấy win");
            if (isObstacle || isCorner || isWinPos || mustStopAtBridge)
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestStopHit = hit;
                    foundStop = true;
                }
            }

            if (isBridge && playerStack.CollectedBrickCount > 0)
            {
                if (hit.distance > maxBridgeDist)
                {
                    maxBridgeDist = hit.distance;
                    furthestBridgeHit = hit;
                    foundBridge = true;
                }
            }
        }

        if (foundStop)
        {
            Vector3 hitPos = closestStopHit.collider.transform.position;

            bool hitIsObstacle = ((1 << closestStopHit.collider.gameObject.layer) & obstacleLayer) != 0;
            
            if (hitIsObstacle) 
            {
                return new Vector3(hitPos.x - direction.x, currentCheckPos.y, hitPos.z - direction.z);
            }
            return new Vector3(hitPos.x, currentCheckPos.y, hitPos.z);
        }
        if (foundBridge)
        {
            Vector3 bridgePos = furthestBridgeHit.collider.transform.position;
            return new Vector3(bridgePos.x, currentCheckPos.y, bridgePos.z);
        }
        return currentCheckPos;
    }
}