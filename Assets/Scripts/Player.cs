using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 direction;
    private bool hasmoved = true;

    void Update()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        if(direction.x == 0 && direction.y == 0)
        {
            hasmoved = true;
        }

        if(hasmoved)
        {
            if (direction.x != 0 || direction.y != 0)
            {
                Debug.Log("Attempting move: " + transform.position + " to " + (transform.position + direction));
                Move();
                hasmoved = false;
            }
        }
    }

    void Move()
    {
        bool validmove = false;

        validmove = BoardManager.instance.GetTile(transform.position, transform.position + direction, direction);

        if(validmove)
        {
            Debug.Log("Player moving");
            transform.position += direction;
        }
    }
}
