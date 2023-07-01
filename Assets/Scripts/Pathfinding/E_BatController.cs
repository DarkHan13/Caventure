using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_BatController : MonoBehaviour, Destructable
{
    private List<PathNode> nodes;
    private int _currentIndexNode;
    [SerializeField] private int maxHealth = 7;

    private int _currentHealth;

    private Animator _animator;
    
    
    // for direction
    public bool isRight = true;
    
    public State _state = State.Walking;
    
    // for animation
    private bool _isFlying;
    private bool _isFalling;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        SetAnimationTriggers();
    }

    void HandleMovement()
    {
        if (nodes != null)
        {
            var offset = new Vector3(0.5f, 0.5f);
            Vector3 targetPosition = nodes[_currentIndexNode].GetPosition() + offset;
            float distance = Vector3.Distance(targetPosition, transform.position);
            if (distance > 0.5f)
            {
                _isFlying = true;
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                if (moveDir.x > 0.3f && !isRight)
                {
                    isRight = true;
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (moveDir.x < -0.3f && isRight)
                {
                    isRight = false;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                transform.position += moveDir * (5f * Time.deltaTime);
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
        else
        {
            _isFlying = false;
        } 
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
        nodes = Pathfinding.instance.FindPath(pos, player);
        _currentIndexNode = 0;

    }
    
    public void TakeDamage(int damage, int direction = 0)
    {
        _currentHealth -= damage;

        
        if (_currentHealth <= 0)
        {
            _state = State.Dead;
            _animator.SetTrigger("death");
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
        _animator.SetTrigger("damage");
        _isFlying = false;
        _state = State.Stunned;
    }
    
    private void SwitchState(State newState)
    {
        _state = newState;
    }
    
    void SetAnimationTriggers()
    {
        _animator.SetBool("isFlying", _isFlying);
//        _animator.SetBool("isFalling", _isFalling);
    }

    /*private void OnDrawGizmos()
    {
        var offset = new Vector3(0.5f, 0.5f);

        if (Pathfinding.instance != null)
        {
            Gizmos.color = Color.white;
            for (int x = 0; x < Pathfinding.instance.grid.gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < Pathfinding.instance.grid.gridArray.GetLength(1); y++)
                {
                    var node = Pathfinding.instance.grid.GetValue(x, y);
                    if (node != null)
                    {
                        Gizmos.DrawWireCube(node.GetPosition() + offset, Vector3.one);
                    }
                }
            }
        }
        if (nodes == null) return;
        foreach (var pathNode in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pathNode.GetPosition() + offset, Vector3.one);
        }
    }*/
}
