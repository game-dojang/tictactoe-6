using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
                // 오류 발생 팝업 표시
                GameManager.Instance.OpenConfirmPanel("회원가입이 실패했습니다.", () =>
                {
                   failure?.Invoke();
                });
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
}
