using UnityEngine;
using UnityEngine.UI;

public class TargetMarker : MonoBehaviour
{
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private Image _targetImage;
    [SerializeField] private Canvas _canvas;

    private void Update()
    {
        if (_detector != null && _targetImage != null)
        {
            if (_detector.NearestEnemy.InRange)
            {
                _targetImage.gameObject.SetActive(true);
                Vector3 center = _detector.NearestEnemy.Collider.bounds.center;
                float maxDistance = _detector.MaxTargetingDistance;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(center);

                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    screenPos,
                    _canvas.worldCamera,
                    out canvasPos);

                _targetImage.rectTransform.anchoredPosition = canvasPos;
            }
            else
            {
                _targetImage.gameObject.SetActive(false);
            }
        }
    }
}
