using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_DeathScreen : MonoBehaviour
{
    public void GoToCampButton()
    {
        GameManager.instance.ChangeScene("Level_0",RespawnType.NoneSpecific);
    }

    public void GoToCheckPointButton()
    {
        GameManager.instance.RestartScene();
    }

    public void GoToMainMenu()
    {
        GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);
    }

}
