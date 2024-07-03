using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float walkSpeed = 2.0f;
    [SerializeField] float crouchSpeed = 1.0f;
    [SerializeField] float climbSpeed = 2.0f;
    [SerializeField] GameObject stone;
    [SerializeField] Transform launcher;
    [SerializeField] float levelLoadDelay = 0.5f;
    [SerializeField] float loseReloadDelay = 5f;
    [SerializeField] int keyCount = 1;
    [SerializeField] int keyOwned = 0;
    float gravityScaleAtStart;
    Animator playerAnimator;
    Vector2 moveInput;
    Rigidbody2D rgbd2D;
    
    PlayerInput playerInput;
    CapsuleCollider2D playerCapsuleCollider;
    bool isAlive;

    bool isCrouchedPressed;
    bool isMoving;
    bool canThrow;
    [SerializeField] float cooldownTime = 5;
    float nextFireTime = 0;
    void Start()
    {   
        isAlive = true;
        canThrow = true;
        rgbd2D = GetComponent<Rigidbody2D>();   
        playerAnimator = GetComponent<Animator>(); 
        isCrouchedPressed = false;
        playerCapsuleCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = rgbd2D.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isAlive){return;}
        if (Time.time > nextFireTime){
            // nextFireTIme - Time.time
            canThrow = true;
        }
        if (isCrouchedPressed) {
            playerAnimator.SetBool("isCrouching", true);
            Crouch();
        }else{
            playerAnimator.SetBool("isCrouching", false);
            Movement();
        } 
        FlipSprite();   
        Climb();
        Lose();

    }

    void OnCrouch(InputAction.CallbackContext context){
        isCrouchedPressed = context.ReadValueAsButton();
    }

    void OnMove(InputValue value){
        if (!isAlive){return;}
        moveInput = value.Get<Vector2>();
        // Debug.Log(moveInput);
    }

    void OnCrouch(InputValue value) {
        if (!isAlive){return;}
        if(value.isPressed && !isCrouchedPressed && !isMoving){
            isCrouchedPressed = true;
            playerCapsuleCollider.size = new Vector2(playerCapsuleCollider.size.x, 2.55f);
            playerCapsuleCollider.offset = new Vector2(playerCapsuleCollider.offset.x, -0.53f);
        }
        else if (value.isPressed && isCrouchedPressed && !isMoving){
            isCrouchedPressed = false;
            playerCapsuleCollider.size = new Vector2(playerCapsuleCollider.size.x, 3.59f);
            playerCapsuleCollider.offset = new Vector2(playerCapsuleCollider.offset.x, -0.047f);
        }
    }

    void OnFire(InputValue value){
        if(!isAlive || !canThrow){return;}
        Instantiate(stone, launcher.position, transform.rotation);
        playerAnimator.SetTrigger("Throw");
        canThrow = false;
        nextFireTime = Time.time + cooldownTime;
        FindObjectOfType<GameSession>().ReduceCooldown(cooldownTime);
    }

    void OnInteract(InputValue value){
        if(!isAlive){return;}
        playerAnimator.SetTrigger("Interact");
        if (playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Doors")) && keyOwned > 0){
            keyOwned -= keyCount;
            FindObjectOfType<GameSession>().RemoveKey(keyCount);
            StartCoroutine(LoadNextLevel());
        }
        else if (playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Key"))){
            keyOwned += keyCount;
            FindObjectOfType<GameSession>().AddToKey(keyCount);
            Destroy(GameObject.FindGameObjectWithTag("Key"));
        }
    }

    IEnumerator LoadNextLevel(){
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex +1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings){
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void Climb(){
        if(!isAlive){return;}
        if (!playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))){
            rgbd2D.gravityScale = gravityScaleAtStart;    
            playerAnimator.SetBool("isClimbing", false);
            return;
        }
        Vector2 climbVelocity = new Vector2 (rgbd2D.velocity.x, moveInput.y * climbSpeed);
        rgbd2D.velocity = climbVelocity;
        rgbd2D.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(rgbd2D.velocity.y) > Mathf.Epsilon;
        playerAnimator.SetBool("isMoving", false);
        playerAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Crouch(){
        if (isCrouchedPressed){
            Vector2 playerVelocity = new Vector2 (moveInput.x * crouchSpeed , rgbd2D.velocity.y);
            rgbd2D.velocity = playerVelocity;

            bool playerHasHorizontalSpeed = Mathf.Abs(rgbd2D.velocity.x) > Mathf.Epsilon;
            if(playerHasHorizontalSpeed && isCrouchedPressed) {
                isMoving = true;
                playerAnimator.SetBool("isCrouchingAndMoving", true);
            }
            else if (!playerHasHorizontalSpeed && isCrouchedPressed){
                isMoving = false;
                playerAnimator.SetBool("isCrouchingAndMoving", false);
            }
        }
    }

    void Movement(){
        Vector2 playerVelocity = new Vector2 (moveInput.x * walkSpeed , rgbd2D.velocity.y);
        rgbd2D.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rgbd2D.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed) {
            isMoving = true;
            playerAnimator.SetBool("isMoving", true);
        }
        else{
            isMoving = false;
            playerAnimator.SetBool("isMoving", false);
        }
    }
    
    void FlipSprite(){
        bool playerHasHorizontalSpeed = Mathf.Abs(rgbd2D.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed){
            transform.localScale = new Vector2 (-Mathf.Sign(rgbd2D.velocity.x), 1f);
        }
    }

    void Lose(){
        if(!playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Guard"))) {return;}
        Debug.Log("Game over!!!!!!");
        rgbd2D.velocity = new Vector2(0f,0f);
        playerAnimator.SetTrigger("isCaptured");
        isAlive = false;

        StartCoroutine(LoadSameLevel());
        // FindObjectsOfType<GameSession>().ProcessPlayerDeath();
    }

    IEnumerator LoadSameLevel(){
        yield return new WaitForSecondsRealtime(loseReloadDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 

}
