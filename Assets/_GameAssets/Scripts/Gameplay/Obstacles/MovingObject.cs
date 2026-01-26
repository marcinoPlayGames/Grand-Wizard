using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] Transform platform;
    public Vector2 moveOffset;
    public float speed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool moving;
    private bool isOpen;

    void Awake()
    {
        closedPos = platform.position;
        openPos = closedPos + (Vector3)moveOffset;
    }

    void Update()
    {
        if (!moving) return;

        Vector3 target = isOpen ? openPos : closedPos;

        platform.position = Vector3.MoveTowards(
            platform.position,
            target,
            speed * Time.deltaTime
        );

        if (platform.position == target)
            moving = false;
    }

    public void Activate()   // Open
    {
        isOpen = true;
        moving = true;
    }

    public void Deactivate() // Close
    {
        isOpen = false;
        moving = true;
    }
}