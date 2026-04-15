using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Newtonsoft.Json;

public class JSONToUIManager : MonoBehaviour
{
    [SerializeField] private bool isDeveloper;
    public bool canEdit() => isDeveloper;

    public Button developerButton;

    [SerializeField] private GameObject defaultTemplate;
    [SerializeField] private Transform pagesHolder;

    [SerializeField] private List<GameObject> prefabEntries;

    private Dictionary<string, GameObject> uiPrefabDictionary;
    private Navbar navbar;

    private DatabaseReference _dbReference;
    private const string DB_KEY = "UISetup";

    private void Awake()
    {
        uiPrefabDictionary = new();
        navbar = GetComponentInChildren<Navbar>(true);

        foreach (GameObject entry in prefabEntries)
        {
            if (entry.TryGetComponent<UIElementBase>(out var uiElement))
            {
                if (!uiPrefabDictionary.ContainsKey(uiElement.prefabID))
                    uiPrefabDictionary.Add(uiElement.prefabID, uiElement.gameObject);
            }
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError($"Firebase init failed: {task.Result}");
                LoadDefaultTemplate();
                return;
            }

            var db = FirebaseDatabase.GetInstance(
                "https://nnotest-e40e2-default-rtdb.europe-west1.firebasedatabase.app"
            );

            _dbReference = db.GetReference(DB_KEY);

            Construct();
        });
    }

    public void ClickDeveloperButton()
    {
        isDeveloper = !isDeveloper;

        developerButton.GetComponentInChildren<TextMeshProUGUI>().text =
            isDeveloper ? "Yes Dev" : "No Dev";

        navbar?.RefreshLists();
    }

    public void RemoveUI()
    {
        foreach (Transform child in pagesHolder)
            Destroy(child.gameObject);

        navbar?.RefreshLists();
    }

    private void LoadDefaultTemplate()
    {
        Instantiate(defaultTemplate, pagesHolder);
        navbar?.BootToHome();
    }


    public void Construct()
    {
        if (_dbReference == null)
        {
            Debug.LogWarning("Firebase not ready yet");
            return;
        }

        RemoveUI();

        _dbReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Task completed: {task.IsCompleted}, faulted: {task.IsFaulted}");

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogWarning("Firebase fetch failed  default UI");
                LoadDefaultTemplate();
                return;
            }

            DataSnapshot snapshot = task.Result;

            Debug.Log($"Snapshot exists: {snapshot.Exists}");

            string json = snapshot.GetRawJsonValue();
            Debug.Log($"RAW JSON:\n{json}");

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("Empty Firebase data  default UI");
                LoadDefaultTemplate();
                return;
            }

            UIElementData loadedData = null;

            try
            {
                loadedData = JsonConvert.DeserializeObject<UIElementData>(
                    json,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Deserialize error: {e}");
                LoadDefaultTemplate();
                return;
            }

            if (loadedData == null)
            {
                Debug.LogWarning("Deserialized data is null → default UI");
                LoadDefaultTemplate();
                return;
            }

            if (!uiPrefabDictionary.ContainsKey(loadedData.prefabID))
            {
                Debug.LogWarning($"Prefab ID not found: {loadedData.prefabID}");
                LoadDefaultTemplate();
                return;
            }

            Spawn(loadedData, pagesHolder);

            navbar?.BootToHome();
        });
    }


    public void SaveCurrentUI()
    {
        var root = GetComponentInChildren<UIElementBase>(true);
        if (!root)
        {
            Debug.LogWarning("No UI root found");
            return;
        }

        UIElementData data = GenerateDataFromUI(root);

        string json = JsonConvert.SerializeObject(
    data,
    Formatting.Indented,
    new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    }
);

        Debug.Log("SAVE JSON:\n" + json);

        _dbReference.SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError(" SAVE FAILED:");
                    Debug.LogError(task.Exception);
                    return;
                }

                if (task.IsCanceled)
                {
                    Debug.LogWarning(" SAVE CANCELED");
                    return;
                }

                Debug.Log(" SAVE SUCCESS");
            });
    }

    private UIElementData GenerateDataFromUI(UIElementBase uiElement)
    {
        UIElementData data = uiElement.SetupGenerateData();

        data.children = new List<UIElementData>();

        foreach (Transform child in uiElement.GetSpawnRoot())
        {
            if (child.TryGetComponent<UIElementBase>(out var childUI))
                data.children.Add(GenerateDataFromUI(childUI));
        }

        return data;
    }



    private void Spawn(UIElementData data, Transform parent)
    {
        if (!uiPrefabDictionary.TryGetValue(data.prefabID, out var prefab))
        {
            Debug.LogWarning($"Missing prefab: {data.prefabID}");
            return;
        }

        GameObject instance = Instantiate(prefab, parent);

        UIElementBase element = instance.GetComponent<UIElementBase>();
        element.SetupApplyData(data);

        if (data.children == null) return;

        foreach (var child in data.children)
            Spawn(child, element.GetSpawnRoot());
    }
}