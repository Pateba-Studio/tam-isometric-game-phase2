using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollisionHandler : MonoBehaviour
{
    [SerializeField] int targetPosition;
    [SerializeField] int originalPosition;
    [SerializeField] int targetOrder;
    [SerializeField] int originalOrder;

    [SerializeField] GameObject[] objectsToChange;

    public int collisionCounter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MoveableObject"))
        {
            collisionCounter++;

            foreach (GameObject obj in objectsToChange)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                renderer.sortingOrder = targetOrder;
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, originalPosition);

            //Debug.Log("Collision with: " + other.gameObject.name + ", CC: " + collisionCounter);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MoveableObject"))
        {
            collisionCounter--;

            if (collisionCounter <= 0)
            {
                foreach (GameObject obj in objectsToChange)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    renderer.sortingOrder = originalOrder;
                }

                transform.position = new Vector3(transform.position.x, transform.position.y, originalPosition);
                collisionCounter = 0;
            }
        }
    }
}
