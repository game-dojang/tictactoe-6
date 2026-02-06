using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject settingsPanelPrefab;
    [SerializeField] private Canvas canvas;

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // TODO: 씬이 전환될 때 GameManager가 해야 할 일 구현
    }

    // Settings 패널 열기
    public void OpenSettingsPanel()
    {
        var settingsPanelObject = Instantiate(settingsPanelPrefab, canvas.transform);
        settingsPanelObject.GetComponent<SettingsPanelController>().Show();
    }
}
