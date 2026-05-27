using System.Collections.Generic;
using TinyRPG.Gameplay;
using TinyRPG.Player;
using UnityEngine;

namespace TinyRPG.UI
{
    public class NameplateManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float minNameplateOpacity = .2f;
        [SerializeField] private float fadeStartDistance = 5f;

        [Header("References")]
        [SerializeField] private GameObject nameplatePrefab;
        [SerializeField] private GameObject nameplateHolder;

        private Dictionary<Unit, Nameplate> activeNameplates = new Dictionary<Unit, Nameplate>();

        private Camera mainCamera;
        private PlayerNameplateDetector playerNameplateDetector;
        private PlayerTargeter playerTargeter;

        private void Awake()
        {
            mainCamera = Camera.main;

            Transform player = GameObject.Find("Player").transform;

            playerTargeter = player.GetComponent<PlayerTargeter>();
            playerNameplateDetector = player.GetComponentInChildren<PlayerNameplateDetector>();
        }

        /// <summary>
        /// Creates a new nameplate object for a specified unit
        /// </summary>
        /// <param name="newUnit"></param>
        public void CreateNewNameplate(Unit unit)
        {
            if (activeNameplates.ContainsKey(unit) || unit == null) return;

            GameObject nameplate = Instantiate(nameplatePrefab, nameplateHolder.transform);
            Nameplate nameplateUI = nameplate.GetComponent<Nameplate>();

            nameplateUI.Initialize(unit);

            activeNameplates.Add(unit, nameplateUI);
        }

        public void RemoveNameplate(Unit unit)
        {
            if (activeNameplates.TryGetValue(unit, out Nameplate nameplate))
            {
                Destroy(nameplate.gameObject);
                activeNameplates.Remove(unit);
            }
        }

        private void LateUpdate()
        {
            foreach (var pair in activeNameplates)
            {
                Unit unit = pair.Key;
                Nameplate nameplate = pair.Value;

                Vector3 worldPos = unit.transform.position + Vector3.up * 2;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPos);

                if (screenPosition.z < 0)
                {
                    nameplate.gameObject.SetActive(false);
                }
                else
                {
                    nameplate.gameObject.SetActive(true);
                    nameplate.transform.position = new Vector3(screenPosition.x, screenPosition.y, 0);

                    float distance = Vector3.Distance(unit.transform.position,
                        playerNameplateDetector.gameObject.transform.position);

                    if(distance <= fadeStartDistance || unit == playerTargeter.selectedUnit)
                    {
                        nameplate.SetOpacity(1);
                    }
                    else
                    {
                        float fadePercentage = Mathf.InverseLerp(playerNameplateDetector.maxVisibilityDistance, fadeStartDistance, distance);

                        float finalOpacity = Mathf.Max(minNameplateOpacity, fadePercentage);
                        nameplate.SetOpacity(finalOpacity);
                    }
                }
            }
        }
    }
}