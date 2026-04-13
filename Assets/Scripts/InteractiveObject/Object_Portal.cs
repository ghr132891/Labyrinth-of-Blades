using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Portal : MonoBehaviour,ISaveable
{
    public static Object_Portal instance;
    public bool isActive { get; private set; }

    [SerializeField] private Vector2 defaultPosition; // where portal in town.
    [SerializeField] private string townSceneName = "Level_0";

    [SerializeField] private Transform respawmPoint;
    [SerializeField] private bool canBeTriggered;

    private string currentSceneName;
    private bool returningFromTown;
    private string returnSceneName;

    private void Awake()
    {
        instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
        transform.position = new Vector3(9999, 9999); //Hide Portal.
    }

    public void DisableIfNeeded()
    {
        if (returningFromTown == false)
            return;

        SaveManager.instance.GetGameData().inScenePortals.Remove(currentSceneName);
        isActive = false;
        transform.position = new Vector3(9999, 9999); 
    }

    private void UseTeleport()
    {
        string destinationScene = InTown() ? returnSceneName : townSceneName;

        GameManager.instance.ChangeScene(destinationScene,RespawnType.Portal);
    }

    public void ActivatePortal(Vector3 positon, int facingDir = 1)
    {
        isActive = true;
        transform.position = positon;
        SaveManager.instance.GetGameData().inScenePortals.Clear();


        if (facingDir == -1)
            transform.Rotate(0, 180, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;
        UseTeleport();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }

    public void SetTrigger(bool trigger) => canBeTriggered = trigger;

    public Vector3 GetPosition() => respawmPoint != null ? respawmPoint.position : transform.position;

    private bool InTown() => currentSceneName == townSceneName;

    public void LoadData(GameData gameData)
    {
        
        if(InTown() && gameData.inScenePortals.Count > 0)
        {
            transform.position = defaultPosition;
            isActive = true;
        }
        else if (gameData.inScenePortals.TryGetValue(currentSceneName,out Vector3 portalPosition))
        {
            transform.position = portalPosition;
            isActive = true;
        }

        returningFromTown = gameData.returningFromTown;
        returnSceneName = gameData.portalDestinationSceneName;

    }

    public void SaveData(ref GameData gameData)
    {
        gameData.returningFromTown =InTown();

        if (isActive && InTown() == false)
        {
            gameData.inScenePortals[currentSceneName] = transform.position;
            gameData.portalDestinationSceneName = currentSceneName;
        }
        else
        {
            gameData.inScenePortals.Remove(currentSceneName);
        }

    }
}
