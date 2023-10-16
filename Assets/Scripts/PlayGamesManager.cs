using UnityEngine;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class PlayGamesManager : MonoBehaviour
{
    [SerializeField] private GameObject SinglePlayerButton;
    [SerializeField] private GameObject PracticeButton;
    [SerializeField] private GameObject SigninButton;
    [SerializeField] private GameObject GuestButton;
    [SerializeField] private TextMeshProUGUI ConfirmText;

    private void Start()
    {
        Signin();
    }

    public void Signin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            ConfirmText.text = "";
            // Continue with Play Games Services
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string userImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

            SinglePlayerButton.SetActive(true);
            PracticeButton.SetActive(true);
            SigninButton.SetActive(false);
            GuestButton.SetActive(false);
        }
        else
        {
            ConfirmText.text = "Signin Failed...";
            // Debug.Log(status);
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication)
        }
    }

    public void GuestSignin()
    {
        ConfirmText.text = "";

        SinglePlayerButton.SetActive(true);
        PracticeButton.SetActive(true);
        SigninButton.SetActive(false);
        GuestButton.SetActive(false);
    }
}
