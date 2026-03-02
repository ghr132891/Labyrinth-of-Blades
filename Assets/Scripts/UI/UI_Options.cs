using UnityEngine;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{

    private Player player;

    [SerializeField] private Toggle healthBarTooggle;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();

        healthBarTooggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    private void OnHealthBarToggleChanged(bool isOn)
    {
        player.health.EnableHealthBar(isOn);
    }

}
