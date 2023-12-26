using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;

public class GameUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _countHealth;
    [SerializeField] TextMeshProUGUI _countScore;
    [SerializeField] Slider _barHealth;
    [SerializeField] TextMeshProUGUI _countScoreWindowGameOver;
    [SerializeField] GameObject _windowGameOver;

    private CompositeDisposable _disposables;

    private void Start()
    {
        _disposables = new CompositeDisposable();
        var controller = Controller.Instance;

        controller.OnGameOver.Subscribe((_) => ShowWindowGameOver()).AddTo(_disposables);
        controller._myShip._health.Subscribe(UpdateBar).AddTo(_disposables);
        controller.Score.Subscribe(UpdateScore).AddTo(_disposables);
    }

    private void UpdateBar(int value)
    {
        _barHealth.value = ((float)value)/100;
        _countHealth.text = value.ToString();
    }

    private void UpdateScore(int score)
    {
        if (!_windowGameOver.activeSelf)
        {
            _countScore.text = score.ToString();
        }
    }
    public void ShowWindowGameOver()
    {
        _countScoreWindowGameOver.text = Controller.Instance.Score.Value.ToString();
        _windowGameOver.SetActive(true);
    } 

    public void ClickToMainMenu()
    {
        LevelManager.PlayScene(Scenes.MainMenu);
        gameObject.SetActive(false);
    }

    public void ClickRestart()
    {
        LevelManager.PlayScene(Scenes.Game);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_disposables != null)
        {
            _disposables.Dispose();
        }
        _disposables = null;
    }
}