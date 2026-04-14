using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class JSONToUIManager : MonoBehaviour
{
    [SerializeField] private bool isDeveloper;
    public bool canEdit() => isDeveloper;

    //      temp
    
    public TextMeshProUGUI developerButton;

    public void ClickDeveloperButton()
    {
        isDeveloper = !isDeveloper;
        developerButton.text =
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

        UIMainPage[] openPages = pagesHolder.GetComponentsInChildren<UIMainPage>();
        foreach (UIMainPage page in openPages)
            page.RefreshPageDimensions();
    }
    
    public List<GameObject> prefabEntries;
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
            Debug.LogWarning("First child ID not present anymore, aborting build and loading default template: " + loadedData.prefabID);
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
    
    private UIElementData GenerateDataFromUI(UIElementBase uiElement, string spawnRootIdentifier = null)
    {
        UIElementData data = uiElement.GenerateData();
        data.spawnRootIdentifier = spawnRootIdentifier;
        data.children = new List<UIElementData>();

        // Check named spawn roots first
        if (uiElement.namedSpawnRoots != null && uiElement.namedSpawnRoots.Count > 0)
        {
            foreach (var namedRoot in uiElement.namedSpawnRoots)
            {
                foreach (Transform child in namedRoot.root)
                    if (child.TryGetComponent<UIElementBase>(out var childUI))
                        data.children.Add(GenerateDataFromUI(childUI, namedRoot.identifier));
            }
        }
        else
        {
            foreach (Transform child in uiElement.GetSpawnRoot())
                if (child.TryGetComponent<UIElementBase>(out var childUI))
                    data.children.Add(GenerateDataFromUI(childUI));
        }

        return data;
    }
    
    private void Spawn(UIElementData data, Transform parent, UIElementBase parentElement = null)
    {
        if (!uiPrefabDictionary.TryGetValue(data.prefabID, out GameObject prefab))
        {
            Debug.LogWarning("A child's ID does not exist in the dictionary: " + data.prefabID);
            return;
        }

        Transform spawnParent = parent;
        if (parentElement != null && !string.IsNullOrEmpty(data.spawnRootIdentifier))
            spawnParent = parentElement.GetSpawnRoot(data.spawnRootIdentifier);

        GameObject instance = Instantiate(prefab, spawnParent);
        UIElementBase element = instance.GetComponent<UIElementBase>();
        element.ApplyGeneralData(data);
        
        foreach (Transform child in element.GetAllSpawnRoots())
            if (child.TryGetComponent(out UIElementBase _))
                Destroy(child.gameObject);

        foreach (var child in data.children)
            Spawn(child, element.GetSpawnRoot(child.spawnRootIdentifier), element);
    }
}