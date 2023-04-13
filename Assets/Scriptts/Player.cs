using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerCore, IBaseOwner
{
    [SerializeField] private PlayerData _data;
    public PlayerData data => _data;

    private List<Base> _bases = new List<Base>();
    private List<Base> _selectedBases = new List<Base>();
    private Base _targetBase;
    private Vector3 _targetPosition;
    public override void Init(LevelManager levelManager) {
        _levelManager = levelManager;
    }

    private void Update() {
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            _targetPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);

            Collider2D targetCollider = Physics2D.Raycast(_targetPosition, transform.position).collider;

            AddBase(targetCollider, _targetPosition);
        }
#endif
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D targetCollider = Physics2D.Raycast(_targetPosition, transform.position).collider;

            AddBase(targetCollider, _targetPosition);
        }
#endif
        else if (_selectedBases.Count != 0)
        {
            if (_targetBase != null)
            {
                // Send units
                foreach (Base myBase in _selectedBases)
                {
                    if (myBase.data == _data)
                    {
                        myBase.SendUnits(_targetBase.gameObject);
                    }
                }
            }

            _targetBase?.OnUnselected?.Invoke();

            foreach (Base myBase in _selectedBases)
            {
                if (myBase.data == _data)
                {
                    myBase.OnClearLine?.Invoke();
                }
            }

            _selectedBases.Clear();
        }
    }

    private void AddBase(Collider2D collider2D, Vector3 targetPosition) {
        if (collider2D != null)
        {
            Base newBase = collider2D.attachedRigidbody.GetComponent<Base>();

            if (newBase != null)
            {
                _targetBase?.OnUnselected?.Invoke();
                newBase.OnSelected?.Invoke();

                if (newBase.data == _data && (!_selectedBases.Contains(newBase)))
                {
                    _selectedBases.Add(newBase);
                }

                _targetBase = newBase;
            }
        }
        else
        {
            _targetBase?.OnUnselected?.Invoke();
            _targetBase = null;
        }

        // Draw line from selected bases to touch point
        if (_selectedBases.Count != 0)
        {
            foreach (Base myBase in _selectedBases)
            {
                if (myBase.data == _data)
                {
                    myBase.OnDrawLine?.Invoke(_targetPosition);
                }
            }
        }
    }

    public PlayerData GetData() {
        return _data;
    }

    public void AddBase(Base baseArg) {
        _bases.Add(baseArg);
    }

    public void RemoveBase(Base baseArg) {
        if (_bases.Contains(baseArg)) {
            _bases.Remove(baseArg);
        }
    }
}