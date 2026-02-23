using UnityEngine;
using TMPro;

public struct SignupData
{
    public string username;
    public string nickname;
    public string password;
}

public class SignupPanelController : PanelController
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;
    [SerializeField] private TMP_InputField nicknameInputField;

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
        var nickname = nicknameInputField.text;
        var password = passwordInputField.text;
        var confirmPassword = confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            // "입력 값이 누락되었습니다." 팝업 창 표시
            GameManager.Instance.OpenConfirmPanel("입력 값이 누락되었습니다.", () =>
            {

            });
        }

        // 비밀번호 입력이 동일한지 확인
        if (password.Equals(confirmPassword))
        {
            // 비밀번호가 동일하다면 회원가입 진행
            var signupData = new SignupData();
            signupData.username = username;
            signupData.nickname = nickname;
            signupData.password = password;

            // 서버로 SignupData 전달하면서 회원가입 진행
            StartCoroutine(NetworkManager.Instance.Signup(signupData, () =>
            {
                Hide();
            }, () =>
            {
                usernameInputField.text = "";
                passwordInputField.text = "";
                confirmPasswordInputField.text = "";
                nicknameInputField.text = "";
            }));
        }
        else
        {
            // "비밀번호가 다릅니다" 팝업 창 표시
            GameManager.Instance.OpenConfirmPanel("비밀번호가 다릅니다.", () =>
            {
                passwordInputField.text = "";
                confirmPasswordInputField.text = "";
            });
        }
    }

    public void OnClickCancelButton()
    {
        Hide();
    }
}