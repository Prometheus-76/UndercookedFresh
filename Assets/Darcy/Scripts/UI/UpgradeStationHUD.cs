using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Author: Darcy Matheson
// Purpose: Controls the pricing of the different upgrades/resupplies at the upgrade station and the HUD when using it

public class UpgradeStationHUD : MonoBehaviour
{
    #region Variables

    #region General
    [Header("General")]

    public Canvas upgradeStationCanvas;
    public GameObject blurEffect;
    public TextMeshProUGUI currentFibreText;
    public static bool showHUD;

    private PlayerStats playerStats;

    #endregion

    #region Health
    [Header("Health")]

    public TextMeshProUGUI healthUpgradeCostText;
    public TextMeshProUGUI healthUpgradeStatText;
    public TextMeshProUGUI healthRefillCostText;
    public TextMeshProUGUI healthRefillStatText;

    public static int currentHealth;
    public static int maxHealth;
    private int newMaxHealth;
    private int healthUpgradeCost;
    private int healthRefillCost;
    private int healthRefillAmount;

    #endregion

    #region Pepper Shotgun
    [Header("Pepper Shotgun")]

    public TextMeshProUGUI pepperShotgunUpgradeCostText;
    public TextMeshProUGUI pepperShotgunUpgradeStatText;
    public TextMeshProUGUI pepperShotgunRefillCostText;
    public TextMeshProUGUI pepperShotgunRefillStatText;

    public GunController pepperShotgunController;

    private int pepperShotgunCurrentDamage;
    private int pepperShotgunNextDamage;
    private int pepperShotgunLevel;
    private int pepperShotgunCurrentAmmo;
    private int pepperShotgunMaxAmmo;
    private int pepperShotgunUpgradeCost;
    private int pepperShotgunRefillCost;
    private int pepperShotgunRefillAmmo;

    #endregion

    #region A-Salt Rifle
    [Header("A-Salt Rifle")]

    public TextMeshProUGUI aSaltRifleUpgradeCostText;
    public TextMeshProUGUI aSaltRifleUpgradeStatText;
    public TextMeshProUGUI aSaltRifleRefillCostText;
    public TextMeshProUGUI aSaltRifleRefillStatText;

    public GunController aSaltRifleController;

    private int aSaltRifleCurrentDamage;
    private int aSaltRifleNextDamage;
    private int aSaltRifleLevel;
    private int aSaltRifleCurrentAmmo;
    private int aSaltRifleMaxAmmo;
    private int aSaltRifleUpgradeCost;
    private int aSaltRifleRefillCost;
    private int aSaltRifleRefillAmmo;

    #endregion

    #region Microwave Gun
    [Header("Microwave Gun")]

    public TextMeshProUGUI microwaveGunUpgradeCostText;
    public TextMeshProUGUI microwaveGunUpgradeStatText;

    public GunController microwaveGunController;

    private int microwaveGunCurrentDamage;
    private int microwaveGunNextDamage;
    private int microwaveGunLevel;
    private int microwaveGunUpgradeCost;

    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        showHUD = false;
        currentHealth = 100;
        maxHealth = 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Turn on/off effect
        upgradeStationCanvas.enabled = showHUD;
        blurEffect.SetActive(showHUD);

