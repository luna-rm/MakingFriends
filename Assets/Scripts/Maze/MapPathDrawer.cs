using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MapPathDrawer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player; // Drag your Player object here

    [Header("Settings")]
    [SerializeField] private float minDistance = 1.0f; // Distance moved before adding a new point
    [SerializeField] private float mapHeight = 50f; // Height Y to draw the line (should be high above the maze)

    private LineRenderer line;
    private Vector3 lastRecordedPos;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        
        // Basic Setup in code (you can also do this in Inspector)
        line.positionCount = 0;
        line.useWorldSpace = true;
    }

    private void Start()
    {
        // Initialize the first point
        Vector3 startPos = GetMapPosition();
        AddPoint(startPos);
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 currentPos = GetMapPosition();

        // Calculate distance ignoring Y axis (only X and Z movement matters for the map)
        if (Vector3.Distance(lastRecordedPos, currentPos) > minDistance)
        {
            AddPoint(currentPos);
        }
    }

    private void AddPoint(Vector3 position)
    {
        // Increase the number of points in the line
        line.positionCount++;
        // Set the new point at the end of the array
        line.SetPosition(line.positionCount - 1, position);
        
        lastRecordedPos = position;
    }

    private Vector3 GetMapPosition()
    {
        // Return player's X and Z, but use the fixed Map Height for Y
        // This keeps the line flat and clean, even if the player jumps
        return new Vector3(player.position.x, mapHeight, player.position.z);
    }
}