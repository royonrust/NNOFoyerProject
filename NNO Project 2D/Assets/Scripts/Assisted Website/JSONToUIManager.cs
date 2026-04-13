using System;
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

    //      temp
    
    public Button developerButton;

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
            DestroyImmediate(child.gameObject);
        
        navbar?.RefreshLists();
    }
    
    //      /temp
    
    [SerializeField] private GameObject defaultTemplate;
    [SerializeField] private Transform pagesHolder;
    
    private void LoadDefaultTemplate()
    {
        Instantiate(defaultTemplate, pagesHolder);
        navbar?.BootToHome();
    }
    
    [SerializeField] private List<GameObject> prefabEntries;
    private Dictionary<string, GameObject> uiPrefabDictionary;
    
    private Navbar navbar;

    private void Awake()
    {
        uiPrefabDictionary = new();

        navbar = GetComponentInChildren<Navbar>(true);

        foreach (GameObject entry in prefabEntries)
        {
            if (entry.TryGetComponent<UIElementBase>(out var uiElement))
                if (!uiPrefabDictionary.ContainsKey(uiElement.prefabID))
                    uiPrefabDictionary.Add(uiElement.prefabID, uiElement.gameObject);
        }
        
        Construct();
    }

    public void Construct()
    {
        RemoveUI();
        
        string path = Path.Combine(Application.persistentDataPath, "UISetup.json");

        if (!File.Exists(path) || String.IsNullOrEmpty(File.ReadAllText(path)))
        {
            Debug.LogWarning("No valid save file found, loaded default template");
            LoadDefaultTemplate();
            return;
        }

        string json = File.ReadAllText(path);
        
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

        if (!uiPrefabDictionary.ContainsKey(loadedData.prefabID))
        {
            Debug.LogWarning("First child ID not present anymore, aborting build and loading default template");
            LoadDefaultTemplate();
            return;
        }

        Spawn(loadedData, pagesHolder);
        navbar?.BootToHome();
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
        
        foreach (Transform child in uiElement.GetSpawnRoot())
            if (child.TryGetComponent<UIElementBase>(out var childUI))
                data.children.Add(GenerateDataFromUI(childUI));

        return data;
    }
    
    private void Spawn(UIElementData data, Transform parent)
    {
        GameObject prefab = uiPrefabDictionary[data.prefabID];
        GameObject instance = Instantiate(prefab, parent);

        UIElementBase element = instance.GetComponent<UIElementBase>();
        element.SetupApplyData(data);
        
        foreach (var child in data.children)
            Spawn(child, element.GetSpawnRoot());
    }
}