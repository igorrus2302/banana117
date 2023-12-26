using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _effects;
    [SerializeField] private float _speed = 15;
    public int _damage = 10;
    [SerializeField] private float _coolDown = 0.1f;
    public int _maxHealth = 100;
    [SerializeField] private float _shipRollEuler = 45;
    [SerializeField] private float _shipRollSpeed = 80;
    [SerializeField] private float _smothness = 1.2f;

    private Subject<Unit> _fireClick = new Subject<Unit>();
    public IObservable<Unit> FireClick => _fireClick;

    private Rigidbody2D _rigidbody;
    private float _coolDownCurrent = 10;
    private MeshRenderer _mR;
    private Vector3 _sizeWorldShip;
    private Controller _controller;

    [HideInInspector] public ReactiveProperty<int> _health = new ReactiveProperty<int>();

    private void Awake()
    {
        if (Controller.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _rigidbody = GetComponent<Rigidbody2D>();
        _mR = GetComponent<MeshRenderer>();
        _controller = Controller.Instance;
        _controller._myShip = this;
        _sizeWorldShip = _mR.bounds.extents;
    }

    private void Start()
    {
        _controller.UpdateCameraSettings();
        _health.Value = _maxHealth;
    }

    private void Update()
    {
        UpdateKey();
        FireButtonClick();
    }

    private void FireButtonClick()
    {
        if (Input.GetMouseButton(0))
        {
            if (_coolDownCurrent >= _coolDown)
            {
                _coolDownCurrent = 0;
                _fireClick.OnNext(Unit.Default);
            }
        }
        if (_coolDownCurrent < _coolDown)
        {
            _coolDownCurrent += Time.deltaTime;
        }
    }

    private void UpdateKey()
    {
        float moveHor = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");


        if (moveVert > 0) _effects[4].Play();
        else { _effects[4].Stop(); }
        if (moveVert < 0)
        {
            _effects[2].Play();
            _effects[3].Play();
        }
        else
        {
            _effects[2].Stop();
            _effects[3].Stop();
        }
        if (moveHor > 0) _effects[0].Play();
        else _effects[0].Stop();
        if (moveHor < 0) _effects[1].Play();
        else _effects[1].Stop();

        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, new Vector2(moveHor * _speed * 1.2f, moveVert * _speed), _smothness);

        transform.position = CheckBoardWorld();


        var targetRotation = Quaternion.Euler(0, 180 + (-moveHor * _shipRollEuler), 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _shipRollSpeed * Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.PlayScene(Scenes.MainMenu);
        }
    }

    private Vector3 CheckBoardWorld()
    {
        var pos = transform.position;
        var x = pos.x;
        var y = pos.y;

        x = Mathf.Clamp(x, _controller.LeftDownPoint.x + _sizeWorldShip.x, _controller.RightDownPoint.x - _sizeWorldShip.x );
        y = Mathf.Clamp(y, _controller.LeftDownPoint.y + _sizeWorldShip.y, _controller.LeftUpPoint.y - _sizeWorldShip.y);
        return new Vector3(x, y, 0);
    }

    public void DamageMe()
    {
        _health.Value -= _damage;
        if (_health.Value <= 0)
        {
            var tr = transform;
            var position = tr.position;
            gameObject.SetActive(false);
            _controller.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("EnemyBullet"))
        {
            var bull = obj.GetComponent<Bullet>();
            DamageMe();
            bull.HitMe();
        }

        if (obj.CompareTag("AddHealth"))
        {
            var bonus = obj.GetComponent<HealthBonus>();
            bonus.CallMoveToBar();
            _health.Value += bonus.Health;
            if (_health.Value > _maxHealth)
            {
                _health.Value = _maxHealth;
            }
        }
    }
}
