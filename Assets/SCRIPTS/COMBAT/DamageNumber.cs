using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public TMP_Text text;

    public float lifetime = 1f;
    public float floatSpeed = 80f;

    private float timer;

    public Color normalColor = Color.white;
    public Color playerDamageColor = Color.red;

    public void Setup(int damage, bool crit = false, bool isPlayer = false)
    {
        text.text = damage.ToString();

        text.color =
            isPlayer
            ? playerDamageColor
            : normalColor;

        if (crit)
        {
            text.text += "!";
            text.fontSize = 48;
        }
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.unscaledDeltaTime;

        timer += Time.unscaledDeltaTime;

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}