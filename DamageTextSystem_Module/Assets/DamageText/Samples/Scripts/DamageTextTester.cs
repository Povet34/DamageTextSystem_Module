using UnityEngine;
using Povet.DamageText.Core;

namespace Povet.DamageText.Samples
{
    public class DamageTextTester : MonoBehaviour
    {
        [Header("Test Styles")]
        [SerializeField] private string normalStyle = "Normal";
        [SerializeField] private string criticalStyle = "Critical";
        [SerializeField] private string missStyle = "Miss";
        [SerializeField] private string weaknessStyle = "Weakness";

        [Header("Settings")]
        [SerializeField] private int minDamage = 10;
        [SerializeField] private int maxDamage = 999;

        private Camera cam;

        private void Start()
        {
            cam = Camera.main;
            Debug.Log("=== Damage Text Tester ===");
            Debug.Log("좌클릭: 랜덤 데미지 생성");
            Debug.Log("1: Normal / 2: Critical / 3: Miss / 4: Weakness");
            Debug.Log("Space: 랜덤 50개 생성");
            Debug.Log("C: 전체 삭제");
        }

        private void Update()
        {
            // 마우스 좌클릭
            if (Input.GetMouseButtonDown(0))
            {
                SpawnAtMousePosition(normalStyle);
            }

            // 키보드 입력
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnAtMousePosition(normalStyle);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SpawnAtMousePosition(criticalStyle);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SpawnAtMousePosition(missStyle);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SpawnAtMousePosition(weaknessStyle);
            }

            // Space: 대량 생성
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < 50; i++)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(-5f, 5f),
                        Random.Range(0f, 3f),
                        Random.Range(-5f, 5f)
                    );

                    string[] styles = { normalStyle, criticalStyle, missStyle, weaknessStyle };
                    string style = styles[Random.Range(0, styles.Length)];
                    int damage = Random.Range(minDamage, maxDamage);

                    DamageTextController.Instance.Show(damage, style, randomPos);
                }

                Debug.Log("50개 생성 완료");
            }

            // C: 전체 삭제
            if (Input.GetKeyDown(KeyCode.C))
            {
                DamageTextController.Instance.ClearAll();
                Debug.Log("전체 삭제");
            }
        }

        private void SpawnAtMousePosition(string styleName)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int damage = Random.Range(minDamage, maxDamage);
                Vector3 spawnPos = hit.point + Vector3.up * 0.5f;
                DamageTextController.Instance.Show(damage, styleName, spawnPos);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 120, 300, 150));
            GUILayout.Box("=== Controls ===");
            GUILayout.Label("좌클릭: Normal 생성");
            GUILayout.Label("1: Normal / 2: Critical");
            GUILayout.Label("3: Miss / 4: Weakness");
            GUILayout.Label("Space: 랜덤 50개");
            GUILayout.Label("C: 전체 삭제");
            GUILayout.EndArea();
        }
    }
}