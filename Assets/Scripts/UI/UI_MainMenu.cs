using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn();
    }
    public void PlayButton()
    {
        GameManager.instance.ContinuePlay();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

}
