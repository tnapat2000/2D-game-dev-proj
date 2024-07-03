using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float horizontalSpeed = 5.0f;
    [SerializeField] float verticalSpeed = 3.0f;
    Rigidbody2D rgbd;
    PlayerMovement player;
    float xSpeed;
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = -player.transform.localScale.x * horizontalSpeed;
        rgbd.velocity = new Vector2(xSpeed, verticalSpeed);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag != "Guard") {return;}
        // Debug.Log(rgbd.velocity);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other){
        // Debug.Log(rgbd.velocity);
        Destroy(gameObject);
    }
}
