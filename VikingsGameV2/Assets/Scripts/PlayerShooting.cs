using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public UIManager uiManager;
    private Camera mainCamera;
    private LineRenderer lineRenderer;
    private Animator animator;
    private bool isAiming = false;

    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        // Handle aiming
        if (Input.GetMouseButton(1)) // Right mouse button to aim
        {
            if (!isAiming)
            {
                isAiming = true;
                animator.SetBool("isAiming", true);
            }
        }
        else
        {
            if (isAiming)
            {
                isAiming = false;
                animator.SetBool("isAiming", false);
            }
        }

        // Handle shooting
        if (Input.GetMouseButtonDown(0) && isAiming) // Left mouse button to shoot while aiming
        {
            animator.SetTrigger("shoot");
            Shoot();
        }

        // Update line renderer position
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, mainCamera.transform.position);
        }
    }

    void Shoot()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Visualize the shot with a red line
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 100f);
        lineRenderer.enabled = true;

        Invoke("HideLine", 0.1f);

        // Check for hits
        if (Physics.Raycast(ray, out hit))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null && !enemy.IsHit())
            {
                uiManager.UpdateMessage($"Fire Hit: {hit.collider.gameObject.name}", Color.green);
                enemy.Hit();
                uiManager.UpdateScore();
                // Notify GameManager of hit (for win condition)
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.EnemyHit();
                }
            }
            else
            {
                uiManager.UpdateMessage("Fire Miss", Color.blue);
            }
        }
        else
        {
            uiManager.UpdateMessage("Fire Miss", Color.blue);
        }
    }

    void HideLine()
    {
        lineRenderer.enabled = false;
    }
}