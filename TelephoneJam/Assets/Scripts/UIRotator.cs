using UnityEngine;

public class UIRotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 90f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    // Public method to change rotation speed at runtime
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}
