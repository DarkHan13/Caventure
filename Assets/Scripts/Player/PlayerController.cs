using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private PlayerOneWayPlatform _playerOneWayPlatform;
    private float currentSpeed;
    private Vector2 movePosition;
    private bool isJump;
    private float height = 2;
    public bool isGrounded;
    private Hand hand;
    private bool hasKey = true;
    private float move = 0;
    
    // for animation
    private bool _isWalking;
    private bool _isJumping;
    private bool _isFalling;
    private bool _isPickUp;

    // for states
    private bool _isAttacking;
    
    // for direction
    public bool isRight = true;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GroundChecker _groundChecker;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        // Set Components
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _groundChecker = GetComponentInChildren<GroundChecker>();
        hand = GetComponentInChildren<Hand>();
        _playerOneWayPlatform = GetComponentInChildren<PlayerOneWayPlatform>();
        
        // Set parameters
        speed = GlobalObject.Instance.PlayerSettings.Speed;
        jumpHeight = GlobalObject.Instance.PlayerSettings.JumpHeight;
        hand.SetWeapon(GlobalObject.Instance.GetWeapon(), isRight);
        
        
        _prev = transform.position.y;
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler();
        PlayerAnimator();
    }


    private float _prev;

    private void InputHandler()
    {
        _isPickUp = false;
        isGrounded = _groundChecker.isGrounded;
        //move = Input.GetAxis("Horizontal") * speed;
        if (Input.GetKey(KeyCode.RightArrow)) move = currentSpeed;
        else if (Input.GetKey(KeyCode.LeftArrow)) move = -currentSpeed;
        else move = 0;
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.DownArrow) && _playerOneWayPlatform._isReady) _playerOneWayPlatform.JumpDown();
            else isJump = true;
        }

        if (transform.position.y < _prev)
        {
            if (_isJumping) _isJumping = false;
            _isFalling = true;
        }
        if (_rb.velocity.y >= 0) _isFalling = false;
        else _isFalling = true;
        
        
        _prev = transform.position.y;
        
        if (isJump && isGrounded)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * _rb.gravityScale));
            _rb.AddForce(new Vector2(_rb.velocity.x, jumpForce), ForceMode2D.Impulse);
            _isJumping = true;
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow)) PickUp();
        if (Input.GetKeyDown(KeyCode.D)) Attack();
        if (Input.GetKeyDown(KeyCode.E)) DoAction();
        isJump = false;
    }

    private void Attack()
    {
        if (!hand.isPickUp) return;
        _animator.SetBool("isAttack", true);
        hand.SpriteEnable(true);
        
        currentSpeed = 0.5f;
        _isAttacking = true;
    }

    public void StartAttack()
    {
        hand.ColliderEnable(true);
    }

    public void EndAttack()
    {
        hand.ColliderEnable(false);
        hand.SpriteEnable(false);
        _animator.SetBool("isAttack", false);
        
        currentSpeed = speed;
        _isAttacking = false;
    }
    

    private void PickUp()
    {
        _isPickUp = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 2);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Weapon weapon = hit.collider.gameObject.GetComponentInParent<Weapon>();
                if (weapon != null)
                {   
                    hand.SetWeapon(weapon.gameObject, isRight);
                    break;
                }
                
                Item key = hit.collider.gameObject.GetComponentInParent<Item>();
                if (key != null)
                {
                    hasKey = true;
                    key.gameObject.SetActive(false);
                    break;
                }
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }    
        }
    }

    private void DoAction()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1);
        foreach (Collider2D hit in hits)
        {
            if (hit != null)
            {
                Chest chest = hit.gameObject.GetComponent<Chest>();
                if (chest != null && hasKey)
                {
                    hasKey = false;
                    chest.Open();
                    break;
                }
            }    
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        if (move != 0)
        {
            _isWalking = true;
            if (!_isAttacking)
            {
                if (move > 0 && !isRight)
                {
                    isRight = true;
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (move < 0 && isRight)
                {
                    isRight = false;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        else _isWalking = false;
        Vector2 horizontal = move * new Vector2(1f, 0);
        _rb.velocity = new Vector2(horizontal.x ,_rb.velocity.y);
    }

    public void Save()
    {
        PlayerSettings playerSettings = new PlayerSettings(speed, jumpHeight, hand.GetWeaponGameObject());
        GlobalObject.Instance.SetPlayerSettings(playerSettings);
    }

    void PlayerAnimator()
    {
        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isJumping", _isJumping);
        _animator.SetBool("isFalling", _isFalling);
        _animator.SetBool("isPickUp", _isPickUp);
    }
}
