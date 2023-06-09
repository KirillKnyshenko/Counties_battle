using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    private GameManager _gameManager;
    public GameManager gameManager => _gameManager;

    [SerializeField] private List<Base> _bases;
    public List<Base> bases => _bases;

    [SerializeField] private List<PlayerCore> _players;
    public List<PlayerCore> players => _players;
    [SerializeField] private int _poolCount;
    public bool _autoExpand = true;
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private Transform _poolContainer;
    
    public static PoolMono<Unit> _pool;
    public PoolMono<Unit> pool => _pool;

    public int unitLoopBonus;

    public void Init(GameManager gameManager) {
        _gameManager = gameManager;
        _gameManager.OnStartLevel.AddListener(ClearPool);

        foreach (var myBase in _bases)
        {
            myBase.Init(this, gameManager.saveData.loop);
        }

        foreach (var player in _players)
        {
            player.Init(this);
        }

        if (_pool == null || _pool.container == null)
        {
            _pool = new PoolMono<Unit>(_unitPrefab, _poolCount, _poolContainer);
            _pool.autoExpand = _autoExpand;
        }
        else
        {
            Destroy(_poolContainer.gameObject);
        }
    }

    public float GetSumMass() {
        float sumMass = 0f;

        foreach (var myBase in bases)
        {
            sumMass += myBase.mass;
        }

        foreach (var player in players)
        {
            sumMass += player.unitMass;
        }

        return sumMass;
    }

    private void ClearPool() {
        _pool.ClearPool();
    }
}
