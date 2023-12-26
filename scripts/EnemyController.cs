using System.Collections;
using UnityEngine;
using UniRx;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _minDelay = 2; //in seconds
    [SerializeField] private float _maxDelay = 4;
    [SerializeField] private int _maxCountOneSpawn = 5;
    private float _timerDelay;
    private int _countOnePull;
    private SpawnManager _spawnManager;
    private CompositeDisposable _disposablesEnemy = new CompositeDisposable();

    private void Awake()
    {
        _spawnManager = GetComponent<SpawnManager>();
        _timerDelay = Random.Range(_minDelay, _maxDelay);
    }

    private void OnEnable()
    {
        _disposablesEnemy = new CompositeDisposable();
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        while(true)
        {
            _timerDelay -= Time.deltaTime;
            if (_timerDelay < 0)
            {
                _countOnePull = Random.Range(1, _maxCountOneSpawn);
                _timerDelay = Random.Range(_minDelay, _maxDelay);
                for (int i = 0; i < _countOnePull; i++)
                {
                    var hunter = _spawnManager.SpawnEnemy();
                    if (hunter != null)
                    {
                        hunter.Fire.Subscribe((param) => Fire(param.Item1, param.Item2)).AddTo(_disposablesEnemy);
                    }
                    yield return null;
                }
                _countOnePull = Random.Range(1, _maxCountOneSpawn);
            }
            yield return null;
        }
    }

    private void Fire(Transform tr, Bullet bullet)
    {
        _spawnManager.SpawnBullet(tr, bullet);
    }

    private void OnDisable()
    {
        if (SpawnEnemy() != null)
        {
            StopCoroutine(SpawnEnemy());
            //_coroutine = null;
        }
        _disposablesEnemy.Dispose();
        _disposablesEnemy = null;
    }

}
