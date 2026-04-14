using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Navbar : MonoBehaviour
{
    [SerializeField] private GameObject navbarButton;
    private GameObject addExtraPagesButton;
    
    [SerializeField] private GameObject pageUIElement;
    
    private UIGenericElement holder;
    private UIMainPage[] currentPages;
    [HideInInspector] public List<PageType> pageTypes;
    [HideInInspector] public PageType currentlyOpenedPage;

    private JSONToUIManager manager;

    public void BootToHome()
    {
        addExtraPagesButton = GetComponentInChildren<NavbarAddPagesButton>(true).gameObject;
        manager ??= GetComponentInParent<JSONToUIManager>();
        
        gameObject.SetActive(true);
        GoToPage(PageType.home);
    }

    public void RefreshLists(bool refreshButtons = true)
    {
        manager ??= GetComponentInParent<JSONToUIManager>();
        holder = manager?.GetComponentInChildren<UIGenericElement>(true);
        
        if (holder == null)
        {
            RefreshNavbarButtons();
            gameObject.SetActive(false);
            return;
        }

        currentPages = holder.GetSpawnRoot().GetComponentsInChildren<UIMainPage>(true);
            
        pageTypes = currentPages.Select(p => p.pageType)
            .OrderBy(p => (int)p)
            .ToList();
        
        if (refreshButtons) RefreshNavbarButtons();
    }
    
    private void RefreshNavbarButtons()
    {
        manager ??= GetComponentInParent<JSONToUIManager>();
        bool canEdit = manager.canEdit();
        
        foreach (Transform child in transform) 
            if (child.TryGetComponent(out NavbarButton _))
                Destroy(child.gameObject);
        
        foreach (var page in pageTypes) 
            if (canEdit || !GetPageFromList(page).isHidden)
                SpawnNavbarButton(page);
        
        addExtraPagesButton?.SetActive(
            pageTypes.Count < Enum.GetValues(typeof(PageType)).Length - 1 && canEdit);  //Ignore empty page type
        
        addExtraPagesButton?.transform.SetAsLastSibling();
    }

    private void SpawnNavbarButton(PageType page)
    {
        var newButton = Instantiate(navbarButton, transform);
        var buttonScript = newButton.GetComponent<NavbarButton>();
        
        buttonScript.page = page;
        buttonScript.SetUpText(GetPageFromList(page).isHidden);
    }

    public void GoToPage(PageType page)
    {
        RefreshLists(false);
        
        UIMainPage targetPage = GetPageFromList(page);
        
        if (targetPage == null)
        {
            Debug.LogWarning("Page no longer exists");
            return;
        }

        foreach (Transform child in holder.GetSpawnRoot())
            if (child.TryGetComponent(out UIMainPage _))
                child.gameObject.SetActive(false);
        
        targetPage.gameObject.SetActive(true);
        currentlyOpenedPage = page;
        
        RefreshNavbarButtons();
    }

    public void AddPage(PageType page)
    {
        GameObject newPagePrefab = manager.prefabEntries
            .FirstOrDefault(p => p.GetComponent<UIMainPage>()?.pageType == page);

        if (newPagePrefab == null)
        {
            Debug.LogWarning("page prefab not found, aborting");
            return;
        }
        
        GameObject newPage = Instantiate(newPagePrefab, holder.GetSpawnRoot());
        UIMainPage pageScript = newPage.GetComponent<UIMainPage>();

        pageScript.RefreshPageDimensions();
        pageScript.pageType = page;
        GoToPage(page);
    }
    
    public void RemovePage(PageType page)
    {
        UIMainPage targetPage = GetPageFromList(page);

        if (targetPage == null)
            return;
        
        DestroyImmediate(targetPage.gameObject);
        
        if (currentlyOpenedPage == page) 
            GoToPage(PageType.home);
        else 
            RefreshLists();
    }

    public void HidePage(PageType page)
    {
        UIMainPage targetPage = GetPageFromList(page);

        if (targetPage == null) 
            return;

        targetPage.isHidden = !targetPage.isHidden;
        
        RefreshLists();
    }
    
    private UIMainPage GetPageFromList(PageType page)
    {
        return currentPages.FirstOrDefault(p => p.pageType == page);
    }

    public string GetPageName(PageType page)
    {
        switch (page)
        {
            case PageType.home:
                return "Home";
            case PageType.news:
                return "News";
            case PageType.about:
                return "About";
            case PageType.extra:
                return "Extra's";
        }

        return "null";
    }
}

public enum PageType
{
    empty, 
    home,
    news,
    about,
    extra
}