using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Navbar : MonoBehaviour
{
    [SerializeField] private GameObject navbarButton;
    private GameObject addExtraPagesButton;
    
    [SerializeField] private GameObject pageUIElement;
    
    private UIMainPage holder;
    private UIMainPage[] currentPages;
    [HideInInspector] public List<PageType> pageTypes;
    [HideInInspector] public PageType currentlyOpenedPage;

    public void BootToHome()
    {
        addExtraPagesButton = GetComponentInChildren<NavbarAddPagesButton>().gameObject;
        
        gameObject.SetActive(true);
        GoToPage(PageType.home);
    }

    public void RefreshLists(bool refreshButtons = true)
    {
        holder = GetComponentInParent<JSONToUIManager>()?.GetComponentInChildren<UIMainPage>(true);

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
        foreach (Transform child in transform) 
            if (child.TryGetComponent(out NavbarButton _))
                Destroy(child.gameObject);
        
        foreach (var page in pageTypes) 
            SpawnNavbarButton(page);
        
        addExtraPagesButton?.SetActive(
            pageTypes.Count < Enum.GetValues(typeof(PageType)).Length &&
            GetComponentInParent<JSONToUIManager>().canEdit());
        
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

        var newPage = Instantiate(pageUIElement, holder.transform);
        var pageScript = newPage.GetComponent<UIMainPage>();

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
    home,
    news,
    about,
    extra
}