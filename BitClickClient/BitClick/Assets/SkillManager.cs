using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject StaminaSkill;
    public GameObject DamageSkill;
    public GameObject ClickMultiplierSkill;
    public GameObject StaminaRechargeSkill;
    public GameObject CriticalHitDamageSkill;

    public GameObject PointsRemaning;

    void ChangeSkill(Constants.Skills skill, bool inc)
    {
        //checks if its neccessary to even send a packet, ie if you are at cap already
        switch (skill)
        {
            //call packet, queu skill for change and wait for response if it went trough

            case Constants.Skills.STAMINA_PERCENT:
                if (inc) {
                    if (gameManager.player.skills.Stamina_Percent + 1 <= Constants.SkillCaps[skill])
                    {

                    }
                    else
                        return;
                }
                if(!inc)
                {
                    if ((int)gameManager.player.skills.Stamina_Percent - 1 >= 0)
                    {

                    }
                    else
                        return;
                }
                    
                break;
            case Constants.Skills.DAMAGE_PERCENT:
                if (inc)
                {
                    if (gameManager.player.skills.Damage_Percent + 1 <= Constants.SkillCaps[skill])
                    {

                    }
                    else
                        return;
                }
                if (!inc)
                {
                    if ((int)gameManager.player.skills.Damage_Percent - 1 >= 0)
                    {

                    }
                    else
                        return;
                }
                break;
            case Constants.Skills.CLICK_MULTIPLIER:
                if (inc)
                {
                    if (gameManager.player.skills.Click_Multiplier + 1 <= Constants.SkillCaps[skill])
                    {

                    }
                    else
                        return;
                }
                if (!inc)
                {
                    if ((int)gameManager.player.skills.Click_Multiplier - 1 >= 0)
                    {

                    }
                    else
                        return;
                }
                break;
            case Constants.Skills.STAMINA_RECHARGE_PERCENT:
                if (inc)
                {
                    if (gameManager.player.skills.Stamina_Recharge_Percent + 1 <= Constants.SkillCaps[skill])
                    {

                    }
                    else
                        return;
                }
                if (!inc)
                {
                    if ((int)gameManager.player.skills.Stamina_Recharge_Percent - 1 >= 0)
                    {

                    }
                    else
                        return;
                }
                break;
            case Constants.Skills.CRITICAL_HIT_DAMAGE_PERCENT:
                if (inc)
                {
                    if (gameManager.player.skills.Critical_Hit_Damage_Percent + 1 <= Constants.SkillCaps[type])
                    {

                    }
                    else
                        return;
                }
                if (!inc)
                {
                    if ((int)gameManager.player.skills.Critical_Hit_Damage_Percent - 1 >= 0)
                    {

                    }
                    else
                        return;
                }
                break;
            default:
                return;
        }
    }

    //call this on SKILL_POINT_RES
    public void UpdateSkills(Constants.Skills type, bool inc)
    {
        switch(type)
        {
            case Constants.Skills.STAMINA_PERCENT:
                if(inc)
                    gameManager.player.skills.Stamina_Percent++;
                else
                    gameManager.player.skills.Stamina_Percent--;

                StaminaSkill.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = ((float)gameManager.player.skills.Stamina_Percent / (float)Constants.SkillCaps[type]);
                StaminaSkill.transform.GetChild(2).GetComponent<Text>().text = gameManager.player.skills.Stamina_Percent + " / " + Constants.SkillCaps[type];
                break;
            case Constants.Skills.DAMAGE_PERCENT:
                if (inc)
                    gameManager.player.skills.Damage_Percent++;
                else
                    gameManager.player.skills.Damage_Percent--;

                DamageSkill.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = ((float)gameManager.player.skills.Damage_Percent / (float)Constants.SkillCaps[type]);
                DamageSkill.transform.GetChild(2).GetComponent<Text>().text = gameManager.player.skills.Damage_Percent + " / " + Constants.SkillCaps[type];
                break;
            case Constants.Skills.CLICK_MULTIPLIER:
                if (inc)
                    gameManager.player.skills.Click_Multiplier++;
                else
                    gameManager.player.skills.Click_Multiplier--;

                ClickMultiplierSkill.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = ((float)gameManager.player.skills.Click_Multiplier / (float)Constants.SkillCaps[type]);
                ClickMultiplierSkill.transform.GetChild(2).GetComponent<Text>().text = gameManager.player.skills.Click_Multiplier + " / " + Constants.SkillCaps[type];
                break;
            case Constants.Skills.STAMINA_RECHARGE_PERCENT:
                if (inc)
                    gameManager.player.skills.Stamina_Recharge_Percent++;
                else
                    gameManager.player.skills.Stamina_Recharge_Percent--;

                StaminaRechargeSkill.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = ((float)gameManager.player.skills.Stamina_Recharge_Percent / (float)Constants.SkillCaps[type]);
                StaminaRechargeSkill.transform.GetChild(2).GetComponent<Text>().text = gameManager.player.skills.Stamina_Recharge_Percent + " / " + Constants.SkillCaps[type];
                break;
            case Constants.Skills.CRITICAL_HIT_DAMAGE_PERCENT:
                if (inc)
                    gameManager.player.skills.Critical_Hit_Damage_Percent++;
                else
                    gameManager.player.skills.Critical_Hit_Damage_Percent--;

                CriticalHitDamageSkill.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = ((float)gameManager.player.skills.Critical_Hit_Damage_Percent / (float)Constants.SkillCaps[type]);
                CriticalHitDamageSkill.transform.GetChild(2).GetComponent<Text>().text = gameManager.player.skills.Critical_Hit_Damage_Percent + " / " + Constants.SkillCaps[type];
                break;
            default:
                return;
        }
    }

    //upon loging in pass settings here so we can update the list
    public void LoadSettings()
    {

    }

    #region button_calls
    public void StaminaButtonCall(bool inc)
    {
        ChangeSkill(Constants.Skills.STAMINA_PERCENT, inc);
    }

    public void DamageButtonCall(bool inc)
    {
        ChangeSkill(Constants.Skills.DAMAGE_PERCENT, inc);
    }

    public void ClickMultiplierButtonCall(bool inc)
    {
        ChangeSkill(Constants.Skills.CLICK_MULTIPLIER, inc);
    }

    public void StaminaRechargeButtonCall(bool inc)
    {
        ChangeSkill(Constants.Skills.STAMINA_RECHARGE_PERCENT, inc);
    }

    public void CriticalHitDamageButtonCall(bool inc)
    {
        ChangeSkill(Constants.Skills.CRITICAL_HIT_DAMAGE_PERCENT, inc);
    }
    #endregion
    }
