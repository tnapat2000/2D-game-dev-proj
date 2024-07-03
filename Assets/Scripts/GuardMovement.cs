using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D rgbd2D;
    Animator guardAnimator;
    BoxCollider2D guardBoxCollider;
    // Start is called before the first frame update
    void Start()
    {
        rgbd2D = GetComponent<Rigidbody2D>();
        transform.localScale = new Vector2(-(Mathf.Sign(rgbd2D.velocity.x)),1f);
        guardAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {   
        rgbd2D.velocity = new Vector2 (moveSpeed, 0f);
    }
    
    void OnTriggerExit2D(Collider2D other){
        if(other.tag != "Enemy Confiner"){return;}
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing(){
        transform.localScale = new Vector2((Mathf.Sign(rgbd2D.velocity.x)),1f);
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Stone"){ 
            Rigidbody2D stoneLocation = other.GetComponent<Rigidbody2D>();
            Vector2 pos = stoneLocation.velocity;

            moveSpeed = -Mathf.Sign(pos.x);
            // Debug.Log(pos.x);
            transform.localScale = new Vector2((Mathf.Sign(pos.x)),1f);
        }

        if(other.tag != "Player"){return;}
        // Debug.Log("Stop There!!!!!!");
        moveSpeed = 0;
        guardAnimator.SetTrigger("isAngry");
    }

    
}
