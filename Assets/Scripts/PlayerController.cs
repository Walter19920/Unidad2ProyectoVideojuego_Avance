using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 120f;
    public float jumpHeight = 1.8f;
    public float gravity = -15f;

    [Header("Basura y Puntos de Sujeción")]
    public Transform handPoint;
    public bool isCarryingTrash = false;
    private GameObject carriedTrash = null;

    [Header("Interfaz de Usuario (Canvas TMP)")]
    public TextMeshProUGUI uiMessageText; // Texto del contador superior
    public GameObject winPanel;           // Panel flotante de Victoria

    private int currentDeposited = 0;
    private int totalTrashInScene = 0;

    [Header("Ajuste de IK para Manos")]
    [Range(0, 1)] public float ikWeight = 1f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        // Asegurar que el panel de victoria inicie oculto
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Contar la basura total en la escena con Tag "Trash"
        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("Trash");
        totalTrashInScene = trashObjects.Length;

        UpdateUI($"Basura depositada: {currentDeposited}/{totalTrashInScene}");
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Salto
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Movimiento WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(0f, horizontal * rotationSpeed * Time.deltaTime, 0f);

        Vector3 move = transform.forward * vertical * moveSpeed;
        controller.Move(move * Time.deltaTime);

        if (animator != null)
        {
            animator.speed = (Mathf.Abs(vertical) > 0.1f) ? 1f : 0f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (isCarryingTrash && handPoint != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, handPoint.position + handPoint.right * 0.15f);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, handPoint.position - handPoint.right * 0.15f);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }

    public bool PickUp(GameObject trash)
    {
        if (isCarryingTrash) return false;

        isCarryingTrash = true;
        carriedTrash = trash;

        Rigidbody rb = trash.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = trash.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (handPoint != null)
        {
            trash.transform.SetParent(handPoint);
            trash.transform.localPosition = Vector3.zero;
            trash.transform.localRotation = Quaternion.identity;
            trash.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        }

        return true;
    }

    public void Deposit()
    {
        if (!isCarryingTrash) return;

        Destroy(carriedTrash);
        carriedTrash = null;
        isCarryingTrash = false;

        currentDeposited++;
        UpdateUI($"Basura depositada: {currentDeposited}/{totalTrashInScene}");

        // CONDICIÓN DE VICTORIA
        if (currentDeposited >= totalTrashInScene && totalTrashInScene > 0)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true); // Activar la pantalla de victoria
            }
        }
    }

    private void UpdateUI(string message)
    {
        if (uiMessageText != null)
        {
            uiMessageText.text = message;
        }
    }

    // MÉTODO PARA REINICIAR EL JUEGO
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}