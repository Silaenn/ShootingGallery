using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CrazyGames
{
    public class UserModuleDemo : MonoBehaviour
    {
        public Text userAccountAvailableText;
        public Text userLoggedOutText;
        public Image userAvatarImage;
        public Text usernameText;
        public GameObject userInfoContainer;

        private readonly List<Action<PortalUser>> _authListeners = new List<Action<PortalUser>>();

        public void Start()
        {
            userInfoContainer.SetActive(false);
            userLoggedOutText.gameObject.SetActive(false);
            CrazySDK.Init(() =>
            {
                CrazySDK.User.GetUser(
                    (user) =>
                    {
                        if (user != null)
                        {
                            Debug.Log("User is signed in: " + user);
                            userInfoContainer.SetActive(true);
                            usernameText.text = user.username;
                            StartCoroutine(FetchUserImage(user.profilePictureUrl));
                        }
                        else
                        {
                            Debug.Log("User is not signed in");
                            userLoggedOutText.gameObject.SetActive(true);
                        }
                    }
                );
            }); // ensure if starting this scene from editor it is initialized

            if (userAccountAvailableText != null)
            {
                var isAvailable = CrazySDK.User.IsUserAccountAvailable;
                userAccountAvailableText.text = $"User account available: {isAvailable}";
            }
        }

        IEnumerator FetchUserImage(string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                userAvatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }

        public void GetUser()
        {
            CrazySDK.User.GetUser(
                (user) =>
                {
                    if (user != null)
                    {
                        Debug.Log("Get user result: " + user);
                    }
                    else
                    {
                        Debug.Log("User is not signed in");
                    }
                }
            );
        }

        public void GetToken()
        {
            CrazySDK.User.GetUserToken(
                (error, token) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Get user token error: " + error);
                        return;
                    }

                    Debug.Log("User token: " + token);
                }
            );
        }

        public void GetXsollaUserToken()
        {
            CrazySDK.User.GetXsollaUserToken(
                (error, token) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Get Xsolla user token error: " + error);
                        return;
                    }

                    Debug.Log("Xsolla user token: " + token);
                }
            );
        }

        public void SyncUnityGameData()
        {
            CrazySDK.User.SyncUnityGameData();
        }

        public void ShowAuthPrompt()
        {
            CrazySDK.User.ShowAuthPrompt(
                (error, user) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Show auth prompt error: " + error);
                        return;
                    }

                    Debug.Log("Auth prompt user: " + user);
                }
            );
        }

        public void ShowAccountLinkPrompt()
        {
            CrazySDK.User.ShowAccountLinkPrompt(
                (error, answer) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Show account link prompt error: " + error);
                        return;
                    }

                    Debug.Log("Account link answer: " + answer);
                }
            );
        }

        public void AddAuthListener()
        {
            _authListeners.Add(
                (user) =>
                {
                    Debug.Log("Auth listener, user: " + user);
                }
            );
            CrazySDK.User.AddAuthListener(_authListeners.Last());
        }

        public void RemoveLastAuthListener()
        {
            if (_authListeners.Count == 0)
            {
                return;
            }

            var lst = _authListeners.Last();
            CrazySDK.User.RemoveAuthListener(lst);
            _authListeners.Remove(lst);
        }

        public void GetSystemInfo()
        {
            var systemInfo = CrazySDK.User.SystemInfo;
            Debug.Log($"All system info: {systemInfo}");
            Debug.Log($"Browser: {systemInfo.browser}");
            Debug.Log($"OS: {systemInfo.os}");
            Debug.Log($"Country code: {systemInfo.countryCode}");
            Debug.Log($"Device type: {systemInfo.device.type}");
            Debug.Log($"Application type: {systemInfo.applicationType}");
        }

        public void AddScore()
        {
            CrazySDK.User.AddScore(152.1f);
        }
    }
}
