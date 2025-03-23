using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    [Header("UI References")]
    public List<Image> ammoIcons;
    public Button reloadButton;

    [Header("Ammo Settings")]
    int initialAmmo;
    int currentAmmo;

    void Start()
    {
        if (ammoIcons == null || ammoIcons.Count == 0)
        {
            Debug.LogError("AmmoIcons list is empty or not assigned!");
            return;
        }
        if (reloadButton == null)
        {
            Debug.LogError("ReloadButton is not assigned!");
            return;
        }

        initialAmmo = ammoIcons.Count;
        currentAmmo = initialAmmo;

        UpdateAmmoUI();
        UpdateReloadButton();
    }



    public bool UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;

            UpdateAmmoUI();
            UpdateReloadButton();
            return true;
        }
        else
        {
            Debug.Log("Out of ammo!");
            return false;
        }
    }

    public void ReloadAmmo()
    {
        if (currentAmmo < initialAmmo)
        {
            currentAmmo = initialAmmo;
            UpdateAmmoUI();
            UpdateReloadButton();
        }
    }

    void UpdateAmmoUI()
    {
        for (int i = 0; i < ammoIcons.Count; i++)
        {
            ammoIcons[i].gameObject.SetActive(i < currentAmmo);
        }
    }


    void UpdateReloadButton()
    {
        reloadButton.interactable = (currentAmmo < initialAmmo);
    }

    public int GetRemainingAmmo()
    {
        return currentAmmo;
    }
}