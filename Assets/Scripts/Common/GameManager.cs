using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject settingsPanelPrefab;
    [SerializeField] private GameObject confirmPanelPrefab;

    // 캔버스
    private Canvas _canvas;

    // 게임 화면의 UI 컨트롤러
    private GamePanelController _gamePanelController; 

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

            // GamePanelController 참조 가져오기
            _gamePanelController = FindFirstObjectByType<GamePanelController>();

            // Game Logic 생성
            _gameLogic = new GameLogic(_gameType, blockController);
        }
    }

    // Game O/X UI 업데이트
    public void SetGameTurn(Constants.PlayerType playerTurnType)
    {
        _gamePanelController.SetPlayerTurnPanel(playerTurnType);
    }

    // Settings 패널 열기
    public void OpenSettingsPanel()
    {
        var settingsPanelObject = Instantiate(settingsPanelPrefab, _canvas.transform);
        settingsPanelObject.GetComponent<SettingsPanelController>().Show();
    }

    // Confirm 패널 열기
    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClicked onConfirmButtonClicked)
    {
        var confirmPanelObject = Instantiate(confirmPanelPrefab, _canvas.transform);
        confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, onConfirmButtonClicked);
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
        _gameLogic?.Dispose();
        _gameLogic = null;
        SceneManager.LoadScene(SCENE_MAIN);
    }

    // 게임이 강제 종료했을 때
    private void OnApplicationQuit()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
    }
}
