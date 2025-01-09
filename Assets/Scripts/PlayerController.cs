using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 6f;
    private float direction = 0f;
    private Rigidbody2D player;

    public Animator playerAnimation;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    private bool isTouchingGround;

    public Vector3 respawnPoint;
    public GameObject fallDetector;

    public TMP_Text scoreText;
    [SerializeField] private HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
        }

        player = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        respawnPoint = transform.position;
        scoreText.text = "Score: " + Scoring.totalScore;
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        direction = Input.GetAxis("Horizontal");
        if(direction > 0f){
            player.velocity = new Vector2(direction * speed, player.velocity.y);
            transform.localScale = new Vector2(0.2f, 0.2f);
        }
        else if (direction < 0f){
            player.velocity = new Vector2(direction * speed, player.velocity.y);
            transform.localScale = new Vector2(-0.2f, 0.2f);
        }
        else{
            player.velocity = new Vector2(0, player.velocity.y);
        }

        if(Input.GetButtonDown("Jump") && isTouchingGround){
            player.velocity = new Vector2(player.velocity.x, jumpForce);
        }

        playerAnimation.SetFloat("Speed", Mathf.Abs(player.velocity.x));
        playerAnimation.SetBool("OnGround", isTouchingGround);  
    
        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);  
    
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "FallDetector"){
            transform.position = respawnPoint;
        }

        else if(collision.tag == "Crystal"){
            Scoring.totalScore += 1;
            scoreText.text = "Score: " + Scoring.totalScore;
            collision.gameObject.SetActive(false);
        }

        else if(collision.tag == "CheckPoint"){
            respawnPoint = transform.position;
        }
        
        else if(collision.tag == "NextLevel"){
            SceneManager.LoadScene(1);
            respawnPoint = transform.position;
        }
        else if(collision.tag == "PreviousLevel"){
            SceneManager.LoadScene(0);
            respawnPoint = transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision){
        if (collision.tag == "Damage" && healthBar != null)
        {
            healthBar.Damage(0.01f);
        }
    }
}