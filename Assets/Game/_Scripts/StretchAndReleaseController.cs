using UnityEngine;

public class StretchAndReleaseController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float forceMultiplier = 10f;
    public float maxStretchDistance = 5f; // Maximum distance the stretch can reach
    private Vector2 initialMousePosition;
    private LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>(); // Assign the LineRenderer component
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detects mouse click
        {
            initialMousePosition = transform.position; // Start the line at the player's position
            lineRenderer.SetPosition(0, initialMousePosition); // Set the start position of the line
            lineRenderer.enabled = true; // Enable the LineRenderer
        }
        else if (Input.GetMouseButton(0)) // Detects mouse drag
        {
            Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = CalculateStretchDirection(initialMousePosition, currentMousePosition);
            float distance = Mathf.Min(CalculateStretchMagnitude(initialMousePosition, currentMousePosition), maxStretchDistance);
            Vector2 finalMousePosition = initialMousePosition + direction * distance;
            lineRenderer.SetPosition(1, finalMousePosition); // Update the end position of the line
        }
        else if (Input.GetMouseButtonUp(0)) // Detects mouse release
        {
            Vector2 finalMousePosition = lineRenderer.GetPosition(1); // Use the end position of the line

            Vector2 direction = CalculateStretchDirection(initialMousePosition, finalMousePosition);
            float magnitude = CalculateStretchMagnitude(initialMousePosition, finalMousePosition);

            Vector2 force = direction * magnitude * forceMultiplier;
            rb.AddForce(force);

            lineRenderer.enabled = false; // Disable the LineRenderer
        }
    }

    Vector2 CalculateStretchDirection(Vector2 initialPosition, Vector2 finalPosition)
    {
        Vector2 direction = initialPosition - finalPosition; // Inverted calculation
        return direction.normalized;
    }

    float CalculateStretchMagnitude(Vector2 initialPosition, Vector2 finalPosition)
    {
        float magnitude = Vector2.Distance(initialPosition, finalPosition);
        return magnitude;
    }
}
