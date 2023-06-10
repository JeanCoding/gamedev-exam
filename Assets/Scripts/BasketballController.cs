using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasketballController : MonoBehaviour
{
    // References
    public float MoveSpeed = 10;
    public GameObject BallPrefab; 
    public Transform Arms;
    public Transform PosOverHead;
    public Transform PosDribble;
    public Transform Target;

    // Variables
    private GameObject currentBall;
    private bool IsBallInHands = true;
    private bool IsBallFlying = false;
    private bool isCountdownActive = false;
    private float T = 0;
    private int score = 0;
    private float timer = 20f;
    private float countdownTimer = 5f;
    private bool isGameOver = false;

    // Ball spawn parameters
    private Vector3 ballSpawnPosition;

    // Reference to the UI Text components
    public Text scoreText;
    public Text timerText;

    private void Start()
    {
        GetRandomSpawnPosition();
        SpawnNewBall();
        UpdateTimerText();
    }

    void Update()
    {
        if (isGameOver)
            return;

        // Walking
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);

        // Ball is in your hands
        if (IsBallInHands)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                currentBall.transform.position = PosOverHead.position;
                Arms.localEulerAngles = Vector3.right * 180;

                // Looks towards the basket 
                transform.LookAt(Target.parent.position);
            }
            else
            {
                // Creates the bouncing ball and puts the arms down
                currentBall.transform.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                IsBallInHands = false;
                IsBallFlying = true;
                T = 0;

                // Stop the countdown timer when the ball is thrown
                isCountdownActive = false;
            }
        }

        // When the ball is in the air
        if (IsBallFlying)
        {
            T += Time.deltaTime;
            float duration = 0.7f;
            float t01 = T / duration;

            // To fly towards the basket
            Vector3 A = PosOverHead.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A, B, t01);

            // Move in arc
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * Mathf.PI);

            currentBall.transform.position = pos + arc;

            // The moment when ball hits the target
            if (t01 >= 1)
            {
                IsBallFlying = false;
                currentBall.GetComponent<Rigidbody>().isKinematic = false;

                // Increment the score
                score++;

                // Update the score on the UI Text component
                scoreText.text = "Score: " + score;

                // Destroy the ball with a small delay
                DestroyBallWithDelay();

                // Spawn a new ball
                SpawnNewBall();

                // Add time to the timer
                timer += 3f;
                UpdateTimerText();

                // Set the countdown timer duration for the new ball based on the score
                if (score >= 10 && score < 20)
                {
                    countdownTimer = 3f;
                }
                else if (score >= 20 && score < 30)
                {
                    countdownTimer = 2f;
                }
                else if (score >= 30)
                {
                    countdownTimer = 1f;
                }
                else if (score >= 40)
                {
                    countdownTimer = 0.5f;
                }
            }
        }

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

        // Countdown timer
        if (isCountdownActive)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                // Destroy the ball with a small delay
                DestroyBallWithDelay();

                // Spawn a new ball
                SpawnNewBall();

                // Remove 3 seconds from the timer
                timer -= 3f;
                UpdateTimerText();

                // Set the countdown timer duration for the new ball based on the score
                if (score >= 10 && score < 20)
                {
                    countdownTimer = 3f;
                }
                else if (score >= 20 && score < 30)
                {
                    countdownTimer = 2f;
                }
                else if (score >= 30 && score < 40)
                {
                    countdownTimer = 1f;
                }
                else if (score >= 40)
                {
                    countdownTimer = 0.5f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsBallInHands && !IsBallFlying)
        {
            IsBallInHands = true;
            if (currentBall != null)
                currentBall.GetComponent<Rigidbody>().isKinematic = true;

            isCountdownActive = true;
            countdownTimer = 5f;
        }
    }

    // Update the timer UI Text
    private void UpdateTimerText()
    {
        timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
    }

    // Set the game over state
    private void GameOver()
    {
        isGameOver = true;
        int finalScore = GetScore();
        SceneManager.LoadScene("GameOver");
        PlayerPrefs.SetInt("FinalScore", finalScore);
    }

    private void SpawnNewBall()
    {
        // Set a random spawn position within the specified range
        ballSpawnPosition = GetRandomSpawnPosition();

        // Spawn a new ball at the specified position
        currentBall = Instantiate(BallPrefab, ballSpawnPosition, Quaternion.identity);

        // Start the countdown timer
        isCountdownActive = true;
        countdownTimer = 5f;
    }

    private void DestroyBallWithDelay()
    {
        // Destroy the ball with a delay
        Destroy(currentBall, 0.1f);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Defines the range for the random spawn position
        Vector3 spawnRangeMin = new Vector3(-7f, 0.6f, -7f);
        Vector3 spawnRangeMax = new Vector3(7f, 0.6f, 7f);

        // Generates random spawn position within the specified range
        float randomX = Random.Range(spawnRangeMin.x, spawnRangeMax.x);
        float randomY = Random.Range(spawnRangeMin.y, spawnRangeMax.y);
        float randomZ = Random.Range(spawnRangeMin.z, spawnRangeMax.z);

        return new Vector3(randomX, randomY, randomZ);
    }

    private int GetScore()
    {
        return score;
    }
}
