using UnityEngine;
using System.Collections.Generic;

public class ColonyManager : MonoBehaviour
{
    public PlayerController playerController;
    private List<ColonistPlayerAI> selected;

    private GameObject Colonist;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Colonist = gameObject;
        selected = new List<ColonistPlayerAI>();
    }

    public void SelectAndMove()
    {
        if (playerController.ObjectHit.transform.parent != null)
            Colonist = playerController.ObjectHit.transform.parent.gameObject;

        if (Colonist.TryGetComponent<ColonistPlayerAI>(out ColonistPlayerAI playerAI))
        {
            playerAI.SelectUI();
            selected.Add(playerAI);

            Colonist = gameObject;

            Debug.Log("selected " + playerAI.name);
        }
        else
        {
            Debug.Log("trying to move " + selected.Count);
            for (int i = 0; i < selected.Count; i++)
            {
                Debug.Log("moving " + selected[i]);

                selected[i].MoveTo(playerController.movementTarget.transform);
                selected[i].DeselectUI();
            }
                selected.Clear();
        }
    }
}
