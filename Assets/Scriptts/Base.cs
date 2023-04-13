using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Base : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    public LevelManager levelManager => _levelManager;
    [SerializeField] private GameObject _ownerObject;
    private IBaseOwner _iOwner;
    public IBaseOwner iOwner => _iOwner;
    [SerializeField] private PlayerData _data;
    public PlayerData data => _data;
    [SerializeField] private BaseVisual _baseVisual;
    public BaseVisual baseVisual => _baseVisual;
    
    [SerializeField] private float _mass;
    public float mass => _mass;
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private float _spawnUnitsBorder;
    [SerializeField] private float _massMax;
    public float massMax => _massMax;

    public UnityEvent<Vector2> OnDrawLine;
    public UnityEvent OnClearLine;
    public UnityEvent OnSelected;
    public UnityEvent OnUnselected;
    public UnityEvent OnMassChanged;
    public UnityEvent OnUnitTaken;
    public UnityEvent OnOwnerChanged;

    public void Init(LevelManager levelManager) {
        _levelManager = levelManager;
        
        if (_ownerObject != null) SetOwner(_ownerObject);

        _baseVisual.Init();

        StartCoroutine(BaseUpdate());
    }

    private IEnumerator BaseUpdate() {
        while (true)
        {
            if (_iOwner != null && _mass < _massMax) 
            {
                AddMass(1f);
                yield return new WaitForSeconds(_data.reproductionTime);
            }

            yield return null;
        };
    }

    private void SetOwner(GameObject ownerObject) {
        if (_iOwner != null)
        {
            _iOwner.RemoveBase(this);
        }

        _ownerObject = ownerObject;
        _iOwner = ownerObject.GetComponent<IBaseOwner>();

        if (_iOwner != null)
        {
            _data = _iOwner.GetData();

            _iOwner.AddBase(this);

            OnOwnerChanged?.Invoke();
        }
        else
        {
            Debug.LogError("Owner doesn't have a inteface");
        }
    }

    public void SendUnits(GameObject target) {
        if (target == gameObject) return;

        for (int i = 0; i < _mass; i++)
        {
            GameObject newUnit;

            Vector3 unitPos = new Vector3(Random.Range(-_spawnUnitsBorder, _spawnUnitsBorder), Random.Range(-_spawnUnitsBorder, _spawnUnitsBorder)) + transform.position;    

            newUnit = Instantiate(_unitPrefab, unitPos, Quaternion.identity);

            Unit unitSkript = newUnit.GetComponent<Unit>();

            unitSkript.SetTarget(target, _iOwner);
        }

        RemoveMass(_mass);
    }

    private void AddMass(float massToAdd) {
        _mass = _mass + massToAdd;
        OnMassChanged?.Invoke();
    }

    private void RemoveMass(float massToRemove) {
        _mass = _mass - massToRemove;
        OnMassChanged?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Unit unit = collision.attachedRigidbody.GetComponent<Unit>();
        
        if (unit != null)
        {
            if (unit.targetObject == gameObject)
            {

                if (unit.iOwner == _iOwner)
                {
                    AddMass(1f);
                }
                else
                {
                    if (_mass == 0f)
                    {
                        // Change base owner
                        SetOwner(unit.iOwner.GetData().owner);
                    } 
                    else
                    {
                        RemoveMass(1f);
                    }

                }

                OnUnitTaken?.Invoke();
                Destroy(unit.gameObject);
            }
        }
    }
}
