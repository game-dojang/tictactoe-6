using UnityEngine;
using static Constants;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private GameObject signupPanelPrefab;

    public void OnClickSinglePlayButton()
    {
        GameManager.Instance.ChangeToGameScene(GameType.SinglePlay);
    }

    public void OnClickDualPlayButton()
    {
        GameManager.Instance.ChangeToGameScene(GameType.DualPlay);
    }

    public void OnClickSettingsButton()
    {
        GameManager.Instance.OpenSettingsPanel();
    }

    public void OnClickMultiPlayButton()
    {
        GameManager.Instance.ChangeToGameScene(GameType.MultiPlay);
    }
}
