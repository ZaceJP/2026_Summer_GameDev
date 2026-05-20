using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance;

    public Canvas canvas;
    public GameObject damageNumberPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(int damage,Vector3 worldPos,bool crit = false,bool isPlayer = false)
    {
        Vector3 screenPos =
            Camera.main.WorldToScreenPoint(worldPos);

        GameObject obj =
            Instantiate(
                damageNumberPrefab,
                screenPos,
                Quaternion.identity,
                canvas.transform
            );

        DamageNumber dmg =
            obj.GetComponent<DamageNumber>();

        dmg.Setup(damage, crit, isPlayer);
    }
}