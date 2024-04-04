using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Scripts
{
    public class LobbyUI : MonoBehaviour
    {
        public void InputLogin(string login) => 
            PlayerSettings.Instance.SetLogin(login);

        public void ClickConnect()
        {
            if(string.IsNullOrEmpty(PlayerSettings.Instance.Login))
                return;

            SceneManager.LoadScene("Game");
        }
    }
}
