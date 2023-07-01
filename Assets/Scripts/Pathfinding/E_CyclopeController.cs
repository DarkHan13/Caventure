using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_CyclopeController : MonoBehaviour, Destructable
{
    

    public State _state = State.Walking;

    [SerializeField] private float maxHealth = 20;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpFrequency = 1f;
    [SerializeField] private PlayerOneWayPlatform _playerOneWayPlatform;
    
    private float move;
    private float currentSpeed;
    private Vector2 movePosition;
    private bool isJump;
    private float _currentHealth;
    
    private List<PathNode> nodes;
    private int _currentIndexNode;
    private Rigidbody2D _rb;
    private GroundChecker _groundChecker;
    private float jumpTimer;
    private float currentJumpHeight;
    
    // for direction
    public bool isRight = true;
    
    // for animation
    private bool _isWalking;
    private bool _isJumping;
    private bool _isFalling;
    
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
        currentSpeed = speed;
        currentJumpHeight = jumpHeight;
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerOneWayPlatform = GetComponent<PlayerOneWayPlatform>();
        _groundChecker = GetComponentInChildren<GroundChecker>();
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        SetAnimationTriggers();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector2 horizontal = move * new Vector2(1f, 0);
        _rb.velocity = new Vector2(horizontal.x ,_rb.velocity.y);
    }

    void HandleMovement()
    {
        jumpTimer += Time.deltaTime;
        if (nodes != null && _state == State.Walking)
        {
            var offset = new Vector3(0.5f, 0.5f);
            Vector3 targetPosition = nodes[_currentIndexNode].GetPosition() + offset;
            float distance = Vector3.Distance(targetPosition, transform.position);
            if (distance > 0.7f)
            {
                Vector3 moveDir = (targetPosition - transform.position);
                if (moveDir.x > 0) move = currentSpeed;
                else if (moveDir.x < 0) move = -currentSpeed;
                else move = 0;
                
                if (move != 0)
                {
                    _isWalking = true;
                    if (move > 0.3f && !isRight)
                    {
                        isRight = true;
                        transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (move < -0.3f && isRight)
                    {
                        isRight = false;
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                }
                else _isWalking = false;
                
                if (moveDir.y > 0.9f && _groundChecker.isGrounded && jumpTimer >= jumpFrequency)
                {
                    Debug.Log(moveDir.y);
                    isJump = true;
                    jumpTimer = 0f;
                    if (moveDir.y < 1) currentJumpHeight = moveDir.y;
                    else currentJumpHeight = jumpHeight;
                } else if (moveDir.y < 0.5f && _playerOneWayPlatform._isReady) _playerOneWayPlatform.JumpDown();
            }
            else
            {
                _currentIndexNode++;
                if (_currentIndexNode >= nodes.Count)
                {
                    nodes = null;
                    _currentIndexNode = 0;
                }
            }
        }
        if (isJump) Jump();
        isJump = false;
    }

    [ContextMenu("Jump")]
    void Jump()
    {
        var canJump = _groundChecker.isGrounded;
        if (!canJump) return;
        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * _rb.gravityScale));
        _rb.AddForce(new Vector2(_rb.velocity.x, jumpForce), ForceMode2D.Impulse);
    }
    
    IEnumerator UpdatePath()
    {
        while (true)
        {
            Find();
            yield return new WaitForSeconds(1f);    
        }
    }

    [ContextMenu("FindPath")]
    void Find()
    {
        if (Pathfinding.instance == null) return;
        
        Vector3 pos = transform.position;

        Vector3 player = WorldManager.instance.player.transform.position;
        nodes = Pathfinding.instance.FindPathForPhysic(pos, player, jumpHeight);
        _currentIndexNode = 0;

    }
    
    void SetAnimationTriggers()
    {
        _animator.SetBool("isWalking", _isWalking);
        /*_animator.SetBool("isJumping", _isJumping);
        _animator.SetBool("isFalling", _isFalling);
        _animator.SetBool("isPickUp", _isPickUp);*/
    }
    
    

    private void OnDrawGizmos()
    {
        var offset = new Vector3(0.5f, 0.5f);

        if (Pathfinding.instance != null)
        {
            Gizmos.color = Color.white;
            /*for (int x = 0; x < Pathfinding.instance.grid.gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < Pathfinding.instance.grid.gridArray.GetLength(1); y++)
                {
                    var node = Pathfinding.instance.grid.GetValue(x, y);
                    if (node != null)
                    {
                        if (node.isFloor)
                        {
                            Gizmos.color = Color.blue;
                        }
                        else Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(node.GetPosition() + offset, Vector3.one);
                    }
                }
            }*/
        }
        
        if (nodes == null) return;
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            if (i == _currentIndexNode)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(nodes[i].GetPosition() + offset, 0.1f);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nodes[i].GetPosition() + offset, nodes[i + 1].GetPosition() + offset);
        }
    }

    public void TakeDamage(int damage, int direction = 0)
    {
        _currentHealth -= damage;

        
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            GetStunned();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void GetStunned()
    {
        move = 0;
        _animator.SetTrigger("damage");
        _isWalking = false;
        _state = State.Stunned;
    }

    private void SwitchState(State newState)
    {
        _state = newState;
    }
}

public enum State
{
    Idle,
    Walking,
    Stunned,
    Dead
}
