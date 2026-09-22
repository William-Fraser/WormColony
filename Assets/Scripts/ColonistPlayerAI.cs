using UnityEngine;
using Pathfinding;

public class ColonistPlayerAI : MonoBehaviour
{
    private AIDestinationSetter destination;
    private Outline outline;

    void Awake()
    {
        destination = GetComponent<AIDestinationSetter>();
        outline = gameObject.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 10f;
        outline.OutlineColor = Color.green;
        outline.enabled = false;
    }

    public void MoveTo(Transform target)
    {
        destination.target = target;
    }

    public void SelectUI()
    {
        outline.enabled = true;
    }

    public void DeselectUI()
    { 
        outline.enabled=false;
    }
}