        if (showHUD)
        {
            RefreshUI();

            // Display the player's current fibre
            currentFibreText.text = playerStats.currentFibre + " <size=60%>fibre<size=100%><voffset=5> |";

            #region Health

            healthUpgradeStatText.text = "<b>Max HP (lvl " + playerStats.playerLevel + "): </b><size=95%>" + maxHealth + "  <sprite=\"Arrow\" index=0>  " + newMaxHealth;
            
            if (maxHealth - currentHealth > 0)
            {
                healthRefillStatText.text = "<b>Current HP: </b><size=95%>" + currentHealth + " / " + maxHealth + " (+" + (healthRefillAmount) + ")";
            }
            else
            {
                healthRefillStatText.text = "<b>Current HP: </b><size=95%>" + currentHealth + " / " + maxHealth + " (Full!)";
            }

            healthUpgradeCostText.text = "Fibre: " + healthUpgradeCost;
            healthRefillCostText.text = "Fibre: " + healthRefillCost;

            #endregion

            #region Pepper Shotgun

            pepperShotgunUpgradeStatText.text = "<b> DMG / Shot(lvl " + pepperShotgunLevel + "): </b><size=95%>" + pepperShotgunCurrentDamage + "  <sprite=\"Arrow\" index=0>  " + pepperShotgunNextDamage;
             
            if (pepperShotgunMaxAmmo - pepperShotgunCurrentAmmo > 0)
            {
                pepperShotgunRefillStatText.text = "<b>Ammo: </b><size=95%>" + pepperShotgunCurrentAmmo + " / " + pepperShotgunMaxAmmo + " (+" + (pepperShotgunRefillAmmo) + ")";
            }
            else
            {
                pepperShotgunRefillStatText.text = "<b>Ammo: </b><size=95%>" + pepperShotgunCurrentAmmo + " / " + pepperShotgunMaxAmmo + " (Full!)";
            }

            pepperShotgunUpgradeCostText.text = "Fibre: " + pepperShotgunUpgradeCost;
            pepperShotgunRefillCostText.text = "Fibre: " + pepperShotgunRefillCost;

            #endregion

            #region A-Salt Rifle

            aSaltRifleUpgradeStatText.text = "<b> DMG / Shot(lvl " + aSaltRifleLevel + "): </b><size=95%>" + aSaltRifleCurrentDamage + "  <sprite=\"Arrow\" index=0>  " + aSaltRifleNextDamage;

            if (aSaltRifleMaxAmmo - aSaltRifleCurrentAmmo > 0)
            {
                aSaltRifleRefillStatText.text = "<b>Ammo: </b><size=95%>" + aSaltRifleCurrentAmmo + " / " + aSaltRifleMaxAmmo + " (+" + (aSaltRifleRefillAmmo) + ")";
            }
            else
            {
                aSaltRifleRefillStatText.text = "<b>Ammo: </b><size=95%>" + aSaltRifleCurrentAmmo + " / " + aSaltRifleMaxAmmo + " (Full!)";
            }

            aSaltRifleUpgradeCostText.text = "Fibre: " + aSaltRifleUpgradeCost;
            aSaltRifleRefillCostText.text = "Fibre: " + aSaltRifleRefillCost;

            #endregion

            #region Microwave Gun

            microwaveGunUpgradeStatText.text = "<b> DMG / Shot(lvl " + microwaveGunLevel + "): </b><size=95%>" + microwaveGunCurrentDamage + "  <sprite=\"Arrow\" index=0>  " + microwaveGunNextDamage;
            microwaveGunUpgradeCostText.text = "Fibre: " + microwaveGunUpgradeCost;

            #endregion
        }
    }

    // Calculate all required values for fibre cost, upgrades and current stats
    public void RefreshUI()
    {
        #region Health

        currentHealth = playerStats.currentHealth;
        maxHealth = playerStats.maxHealth;
        newMaxHealth = playerStats.CalculateHealthUpgrade();
        healthUpgradeCost = playerStats.CalculateHealthUpgradeCost();

        healthRefillCost = Mathf.CeilToInt((maxHealth - currentHealth) * playerStats.fibrePerHealthPoint);
        healthRefillAmount = Mathf.FloorToInt((maxHealth - currentHealth) * Mathf.Clamp01((float)playerStats.currentFibre / (float)healthRefillCost));
        healthRefillCost = Mathf.Min(healthRefillCost, (int)playerStats.currentFibre);

        #endregion

        #region Pepper Shotgun

        pepperShotgunCurrentDamage = pepperShotgunController.scaledDamagePerBullet;
        pepperShotgunNextDamage = pepperShotgunController.CalculateUpgradeDamage();
        pepperShotgunLevel = pepperShotgunController.gunUpgradeLevel;
        pepperShotgunMaxAmmo = pepperShotgunController.magazineSize + pepperShotgunController.ammoTotalReserves;
        pepperShotgunCurrentAmmo = pepperShotgunController.currentAmmoInMagazine + pepperShotgunController.currentAmmoInReserves;
        pepperShotgunUpgradeCost = pepperShotgunController.CalculateWeaponUpgradeCost();
        pepperShotgunRefillCost = Mathf.CeilToInt((pepperShotgunMaxAmmo - pepperShotgunCurrentAmmo) * pepperShotgunController.costPerBullet);
        pepperShotgunRefillCost += Mathf.CeilToInt(pepperShotgunRefillCost * (playerStats.difficultyLevel / 5f));

        pepperShotgunRefillAmmo = Mathf.FloorToInt((pepperShotgunMaxAmmo - pepperShotgunCurrentAmmo) * Mathf.Clamp01((float)playerStats.currentFibre / (float)pepperShotgunRefillCost));
        pepperShotgunRefillCost = Mathf.Min(pepperShotgunRefillCost, (int)playerStats.currentFibre);

        #endregion

        #region A-Salt Rifle

        aSaltRifleCurrentDamage = aSaltRifleController.scaledDamagePerBullet;
        aSaltRifleNextDamage = aSaltRifleController.CalculateUpgradeDamage();
        aSaltRifleLevel = aSaltRifleController.gunUpgradeLevel;
        aSaltRifleMaxAmmo = aSaltRifleController.magazineSize + aSaltRifleController.ammoTotalReserves;
        aSaltRifleCurrentAmmo = aSaltRifleController.currentAmmoInMagazine + aSaltRifleController.currentAmmoInReserves;
        aSaltRifleUpgradeCost = aSaltRifleController.CalculateWeaponUpgradeCost();
        aSaltRifleRefillCost = Mathf.CeilToInt(Mathf.CeilToInt((aSaltRifleMaxAmmo - aSaltRifleCurrentAmmo) * aSaltRifleController.costPerBullet));
        aSaltRifleRefillCost += Mathf.CeilToInt(aSaltRifleRefillCost * (playerStats.difficultyLevel / 5f));

        aSaltRifleRefillAmmo = Mathf.FloorToInt((aSaltRifleMaxAmmo - aSaltRifleCurrentAmmo) * Mathf.Clamp01((float)playerStats.currentFibre / (float)aSaltRifleRefillCost));
        aSaltRifleRefillCost = Mathf.Min(aSaltRifleRefillCost, (int)playerStats.currentFibre);

        #endregion

        #region Microwave Gun

        microwaveGunCurrentDamage = microwaveGunController.scaledDamagePerBullet;
        microwaveGunNextDamage = microwaveGunController.CalculateUpgradeDamage();
        microwaveGunLevel = microwaveGunController.gunUpgradeLevel;
        microwaveGunUpgradeCost = microwaveGunController.CalculateWeaponUpgradeCost();

        #endregion
    }

    // Return weapon ammo to full and subtract equivalent fibre 
    public void RefillAmmo(int weaponIndex)
    {
        // Determine weapon script and the cost of refilling this weapon
        GunController weaponScript = null;
        int ammoCost = 0;
        int ammoAmount = 0;
        switch (weaponIndex)
        {
            case 1:
                weaponScript = aSaltRifleController;
                ammoCost = aSaltRifleRefillCost;
                ammoAmount = aSaltRifleRefillAmmo;
                break;
            case 2:
                weaponScript = pepperShotgunController;
                ammoCost = pepperShotgunRefillCost;
                ammoAmount = pepperShotgunRefillAmmo;
                break;
            default:
                return;
        }

        // Purchase ammo and refill weapon
        if ((ulong)ammoCost <= playerStats.currentFibre && ammoCost > 0)
        {
            playerStats.currentFibre -= (ulong)ammoCost;

            // Transfer from reserves to magazine
            int ammoMissingFromMagazine = weaponScript.magazineSize - weaponScript.currentAmmoInMagazine;
            int additionToMagazine = Mathf.Min(ammoMissingFromMagazine, weaponScript.currentAmmoInReserves);
            weaponScript.currentAmmoInMagazine += additionToMagazine;
            weaponScript.currentAmmoInReserves -= additionToMagazine;

            // Fill up remainder of magazine from purchase
            ammoMissingFromMagazine = weaponScript.magazineSize - weaponScript.currentAmmoInMagazine;
            additionToMagazine = Mathf.Min(ammoMissingFromMagazine, ammoAmount);
            weaponScript.currentAmmoInMagazine += additionToMagazine;
            ammoAmount -= additionToMagazine;

            // Fill up remainder of reserves from purchase
            weaponScript.currentAmmoInReserves += ammoAmount;
        }
    }

    // Return health to full and subtract equivalent fibre 
    public void RefillHealth()
    {
        // Purchase ammo and refill weapon
        if ((ulong)healthRefillCost <= playerStats.currentFibre && healthRefillCost > 0)
        {
            playerStats.currentFibre -= (ulong)healthRefillCost;
            playerStats.currentHealth += healthRefillAmount;
            playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, playerStats.maxHealth);
        }
    }

    public void CloseUpgradeStation()
    {
        // Resume normal gameplay
        showHUD = false;
        UserInterfaceHUD.showHUD = true;
        PlayerStats.gamePaused = false;

        // Make the cursor invisible and stop it from leaving the window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
