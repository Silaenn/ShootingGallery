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
    private int currentAmmo;

    void Start()
    {
        initialAmmo = ammoIcons.Count;
        Debug.Log(initialAmmo);
        currentAmmo = initialAmmo;

        for (int i = 0; i < ammoIcons.Count; i++)
        {
            ammoIcons[i].gameObject.SetActive(i < currentAmmo);
        }
    }

    public bool UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;

            if (currentAmmo < ammoIcons.Count)
            {
                ammoIcons[currentAmmo].gameObject.SetActive(false);
            }

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

            for (int i = 0; i < ammoIcons.Count; i++)
            {
                ammoIcons[i].gameObject.SetActive(i < currentAmmo);
            }

            UpdateReloadButton();
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