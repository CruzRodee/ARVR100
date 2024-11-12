using UnityEngine;

public class SimulateARImageMovement : MonoBehaviour
{
    public float moveSpeed = 1f;

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;
        float moveZ = 0f;

        // Check for arrow key inputs for X and Z axis movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveX = moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveZ = moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveZ = -moveSpeed * Time.deltaTime;
        }

        // Check for Ctrl + Up Arrow and Ctrl + Down Arrow for Y-axis movement
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveY = moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveY = -moveSpeed * Time.deltaTime;
            }
        }

        // Move the object based on arrow key input and Ctrl for Y-axis movement
        transform.Translate(new Vector3(moveX, moveY, moveZ));
    }
}
