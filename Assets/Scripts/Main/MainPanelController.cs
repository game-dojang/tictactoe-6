using UnityEngine;
using static Constants;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private GameObject signupPanelPrefab;

    // 회원가입 패널 테스트
    void Start()
    {
        var signupPanelObject = Instantiate(signupPanelPrefab, transform);
        signupPanelObject.GetComponent<SignupPanelController>().Show(() =>
        {
            Debug.Log("회원가입 패널 테스트");
        });
    }


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
}
