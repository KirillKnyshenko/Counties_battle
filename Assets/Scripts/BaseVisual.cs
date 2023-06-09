using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BaseVisual : MonoBehaviour
{
    private static int POPUP = Animator.StringToHash("Popup");
    [SerializeField] private float _colorSpeed;
    [SerializeField] private Base _base;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _selectedSpriteRenderer;
    [SerializeField] private SpriteRenderer _arrorSpriteRenderer;
    [SerializeField] private SpriteRenderer _fieldSpriteRenderer;
    [SerializeField] private Transform _arrorTransform;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _maxMassVisual;
    [SerializeField] private GameObject _selectedVisual;
    public LineRenderer LineRenderer => _lineRenderer;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Animator _baseAnimator;

    public void Init() {
        _base.OnDrawLine.AddListener(DrawLine);
        _base.OnClearLine.AddListener(ClearLine);
        
        _base.OnSelected.AddListener(ShowSelection);
        _base.OnUnselected.AddListener(HideSelection);

        _base.OnMassUpdate.AddListener(UpdateVisual);

        _base.OnUnitTaken.AddListener(BasePopup);

        _base.OnOwnerChanged.AddListener(SetOwnerVisual);
        _base.OnOwnerChanged.AddListener(ClearLine);

        SetOwnerVisual();
        ClearLine();
        HideSelection();
        UpdateVisual();
        StartCoroutine(ChangingFieldColor());
    }

    private void SetOwnerVisual() {
        if (_base.playerCore != null)
        {
            _spriteRenderer.color = _base.data.color;

            float alfaColor = .3f;
            Color selectedColor = new Color(_base.data.color.r, _base.data.color.g, _base.data.color.b, alfaColor);
            _selectedSpriteRenderer.color = selectedColor;

            _lineRenderer.material = _base.data.lineMaterial;
            _arrorSpriteRenderer.material = _base.data.lineMaterial;
        }
    }

    private void UpdateVisual() {
        _countText.text = _base.mass.ToString();

        if (_base.massMax <= _base.mass)
        {
            _maxMassVisual.SetActive(true);
        }
        else
        {
            _maxMassVisual.SetActive(false);
        }
    }

    private IEnumerator ChangingFieldColor() {
        while (true)
        {
            float colorCombining = _base.mass / (float)_base.massMax;
            Color fieldColor = _base.data.fieldColor.Evaluate(colorCombining);
            _fieldSpriteRenderer.color = Color.Lerp(_fieldSpriteRenderer.color, fieldColor, Time.deltaTime * _colorSpeed);

            yield return null;
        }   
    }

    private void DrawLine(Vector2 targetPosition) {
        
        _arrorTransform.position = targetPosition;

        // Set target to the center of arrow for better line visual
        targetPosition = _arrorSpriteRenderer.transform.position;

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, targetPosition);
        
        // Rotate arrow
        var dir = targetPosition - (Vector2)transform.position;
        var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
        _arrorTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ClearLine() {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
        
        _arrorTransform.position = (Vector2)transform.position;
    }

    private void ShowSelection() {
        _selectedVisual.SetActive(true);
    }

    private void HideSelection() {
        _selectedVisual.SetActive(false);
    }

    private void BasePopup() {
        _baseAnimator.SetTrigger(POPUP);
    }
}
