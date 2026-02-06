using UnityEngine;

public class GamePanelController : MonoBehaviour
{
    void Start()
    {
    }

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
}
