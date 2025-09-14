using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private Slider slider;
    Enemy targetEnemy;

    void OnGUI(){
        rect.position = targetEnemy.transform.position + new Vector3(0,-0.2f,0);
        slider.value = (float)targetEnemy.CurrentHealth / targetEnemy.MaxHealth;
    }


    public void Init(GameObject enemy)
    {
        targetEnemy = enemy.GetComponent<Enemy>();
    }
}
