using UnityEngine;
using TMPro;

public struct SigninData
{
    public string username;
    public string password;
}

public struct SigninResult
{
    public string message;
}

public class SigninPanelController : PanelController
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    // 회원가입 팝업에서 "확인" 버튼 클릭 시 동작할 함수를 전달받기 위한 델리게이트
    public delegate void OnSignupButtonClicked();
    private OnSignupButtonClicked _onSignupButtonClicked;

    public void Show(OnSignupButtonClicked onSignupButtonClicked)
    {
        _onSignupButtonClicked = onSignupButtonClicked;
        Show();        
    }

    public void OnClickConfirmButton()
    {
        // Input Field에 입력된 값을 체크해서 서버에 전달
        var username = usernameInputField.text;
        var password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // "입력 값이 누락되었습니다." 팝업 창 표시
            GameManager.Instance.OpenConfirmPanel("입력 값이 누락되었습니다.", () =>
            {

            });
        }

        var signinData = new SigninData();
        signinData.username = username;
        signinData.password = password;

        StartCoroutine(NetworkManager.Instance.Signin(signinData, () =>
        {
            Hide();
        }, () =>
        {
            usernameInputField.text = "";
            passwordInputField.text = "";
        }));
    }

    public void OnClickCancelButton()
    {
        Hide();
    }
}
