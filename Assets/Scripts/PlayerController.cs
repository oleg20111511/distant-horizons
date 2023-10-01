using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Enums
    public enum PlayerState {
        Idle,
        Moving,
        Fighting
    }

    // Events
    public event System.Action<PlayerState> StateChanged;


    // Inspector fields
    [SerializeField] private float moveDuration;
    [SerializeField] private SkillCheckController skillCheckController;

    // Private fields
    private PlayerControls actions;
    private CellController currentCell;
    private PlayerState _currentState = PlayerState.Idle;
    private Vector2 scheduledMove = Vector2.zero;

    // Properties
    public PlayerState State
    {
        get { return _currentState; }
        set {
            _currentState = value;
            StateChanged?.Invoke(value);
        }
    }
    public CellController CurrentCell => currentCell;


    private void Awake()
    {
        actions = new PlayerControls();
        actions.gameplay.move.performed += OnMoveAction;
    }


    // Start is called before the first frame update
    private void Start()
    {
        currentCell = FindCurrentCell();
        TowerManager.Instance.TowerSpawned += OnTowerSpawn;
        skillCheckController.SkillCheckPassed += OnSkillCheckPassed;
    }


    private void OnEnable()
    {
        actions.gameplay.Enable();
        StateChanged += StateChangedHandler;
    }


    private void OnDisable()
    {
        actions.gameplay.Disable();
        StateChanged -= StateChangedHandler;
    }


    private void StateChangedHandler(PlayerState newState)
    {
        if (newState == PlayerState.Idle)
        {
            if (scheduledMove != Vector2.zero)
            {
                MoveInDirection(scheduledMove);
                scheduledMove = Vector2.zero;
            }
        }
    }


    private CellController FindCurrentCell()
    {
        Collider2D cell = Physics2D.OverlapPoint(transform.position, GameManager.Instance.CellsLayer);
        return cell.gameObject.GetComponent<CellController>();
    }


    private void OnTowerSpawn(TowerController tower)
    {
        if (!currentCell.Ring.IsAccessible && State != PlayerState.Fighting)
        {
            MoveToSafeZone();
        }
    }


    private void OnSkillCheckPassed()
    {
        currentCell.CellContent = null;
        GameManager.Instance.Money += GameManager.Instance.TowerDestroyReward;

        if (!currentCell.Ring.IsAccessible)
        {
            MoveToSafeZone();
        }
        else
        {
            State = PlayerState.Idle;
        }
    }


    private void OnMoveAction(InputAction.CallbackContext context)
    {
        Vector2 moveVector = actions.gameplay.move.ReadValue<Vector2>();

        // Allow prefiring moves before character finishes movement
        if (State == PlayerState.Moving && scheduledMove == Vector2.zero)
        {
            scheduledMove = moveVector;
            return;
        }
        else if (State != PlayerState.Idle)
        {
            return;
        }

        MoveInDirection(moveVector);
    }


    private void MoveInDirection(Vector2 direction)
    {
        CellController targetCell = currentCell.GetNeighbour(direction);
        if (targetCell == null) {
            return;
        }

        MoveToCell(targetCell);
    }


    private void MoveToSafeZone()
    {
        Vector2 rayDirection = GameManager.Instance.transform.position - transform.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, GameManager.Instance.CellsLayer);
        foreach (RaycastHit2D hit in hits)
        {
            CellController cell = hit.collider.GetComponent<CellController>();
            if (cell.Ring.IsAccessible)
            {
                MoveToCell(cell);
                return;
            }
        }
    }


    private void MoveToCell(CellController targetCell)
    {
        if (!targetCell.Ring.IsAccessible)
        {
            return;
        }

        State = PlayerState.Moving;
        currentCell = targetCell;
        transform.DOMove(currentCell.transform.position, moveDuration).SetEase(Ease.Linear).OnComplete(MovementStopHandler);
    }


    private void MovementStopHandler()
    {
        if (currentCell.CellContent != null && currentCell.CellContent.gameObject.GetComponent<TowerController>())
        {
            skillCheckController.gameObject.SetActive(true);
            skillCheckController.Initialize();
            State = PlayerState.Fighting;
        }
        else
        {
            State = PlayerState.Idle;
        }
    }
}
