using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GamePanelController : MonoBehaviour
{
    [SerializeField] private Image playerATurnImage;
    [SerializeField] private Image playerBTurnImage;

    // 뒤로가기 버튼 클릭
    public void OnClickBackButton()
    {
        GameManager.Instance.ChangeToMainScene();
    }

    // 설정 팝업 표시
    public void OnClickSettingsButton()
    {
        GameManager.Instance.OpenSettingsPanel();
    }

    public void SetPlayerTurnPanel(Constants.PlayerType playerTurnType)
    {
        switch (playerTurnType)
        {
            case Constants.PlayerType.None:
                playerATurnImage.color = Color.white;
                playerBTurnImage.color = Color.white;
                break;
            case Constants.PlayerType.Player1:
                playerATurnImage.color = Color.blue;
                playerBTurnImage.color = Color.white;
                break;
            case Constants.PlayerType.Player2:
                playerATurnImage.color = Color.white;
                playerBTurnImage.color = Color.blue;
                break;
        }
    }
}
