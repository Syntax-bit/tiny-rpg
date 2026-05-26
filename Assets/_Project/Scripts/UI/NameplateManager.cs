using System.Collections.Generic;
using TinyRPG.Gameplay;
using UnityEngine;

namespace TinyRPG.UI
{
    public class NameplateManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float maxVisibilityDistance = 20f;

        [Header("References")]
        [SerializeField] private GameObject nameplatePrefab;
        [SerializeField] private GameObject nameplateHolder;

        private Dictionary<Unit, RectTransform> activeNameplates = new Dictionary<Unit, RectTransform>();

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        /// <summary>
        /// Creates a new nameplate object for a specified unit
        /// </summary>
        /// <param name="newUnit"></param>
        public void CreateNewNameplate(Unit unit)
        {
            if (activeNameplates.ContainsKey(unit) || unit == null) return;

            GameObject nameplate = Instantiate(nameplatePrefab, nameplateHolder.transform);
            RectTransform rectTransform = nameplate.GetComponent<RectTransform>();
            Nameplate nameplateUI = nameplate.GetComponent<Nameplate>();

            nameplateUI.Initialize(unit);

            activeNameplates.Add(unit, rectTransform);
        }

        public void RemoveNameplate(Unit unit)
        {
            if (activeNameplates.TryGetValue(unit, out RectTransform rectTransform))
            {
                Destroy(rectTransform.gameObject);
                activeNameplates.Remove(unit);
            }
        }

        private void LateUpdate()
        {
            foreach (var pair in activeNameplates)
            {
                Unit unit = pair.Key;
                RectTransform rectTransform = pair.Value;

                Vector3 worldPos = unit.transform.position + Vector3.up * 2;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPos);

                if (screenPosition.z < 0)
                {
                    rectTransform.gameObject.SetActive(false);
                }
                else
                {
                    rectTransform.gameObject.SetActive(true);
                    rectTransform.position = new Vector3(screenPosition.x, screenPosition.y, 0);
                }
            }
        }
    }
}