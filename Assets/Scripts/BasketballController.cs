using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasketballController : MonoBehaviour
{
    public float MoveSpeed = 10;
    public GameObject BallPrefab;
    public Transform Arms;
    public Transform PosOverHead;
    public Transform PosDribble;
    public Transform Target;
    public AudioSource src;
    public AudioClip sfx1;
    public Text scoreText;
    public Text timerText;

    bool isBallInHands = true;
    bool isBallFlying = false;
    bool isCountdownActive = false;
    float ballThrowDuration = 0.7f;
    float t = 0;
    int score = 0;
    float timer = 20f;
    float countdownTimer = 5f;
    bool isGameOver = false;
    GameObject currentBall;
    Vector3 ballSpawnPosition;

    void Start()
    {
        GetRandomSpawnPosition();
        SpawnNewBall();
        UpdateTimerText();
    }

    void Update()
    {
        if (isGameOver)
            return;

        HandlePlayerMovement();

        if (isBallInHands)
            HandleBallInHands();
        else if (isBallFlying)
            HandleBallInAir();

        UpdateTimer();

        if (isCountdownActive)
            UpdateCountdownTimer();
    }

    void HandlePlayerMovement()
    {
        // Player movement based on input
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);
    }

    void HandleBallInHands()
    {
        // Ball handling when it is in the player's hands
        if (Input.GetKey(KeyCode.Space))
        {
            // Ball held over the head
            currentBall.transform.position = PosOverHead.position;
            Arms.localEulerAngles = Vector3.right * 180; // Arms above your head
            transform.LookAt(Target.parent.position);
        }
        else
        {
            // Dribbling the ball
            currentBall.transform.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5)); // Bouncing ball effect
            Arms.localEulerAngles = Vector3.right * 0; // Arms faced towards the ground
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Release the ball and start the throw
            isBallInHands = false;
            isBallFlying = true;
            t = 0;
            isCountdownActive = false;
        }
    }

    void HandleBallInAir()
    {
        // Ball behavior when it is in the air after being thrown
        t += Time.deltaTime;
        float t01 = t / ballThrowDuration;
        Vector3 pos = Vector3.Lerp(PosOverHead.position, Target.position, t01); // Fly towards the basket
        Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * Mathf.PI); // Fly with a arc effect
        currentBall.transform.position = pos + arc;

        if (t01 >= 1)
        {
            // Ball has reached the target
            isBallFlying = false;
            currentBall.GetComponent<Rigidbody>().isKinematic = false;
            src.clip = sfx1;
            src.Play();
            score++;
            scoreText.text = "Score: " + score;
            DestroyBallWithDelay();
            SpawnNewBall();
            timer += 3f;
            UpdateTimerText();

            // Adjust countdown timer based on score
            if (score >= 10 && score < 20)
                countdownTimer = 3f;
            else if (score >= 20 && score < 30)
                countdownTimer = 2f;
            else if (score >= 30 && score < 40)
                countdownTimer = 1f;
            else if (score >= 40)
                countdownTimer = 0.5f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Handle catching the ball and starting the countdown timer
        if (!isBallInHands && !isBallFlying)
        {
            isBallInHands = true;
            if (currentBall != null)
                currentBall.GetComponent<Rigidbody>().isKinematic = true;
            isCountdownActive = true;
            countdownTimer = 5f;
        }
    }

    void UpdateTimer()
    {
        // Update the timer
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 0f;
                GameOver();
            }
            UpdateTimerText();
        }
        else if (timer <= 0f)
        {
            timer = 0f;
            GameOver();
        }
    }

    void UpdateCountdownTimer()
    {
        // Update the countdown timer
        countdownTimer -= Time.deltaTime;
        if (countdownTimer <= 0f)
        {
            DestroyBallWithDelay();
            SpawnNewBall();
            timer -= 3f;
            UpdateTimerText();

            // Adjust countdown timer based on score
            if (score >= 10 && score < 20)
                countdownTimer = 3f;
            else if (score >= 20 && score < 30)
                countdownTimer = 2f;
            else if (score >= 30 && score < 40)
                countdownTimer = 1f;
            else if (score >= 40)
                countdownTimer = 0.5f;
        }
    }

    void UpdateTimerText()
    {
        // Update the timer UI text
        timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
    }

    void GameOver()
    {
        // Set the game over state and load the game over scene
        isGameOver = true;
        SceneManager.LoadScene("GameOver");
        PlayerPrefs.SetInt("FinalScore", score);
    }

    void SpawnNewBall()
    {
        // Spawn a new ball at a random position
        ballSpawnPosition = GetRandomSpawnPosition();
        currentBall = Instantiate(BallPrefab, ballSpawnPosition, Quaternion.identity);
        isCountdownActive = true;
        countdownTimer = 5f;
    }

    void DestroyBallWithDelay()
    {
        // Destroy the ball with a small delay
        Destroy(currentBall, 0.1f);
    }

     Vector3 GetRandomSpawnPosition()
    {
        // Get a random spawn position within a specified range
        Vector3 spawnRangeMin = new Vector3(-7f, 0.6f, -7f);
        Vector3 spawnRangeMax = new Vector3(7f, 0.6f, 7f);
        float randomX = Random.Range(spawnRangeMin.x, spawnRangeMax.x);
        float randomY = Random.Range(spawnRangeMin.y, spawnRangeMax.y);
        float randomZ = Random.Range(spawnRangeMin.z, spawnRangeMax.z);
        return new Vector3(randomX, randomY, randomZ);
    }
}
