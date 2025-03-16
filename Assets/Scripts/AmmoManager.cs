using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    [Header("UI References")]
    public List<Image> ammoIcons;
    public Button reloadButton;

    [Header("Ammo Settings")]
    public int initialAmmo = 4;
    private int currentAmmo;

    void Start()
    {
        currentAmmo = initialAmmo;

        for (int i = 0; i < ammoIcons.Count; i++)
        {
            ammoIcons[i].gameObject.SetActive(i < currentAmmo);
        }

        UpdateReloadButton();

        reloadButton.onClick.AddListener(ReloadAmmo);
    }

    public bool UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            Debug.Log("Ammo used. Remaining: " + currentAmmo);

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
            Debug.Log("Jumlah ammo: " + currentAmmo + ", jumlah ikon: " + ammoIcons.Count);

            for (int i = 0; i < ammoIcons.Count; i++)
            {
                if (ammoIcons[i] == null)
                {
                    Debug.LogError("Ikon indeks " + i + " tidak terdaftar!");
                    continue;
                }
                ammoIcons[i].gameObject.SetActive(i < currentAmmo);
            }
        }
    }


    void UpdateReloadButton()
    {
        reloadButton.interactable = (currentAmmo < initialAmmo);
    }
}