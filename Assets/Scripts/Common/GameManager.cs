using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject settingsPanelPrefab;
    private Canvas _canvas;

    // Game Logic
    private GameLogic _gameLogic;

    // 게임의 종류 (싱글, 듀얼)
    private GameType _gameType;

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬에서 Canvas 참조 가져오기
        _canvas = FindFirstObjectByType<Canvas>();

        if (scene.name == SCENE_GAME)
        {
            // 게임 씬에서 블록 초기화
            var blockController = FindFirstObjectByType<BlockController>();
            if (blockController != null)
            {
                blockController.InitBlocks();
            }

            // Game Logic 생성
            _gameLogic = new GameLogic(GameType.DualPlay, blockController);
        }
    }

    // Settings 패널 열기
    public void OpenSettingsPanel()
    {
        var settingsPanelObject = Instantiate(settingsPanelPrefab, _canvas.transform);
        settingsPanelObject.GetComponent<SettingsPanelController>().Show();
    }

    // 씬 전환 (Main > Game)
    public void ChangeToGameScene(GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene(SCENE_GAME);       
    }

    // 씬 전환 (Game > Main)
    public void ChangeToMainScene()
    {
        SceneManager.LoadScene(SCENE_MAIN);
    }
}
