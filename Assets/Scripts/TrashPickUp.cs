using UnityEngine;

public class TrashPickUp : MonoBehaviour
{
    public float interactionDistance = 2.5f; // Distancia máxima para recoger

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Buscar al jugador en la escena
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                // Calcular distancia entre la basura y el jugador
                float distance = Vector3.Distance(transform.position, playerObj.transform.position);

                if (distance <= interactionDistance)
                {
                    PlayerController player = playerObj.GetComponent<PlayerController>();
                    if (player != null && !player.isCarryingTrash)
                    {
                        player.PickUp(gameObject);
                    }
                }
            }
        }
    }

    // Dibuja un círculo amarillo en la vista de escena para ver el alcance
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}