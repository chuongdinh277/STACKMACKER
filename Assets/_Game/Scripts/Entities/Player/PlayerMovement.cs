
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float boostSpeed = 20f;
    private Transform chestTransform;
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
    
    private bool  hasTriggeredWin = false;
    private Direct currentDirect = Direct.None;




    private void Start()
    {
        playerStack = GetComponent<PlayerStack>();
        anim = GetComponentInChildren<Animator>();

        moveState = new PlayerMoveState(anim);
        winState = new PlayerWinState(anim);
        
        OnInit();
    }

    public void OnInit()
    {
        isMoving = false; 
        moveVec = Vector3.zero; 
        targetPos = transform.position;
        hasTriggeredWin = false;
        currentDirect = Direct.None;

        transform.rotation = Quaternion.identity;

        ChangePlayerState(moveState);
        mousePosDown = Input.mousePosition;
        Invoke(nameof(ExecuteInit), 0.1f);

       

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
    public void TriggerWinEffects()
    {
        hasTriggeredWin = true;

        if (playerStack != null) playerStack.ClearStack();

        GameObject lihuaObj = GameObject.Find("lihua");
        if (lihuaObj != null)
        {
            foreach (ParticleSystem ps in lihuaObj.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }
    }
    public void StopAtBridge(Vector3 bridgePos)
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
                    currentDirect = diff.x > 0 ? Direct.Right : Direct.Left;
                }
                else
                {
                    currentDirect = diff.y > 0 ? Direct.Forward : Direct.Back;
                }
                moveVec = GetVectorFromDirect(currentDirect);

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
    
    private void ExecuteInit()
    {
         GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
        
        if (startPoint != null)
        {
            
            Renderer brickRenderer = startPoint.GetComponentInChildren<Renderer>();
            if (brickRenderer != null)
            {
                Vector3 realCenter = brickRenderer.bounds.center;
                float topY = brickRenderer.bounds.max.y;
                Vector3 spawnPos = new Vector3(realCenter.x, topY, realCenter.z);
            
                transform.position = spawnPos;
                
                targetPos = spawnPos;
                
            }
        }
        else
        {
            Debug.Log("Null");
        }
    }
    private void ExecuteMove()
    {
        RaycastHit hit;
        bool isOverBridge = false;
        
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1f))
        {
            if (hit.collider.CompareTag("BridgeStep")) isOverBridge = true;

            if (hit.collider.CompareTag("Win") && !hasTriggeredWin)
            {
                TriggerWinEffects();
            }
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
            
            if (hasTriggeredWin)
            {
                FinalWinCelebration(); 
            }
            else
            {
                StopMoving();
            }
        }
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

   
    private Vector3 FindNextStopPoint(Vector3 currentCheckPos, Vector3 direction)
    {
        float maxDistance = 100f;
        RaycastHit[] hits = Physics.RaycastAll(currentCheckPos + Vector3.up * 0.5f, direction, maxDistance);
        
        RaycastHit closestStopHit = new RaycastHit();
        RaycastHit furthestBridgeHit = new RaycastHit(); 
        RaycastHit winPosHit = new RaycastHit();

        float minDistance = float.MaxValue;
        float maxBridgeDist = -1f;
        
        bool foundStop = false;
        bool foundBridge = false;
        bool foundWin = false;
        bool foundChest = false;
        RaycastHit chestHit = new RaycastHit();
        foreach (RaycastHit hit in hits)
        {
            if (hit.distance < 0.5f) continue;

            bool isObstacle = ((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0;
            bool isCorner = hit.collider.CompareTag("BrickCorner");
            bool isBridge = hit.collider.CompareTag("BridgeStep");
            bool mustStopAtBridge = isBridge && playerStack.CollectedBrickCount <= 0;
            bool isWinPos = hit.collider.CompareTag("Win");
            bool isChest = hit.collider.CompareTag("Chest");

            if (isChest)
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    chestHit = hit;
                    foundChest = true;
                }
                continue; 
            }
            if (isWinPos)
            {
                winPosHit = hit;
                foundWin = true;
                continue;
            }
            if (isObstacle || isCorner || mustStopAtBridge)
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

        if (foundChest)
        {
            chestTransform = chestHit.collider.transform;
            Vector3 chestPos = chestHit.collider.transform.position;
            Vector3 stopPos = new Vector3(chestPos.x - direction.x * 0.8f, currentCheckPos.y, chestPos.z - direction.z * 0.8f);
            return stopPos;      
        }

        if (foundStop)
        {
            Vector3 hitPos = closestStopHit.collider.transform.position;

            bool hitIsObstacle = ((1 << closestStopHit.collider.gameObject.layer) & obstacleLayer) != 0;
            
            Vector3 result;
            if (hitIsObstacle) 
            {
                result = new Vector3(hitPos.x - direction.x, currentCheckPos.y, hitPos.z - direction.z);
            }
            else
            {
                result = new Vector3(hitPos.x, currentCheckPos.y, hitPos.z);
            }
            return result;
        }
        if (foundBridge)
        {
            Vector3 bridgePos = furthestBridgeHit.collider.transform.position;
            
            Vector3 result = new Vector3(bridgePos.x, currentCheckPos.y, bridgePos.z);
            return result;
        }
        return currentCheckPos;
    }
    private void FinalWinCelebration()
    {
        isMoving = false;
        moveVec = Vector3.zero;
        hasTriggeredWin = false;

        if (chestTransform != null)
        {
            Vector3 lookDir = (chestTransform.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        ChangePlayerState(winState);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.NextLevel();
        }
    }

    private Vector3 GetVectorFromDirect(Direct dir)
    {
        switch(dir)
        {
            case Direct.Forward : return Vector3.forward;
            case Direct.Right : return Vector3.right;
            case Direct.Left : return Vector3.left;
            case Direct.Back : return Vector3.back;
            default: return Vector3.zero;
        }
    }

   
}