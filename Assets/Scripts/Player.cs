using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 gridPos;
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
                gridPos = new Vector3(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y),
                    0
                );
                Move();
                hasmoved = false;
            }
        }
    }

    void Move()
    {
        if (BoardManager.instance.GetTile(gridPos, gridPos + direction, direction))
        {
            transform.position += direction;
            BoardManager.instance.CheckObjectives();
        }
    }
}
