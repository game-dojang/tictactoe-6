using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public struct SigninData
{
    public string username;
    public string password;
}

public struct SigninResult
{
    public string message;
}

public struct SignupData
{
    public string username;
    public string nickname;
    public string password;
}

public struct ScoreResult
{
    public int score;
}

public class NetworkManager : Singleton<NetworkManager>
{
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // 네트워크 매니저는 씬이 로드될 때마다 초기화할 필요가 없습니다.
        // 따라서 이 메서드는 비워둡니다.
    }
    
    /// <summary>
    /// 회원가입을 위한 함수
    /// </summary>
    /// <param name="signupData">회원가입에 필요한 정보</param>
    /// <returns></returns>
    public IEnumerator Signup(SignupData signupData, Action success, Action failure)
    {
        string jsonString = JsonUtility.ToJson(signupData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError 
                || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // 오류 코드별 처리
                if (www.responseCode == 400)
                {
                    // 400 오류 발생 팝업 표시
                    GameManager.Instance.OpenConfirmPanel("필수 요소가 누락되었습니다.", () =>
                    {
                        failure?.Invoke();
                    });
                }
                else if (www.responseCode == 409)
                {
                    // 401 오류 발생 팝업 표시
                    GameManager.Instance.OpenConfirmPanel("이미 가입된 아이디입니다.", () =>
                    {
                        failure?.Invoke();
                    });
                }
            }
            else
            {
                var result = www.downloadHandler.text;
                Debug.Log("Result: " + result);

                // 회원 가입 성공 팝업을 표시
                GameManager.Instance.OpenConfirmPanel("회원 가입이 성공적으로 완료되었습니다.", () =>
                {
                    success?.Invoke();
                });
            }
        }
    }

    /// <summary>
    /// 로그인을 위한 함수
    /// </summary>
    /// <param name="signinData">로그인에 필요한 정보</param>
    /// <param name="success">로그인 성공 시 호출할 함수</param>
    /// <param name="failure">로그인 실패 시 호출할 함수</param>
    /// <returns></returns>
    public IEnumerator Signin(SigninData signinData, Action success, Action failure)
    {
        string jsonString = JsonUtility.ToJson(signinData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError 
                || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // 오류 코드별 처리
                if (www.responseCode == 400)
                {
                    // 400 오류 발생 팝업 표시
                }
            }
            else 
            {
                var cookie = www.GetResponseHeader("Set-Cookie");
                if (!string.IsNullOrEmpty(cookie))
                {
                    // 쿠키 저장
                    int lastIndex = cookie.LastIndexOf(";");
                    string sid = cookie.Substring(0, lastIndex);
                    PlayerPrefs.SetString("SID", sid);
                }

                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);

                Debug.Log("Result: " + resultString);
            }
        }
    }

    public IEnumerator GetScore(Action<ScoreResult> success, Action failure)
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/score", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();

            string sid = PlayerPrefs.GetString("SID", null);
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError
                || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // 오류 코드별 처리
                if (www.responseCode == 400)
                {
                    // 400 오류 발생 팝업 표시
                }

                failure?.Invoke();
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<ScoreResult>(resultString);
                Debug.Log("Score: " + result.score);

                success?.Invoke(result);
            }
        }
    }

    public IEnumerator Signout(Action<SigninResult> success, Action failure)
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signout", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();

            string sid = PlayerPrefs.GetString("SID", null);
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError
                || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // 오류 코드별 처리
                if (www.responseCode == 400)
                {
                    // 400 오류 발생 팝업 표시
                }

                failure?.Invoke();
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);
                Debug.Log("Score: " + result.message);

                success?.Invoke(result);
            }
        }
    }
}
