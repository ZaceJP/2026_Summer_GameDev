using UnityEngine;
using System.Collections;

public enum IndicatorShape
{
    Circle,
    Cone,
    Rectangle,
    Ring
}

public class AttackIndicator : MonoBehaviour
{
    [Header("Shape Objects")]
    [SerializeField] private GameObject circleVisual;
    [SerializeField] private GameObject coneVisual;
    [SerializeField] private GameObject rectangleVisual;
    [SerializeField] private GameObject ringVisual;

    [Header("Renderers")]
    [SerializeField] private Renderer circleRenderer;
    [SerializeField] private Renderer coneRenderer;
    [SerializeField] private Renderer rectangleRenderer;
    [SerializeField] private Renderer ringRenderer;

    private Renderer activeRenderer;

    private void DisableAll()
    {
        if (circleVisual != null)
            circleVisual.SetActive(false);

        if (coneVisual != null)
            coneVisual.SetActive(false);

        if (rectangleVisual != null)
            rectangleVisual.SetActive(false);

        if (ringVisual != null)
            ringVisual.SetActive(false);
    }

    public void Initialize(
        IndicatorShape shape,
        float width,
        float length = 0f,
        float angle = 0f)
    {
        DisableAll();

        switch (shape)
        {
            case IndicatorShape.Circle:

                circleVisual.SetActive(true);
                activeRenderer = circleRenderer;

                circleVisual.transform.localScale =
                    new Vector3(width * 2f, 1f, width * 2f);

                break;

            case IndicatorShape.Cone:

                coneVisual.SetActive(true);
                activeRenderer = coneRenderer;

                // Width = attack range
                coneVisual.transform.localScale =
                    new Vector3(width, 1f, width);

                // Rotate so the cone is centered
                coneVisual.transform.localRotation =
                    Quaternion.Euler(0f, -angle * 0.5f, 0f);

                break;

            case IndicatorShape.Rectangle:

                rectangleVisual.SetActive(true);
                activeRenderer = rectangleRenderer;

                rectangleVisual.transform.localScale =
                    new Vector3(width, 1f, length);

                break;

            case IndicatorShape.Ring:

                ringVisual.SetActive(true);
                activeRenderer = ringRenderer;

                ringVisual.transform.localScale =
                    new Vector3(width * 2f, 1f, width * 2f);

                break;
        }
    }

    public IEnumerator Play(float duration)
    {
        if (activeRenderer == null)
            yield break;

        Material mat = activeRenderer.material;
        Color color = mat.color;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            color.a = Mathf.Lerp(0.25f, 0.85f, t);

            mat.color = color;

            yield return null;
        }
    }
}