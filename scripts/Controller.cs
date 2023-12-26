using UnityEngine;
using UniRx;
using System;
//top
public class Controller : MonoBehaviour
{
    public HealthBonus _healthBonusPref;
    public int _procentBonusHealth = 30;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _effectSource;
    [SerializeField] private AudioClip _clipShot;

    public ReactiveProperty<int> Score = new ReactiveProperty<int>();

    public AudioSource MusicSource => _musicSource;
    public AudioSource EffectSource => _effectSource;

    public static Controller Instance;
    public PlayerShip _myShip;

    private Vector3 _leftDownPoint;
    private Vector3 _rightDownPoint;
    private Vector3 _leftUpPoint;
    private Vector3 _rightUpPoint;
    private Vector2 _centrCam;

    public Vector3 LeftDownPoint => _leftDownPoint;
    public Vector3 RightDownPoint => _rightDownPoint;
    public Vector3 LeftUpPoint => _leftUpPoint;
    public Vector3 RightUpPoint => _rightUpPoint;
    public Vector2 CentrCam => _centrCam;

    private Subject<Unit> _gameOver = new Subject<Unit>();
    public IObservable<Unit> OnGameOver => _gameOver;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayAudioShot()
    {
        _effectSource.PlayOneShot(_clipShot);
    }

    public void UpdateCameraSettings()
    {
        var cameraMain = Camera.main;
        if (cameraMain != null)
        {
            _centrCam = cameraMain.transform.position;

            float distance = cameraMain.farClipPlane;
            _leftDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(0, 0, distance));
            _leftUpPoint = cameraMain.ScreenToWorldPoint(new Vector3(0, cameraMain.pixelHeight, distance));
            _leftDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(cameraMain.pixelWidth, cameraMain.pixelHeight, distance));
            _leftDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(cameraMain.pixelWidth, 0, distance));
        }
    }

    public void GameOver()
    {
        _gameOver.OnNext(Unit.Default);
    }
}
