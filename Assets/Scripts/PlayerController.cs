using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public ColonyManager ColonyM;

    //mouse pos to world coords
    private Camera Cam;
    private RaycastHit RayHit;
    private Ray ray;
    public GameObject ObjectHit;
    public GameObject movementTarget;

    void Awake()
    {
        ObjectHit = gameObject;
    }

    public void Activate(InputAction.CallbackContext context)
    {// clicking LMB
        if (context.performed)
        { 
            WorldPosFromMouse();
            ColonyM.SelectAndMove();
        }
    }

    private void WorldPosFromMouse()
    {
        Cam = Camera.main;

        if (Cam != null)
        {
            ray = Cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RayHit))
            {
                ObjectHit = RayHit.transform.gameObject;

                movementTarget = new GameObject("Target");
                movementTarget.AddComponent<DestroySelfTool>();
                movementTarget.transform.position = RayHit.point;
            }
        }
    }
}