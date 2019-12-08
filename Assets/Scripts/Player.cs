using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 direction;
    private bool hasmoved = true;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            BoardManager.instance.ResetLevel();
        }

        direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        if(direction.x == 0 && direction.y == 0) // Only let the player move once per keypress
        {
            hasmoved = true;
        }

        if(hasmoved)
        {
            if (direction.x != 0 || direction.y != 0)
            {
                Move();
                hasmoved = false;
            }
        }
    }

    void Move()
    {
        bool validmove = false;

        validmove = BoardManager.instance.GetTile(transform.position, transform.position + direction, direction); // Check wether the target tile is free/valid

        if(validmove)
        {
            Debug.Log("Player moving");
            transform.position += direction;
        }
    }
}
