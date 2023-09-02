using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    // Public variables for customization in the Unity Editor
    [SerializeField]
    public PointCounter pointsCounter; // user points counter
    public Transform segmentPrefab;  // Prefab for snake segments
    public Vector2Int direction = Vector2Int.right;  // Initial direction
    public float speed = 5f;  // Base speed
    public float speedMultiplier = 0.5f;  // Speed multiplier
    public float maxSpeed = 20f;  // Maximum speed multiplier limit
    public int initialSize = 4;  // Initial snake size
    public bool moveThroughWalls = false;  // Can the snake move through walls?
    public int nomOfFoodEaten = 0;

    // Private variables for internal state
    private List<Transform> segments = new List<Transform>();  // List of snake segments
    private Vector2Int input;  // User input for direction
    private float nextUpdate;  // Time for the next update

    // Initialize the snake state
    private void Start()
    {
        ResetState();
        pointsCounter.Start();
    }

    // Handle user input for direction
    private void Update()
    {
        // Restrict vertical movement when moving horizontally
        if (direction.x != 0f)
        {
            HandleVerticalInput();
        }
        // Restrict horizontal movement when moving vertically
        else if (direction.y != 0f)
        {
            HandleHorizontalInput();
        }
    }

    // Update snake position based on direction and speed
    private void FixedUpdate()
    {
        // Delay update until the next scheduled time
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Update direction based on user input
        if (input != Vector2Int.zero)
        {
            direction = input;
        }

        // Update segment positions to follow the head
        UpdateSegmentPositions();

        // Move the head in the current direction
        MoveHead();

        // Schedule the next update based on the speed
        nextUpdate = Time.time + (1f / speed);
    }

    // Add a new segment to the snake and increase speed
    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);

        // Increase speed multiplier and clamp it to the maximum speed
        speed += speedMultiplier;
        Debug.Log("speed: " + speed.ToString());
        speed = Mathf.Min(speed, maxSpeed);
        Debug.Log("speed: " + speed.ToString());
    }

    // Reset the snake to its initial state
    public void ResetState()
    {
        // Initialize direction and position
        direction = Vector2Int.right;
        transform.position = Vector3.zero;
        speed = 5f;
        maxSpeed = 25f;
        speedMultiplier = 0.5f;

        // Destroy all segments except the head
        DestroySegments();

        // Reinitialize the snake with the head and new segments
        InitializeSnake();
    }

    // Check if a grid cell is occupied by the snake
    public bool Occupies(int x, int y)
    {
        // Iterate through segments to check occupation
        return CheckOccupation(x, y);
    }

    // Handle collision events
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Grow when colliding with food
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
            nomOfFoodEaten++;
            pointsCounter.Factor += 2;
        }
        // Reset when colliding with an obstacle
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
            pointsCounter.GameContinues = false;
            PlayerPrefs.SetString("LastScore", pointsCounter.Points.ToString());
            SceneManager.LoadSceneAsync(0);
        }
        // Handle wall collision based on the moveThroughWalls setting
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                ResetState();
                pointsCounter.GameContinues = false;
                PlayerPrefs.SetString("LastScore", pointsCounter.Points.ToString());
                SceneManager.LoadSceneAsync(0);
            }
        }
    }

    // Move to the opposite wall when colliding with a wall
    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;
        // Handle horizontal wall traversal
        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        // Handle vertical wall traversal
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

    // Additional helper methods for better code organization
    private void HandleVerticalInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            input = Vector2Int.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            input = Vector2Int.down;
        }
    }

    private void HandleHorizontalInput()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            input = Vector2Int.right;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            input = Vector2Int.left;
        }
    }

    private void UpdateSegmentPositions()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }
    }

    private void MoveHead()
    {
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);
    }

    private void DestroySegments()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
    }

    private void InitializeSnake()
    {
        segments.Clear();
        segments.Add(transform);
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }
    }

    private bool CheckOccupation(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }
}
