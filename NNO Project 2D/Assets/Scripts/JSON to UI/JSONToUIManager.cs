using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;

public class JSONToUIManager : MonoBehaviour
{
    [SerializeField] private bool isDeveloper;
    public bool canEdit() => isDeveloper;

    public Button developerButton;

    public void ClickDeveloperButton()
    {
        isDeveloper = !isDeveloper;
        developerButton.GetComponentInChildren<TextMeshProUGUI>().text =
            isDeveloper ? "Yes Developer" : "No Developer";
    }
    
    [SerializeField] private Transform pagesHolder;
    [SerializeField] private List<GameObject> prefabEntries;

    private Dictionary<string, GameObject> uiPrefabDictionary;

    private void Awake()
    {
        uiPrefabDictionary = new();

        foreach (GameObject entry in prefabEntries)
        {
            if (entry.TryGetComponent<UIElementBase>(out var uiElement))
                if (!uiPrefabDictionary.ContainsKey(uiElement.prefabID))
                    uiPrefabDictionary.Add(uiElement.prefabID, uiElement.gameObject);
        }
    }

    public void Construct()
    {
        string path = Path.Combine(Application.persistentDataPath, "UISetup.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(path);

        Debug.Log("Succesfully loaded: " + path);
        
        UIElementData loadedData = JsonConvert.DeserializeObject<UIElementData>(
            json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = false
                }
            });

        SpawnNewUIPrefab(loadedData, pagesHolder);
    }
    
    public void SaveCurrentUI()
    {
        if (!GetComponentInChildren<UIElementBase>(true)) return;
        UIElementData fullData = GenerateDataFromUI(GetComponentInChildren<UIElementBase>(true));

        string json = JsonConvert.SerializeObject(
            fullData, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = false
                }
            });
        
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "UISetup.json"), json);
        Debug.Log("UI hierarchy saved!");
    }
    
    private UIElementData GenerateDataFromUI(UIElementBase uiElement)
    {
        UIElementData data = uiElement.SetupGenerateData();
        data.children = new List<UIElementData>();
        
        foreach (Transform child in uiElement.transform)
            if (child.TryGetComponent<UIElementBase>(out var childUI))
                data.children.Add(GenerateDataFromUI(childUI));

        return data;
    }
    
    private void SpawnNewUIPrefab(UIElementData data, Transform parent)
    {
        if (!string.IsNullOrEmpty(data.parentIdentifier))
        {
            var foundParent = GameObject.Find(data.parentIdentifier);
            if (foundParent != null) parent = foundParent.transform;
        }

        GameObject newUI = Instantiate(uiPrefabDictionary[data.prefabID], parent);
        
        if (newUI.TryGetComponent<UIElementBase>(out var uiElement))
            uiElement.SetupApplyData(data);
        
        if (data.children != null)
            foreach (var childData in data.children)
                SpawnNewUIPrefab(childData, newUI.transform);
    }

    public void RemoveUI()
    {
        foreach (Transform child in pagesHolder) Destroy(child.gameObject);
    }
}