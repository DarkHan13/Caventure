using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatDummyController : MonoBehaviour, Destructable
{
    [SerializeField] private float maxhealth, knockbackSpeedX, knockbackSpeedY, _knockbackDuration;
    [SerializeField] private bool applyKnockback;
    private float _currentHealth, _knockbackStart;
    
    public GameObject hitParticle;


    public GameObject
        aliveGO,
        brokenTopGo,
        brokenBottomGo;

    private Rigidbody2D
        rbAlive,
        rbTop,
        rbBottom;

    private Animator _animator;

    private bool _onLeft, _isKnockback;

    private int direction = 1;
    
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        _currentHealth = maxhealth;
        

        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        rbTop = brokenTopGo.GetComponent<Rigidbody2D>();
        rbBottom = brokenBottomGo.GetComponent<Rigidbody2D>();
        
        brokenTopGo.SetActive(false);
        brokenTopGo.SetActive(false);
        
        Debug.Log(aliveGO.name);
        Debug.Log(brokenTopGo.name);
        Debug.Log(brokenBottomGo.name);
    }

    private void Update()
    {
        CheckKnockback();
    }

    public void TakeDamage(int damageAmount, int direction = -1)
    {
        _currentHealth -= damageAmount;

        Instantiate(hitParticle, aliveGO.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
        //0 is left, 1 is right
        if (direction == 1)
        {
            _onLeft = true;
            this.direction = 1;
        }
        else
        {
            _onLeft = false;
            this.direction = -1;
        }
        
        _animator.SetBool("onLeft", _onLeft);
        _animator.SetTrigger("damage");

        if (applyKnockback && _currentHealth > 0f)
        {
            Knockback();   
        } else Die();
        
    }
    
    

    private void Knockback()
    {
        _isKnockback = true;
        _knockbackStart = Time.time;
        rbAlive.velocity = new Vector2(knockbackSpeedX * direction, knockbackSpeedY);
    }

    private void CheckKnockback()
    {
        if (Time.time >= _knockbackStart + _knockbackDuration && _isKnockback)
        {
            _isKnockback = false;
            rbAlive.velocity = new Vector2(.0f, rbAlive.velocity.y);
        }
    }

    private void Die()
    {
        brokenTopGo.SetActive(true);
        brokenBottomGo.SetActive(true);

        var position = aliveGO.transform.position;
        brokenTopGo.transform.position = position;
        brokenBottomGo.transform.position = position;

        rbTop.velocity = new Vector2(knockbackSpeedX * direction, knockbackSpeedY);
        rbBottom.velocity = new Vector2(knockbackSpeedX * direction, knockbackSpeedY);
        rbTop.AddTorque(-direction, ForceMode2D.Impulse); 
        
        aliveGO.SetActive(false);

        
    }


}
