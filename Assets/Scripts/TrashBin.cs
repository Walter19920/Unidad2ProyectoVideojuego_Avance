using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public float interactionDistance = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                float distance = Vector3.Distance(transform.position, playerObj.transform.position);

                if (distance <= interactionDistance)
                {
                    PlayerController player = playerObj.GetComponent<PlayerController>();
                    if (player != null && player.isCarryingTrash)
                    {
                        player.Deposit();
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}