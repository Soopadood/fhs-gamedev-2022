using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float _runSpeed = 5;
    //[SerializeField] private float baseRunSpeed = 3;
    //[SerializeField] private float sprintSpeedMultiplier = 2f;

    [Header("Dashing")]
    private bool _canDash = false;
    [SerializeField] private float _dashPowerMultiplier = 1f;
    [SerializeField] private float _dashingTime = 0.3f;
    //private float _dashingCooldown = 1f;
    [SerializeField] TrailRenderer _dashTrail;

    [Header("Jumping")]
    [SerializeField] private float _jumpHeight = 5;
    [SerializeField] private float _groundedDist = 1;
    [SerializeField] private float _maxVertSpeed = 20;
    private bool _canJump = true;
    private bool _isGrounded = false;
    [Header("Walljumping")]
    [Tooltip("Distance from side of player")] [SerializeField] private float _wallJumpDist = 1f;
    [Tooltip("Side power of walljump")][SerializeField] private float _wallJumpDistSide = 2f;
    private float _wallJumpSideDistNow = 0f;
    private float _wallJumpTime;
    [Header("Collision")]
    [Tooltip("Editor ground layer")][SerializeField] private LayerMask _ground;
    private int _groundLayer; // log base 2 of ground (^) actually used
    private Rigidbody2D _rigidBody;
    private WaterLevel _waterLevel;


    [SerializeField] private Transform _camera;
    [SerializeField] private PlayerAnimation _playerAnimation;
    void Start()
    {
        _groundLayer = (int)Mathf.Log(_ground, 2);
        _rigidBody = GetComponent<Rigidbody2D>();
        _waterLevel = GetComponent<WaterLevel>();
        _playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if (canDash && Input.GetKey(KeyCode.Space))
            StartCoroutine(Dash());
        RaycastHit2D groundedRaycast = Physics2D.Raycast(transform.position, -Vector2.up, groundedDist);//grounded raycast to detect if on the ground
        isGrounded = groundedRaycast.collider != null;
        if (isGrounded) canDash = false;
        GroundJump();
        WallJump();

        // sprinting
        //runSpeed = baseRunSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeedMultiplier : 1);

        // Dashing
        if (Input.GetKey(KeyCode.LeftShift) && _canDash)
            StartCoroutine(Dash());

        //set velocity of rigidbody based on horizontal input, add the wall jump momentum, clamp the vertical speed for falling and wall accelerating upward
        _rigidBody.velocity = new Vector2((_runSpeed * Input.GetAxisRaw("Horizontal")) + _wallJumpSideDistNow, Mathf.Clamp(_rigidBody.velocity.y, -_maxVertSpeed, _maxVertSpeed));
        Animation();

    }

    private void Animation()
    {
        if (!_isGrounded)
            _playerAnimation.State = AnimationState.Falling;
        else _playerAnimation.State = (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f) ? AnimationState.Running : AnimationState.Idle;
    }

    private void GroundJump()
    {
        RaycastHit2D groundedRaycast = Physics2D.Raycast(transform.position, -Vector2.up, _groundedDist);//grounded raycast to detect if on the ground
        _isGrounded = groundedRaycast.collider != null;

        if (((Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0) || Input.GetKeyDown(KeyCode.Space)) && _isGrounded && _canJump == true) //if vertical input, is grounded, and doesn't have jump cooldown
        {
            canDash = true;
            StartCoroutine(JumpDelay());//Small cooldown for jump
            _rigidBody.velocity += Vector2.up * _jumpHeight;
            // camera.GetComponent<CameraShake>().cameraShake();
        }
    }

    private void WallJump()
    {
        var left = Physics2D.Raycast(transform.position, Vector2.left, _wallJumpDist, _ground);
        var right = Physics2D.Raycast(transform.position, Vector2.right, _wallJumpDist, _ground);

        if ((left.collider != null || right.collider != null) && /*Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 &&*/ (Input.GetButtonDown("Vertical") || Input.GetKeyDown(KeyCode.Space)) && _canJump)
        {
            StartCoroutine(JumpDelay());
            //rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0); //
            _rigidBody.velocity = Vector2.up * _jumpHeight;
            _wallJumpSideDistNow = left.collider != null ? (_wallJumpDistSide) : (_wallJumpDistSide * -1);
            _wallJumpTime = Time.time;
            //Debug.Log(wallJumpSideDistNow);
        }

        float sidePower = Mathf.Exp(-4 * (Time.time - _wallJumpTime));
        if (sidePower < 0.2)
            sidePower = 0;
        if (_isGrounded) //TODO make this fix better for jumping up the same wall instead of back and forth between two
            sidePower = 0;
        if (_wallJumpSideDistNow > 0)
            _wallJumpSideDistNow = _wallJumpDistSide * sidePower;
        if (_wallJumpSideDistNow < 0)
            _wallJumpSideDistNow = _wallJumpDistSide * sidePower * -1;
    }

    IEnumerator Dash()//sets gravity to 0, turns on dash trail, adds horizontal velocity in the same way as wall jump
    {
        _canDash = false;
        
        float originalGravity = _rigidBody.gravityScale;
        _rigidBody.gravityScale = 0f;
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        _wallJumpSideDistNow = _rigidBody.velocity.x > 0 ? (_wallJumpDistSide) * _dashPowerMultiplier : (_dashPowerMultiplier * _wallJumpDistSide * -1);
        _wallJumpTime = Time.time;
        _dashTrail.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        _dashTrail.emitting = false;
        _rigidBody.gravityScale = originalGravity;

        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
    }

    IEnumerator JumpDelay() //small cooldown for jump
    {
        _canJump = false;
        yield return new WaitForSeconds(0.1f);
        _canJump = true;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, wallJumpDist);
        //Gizmos.DrawWireSphere(transform.position, jumpDistance); 
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + _wallJumpDist, transform.position.y)); //I've never done gizmos before but I made them
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - _wallJumpDist, transform.position.y)); //lines instead of circles I thought it might be good
        Gizmos.DrawLine(transform.position, -Vector2.up * _groundedDist);
    }
}
