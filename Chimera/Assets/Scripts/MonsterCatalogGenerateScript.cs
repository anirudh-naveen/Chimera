using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor;
using System.Collections.Generic;

public class MonsterCatalogGenerateScript : MonoBehaviour
{
    public GameObject prefab;
    public GameObject contentRect;
    public GameObject[] Monsters;

    private int index = 0;
    private GameObject currentEntry;
    private Dictionary<string, Dictionary<string, string>> catalogInfo = new Dictionary<string, Dictionary<string, string>>();

    void Start()
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            GameObject monster = Instantiate(Monsters[i]);
            Head head_script = monster.GetComponentInChildren<Head>();
            Body body_script = monster.GetComponentInChildren<Body>();
            Tail tail_script = monster.GetComponentInChildren<Tail>();
            catalogInfo.Add(Monsters[i].name, new Dictionary<string, string>()
            {
                {"attack", tail_script.getAttack().ToString()},
                {"health", body_script.getHealth().ToString()},
                {"ability", head_script.ability_description},
                {"description", head_script.scientist_description}
            });
            Destroy(monster);
        }
        /*
        foreach (KeyValuePair<string, Dictionary<string, string>> entry in catalogInfo)
        {
            Debug.Log(entry.Key + "- {attack: " + entry.Value["attack"] + ", health: " + entry.Value["health"] + ", ability: " + entry.Value["ability"] + ", description: " + entry.Value["description"] + "}");
        }
        */
        UpdateMonsterCatalog();
    }

    void OnDestroy()
    {
        Debug.Log("Exiting monster catalog.");
    }

    public void MoveForward()
    {
        SFXPlayer[] sfxplayer = UnityEngine.Object.FindObjectsByType<SFXPlayer>(FindObjectsSortMode.InstanceID);
        if (sfxplayer.Length > 0 && sfxplayer.Length >= 1)
        {
            sfxplayer[sfxplayer.Length - 1].Page();
        }
        index = (index + 1) % Monsters.Length;
        UpdateMonsterCatalog();
    }

    public void MoveBackward()
    {
        SFXPlayer[] sfxplayer = UnityEngine.Object.FindObjectsByType<SFXPlayer>(FindObjectsSortMode.InstanceID);
        if (sfxplayer.Length > 0 && sfxplayer.Length >= 1)
        {
            sfxplayer[sfxplayer.Length - 1].Page();
        }
        index--;
        if (index < 0)
        {
            index = Monsters.Length - 1;
        }
        UpdateMonsterCatalog();
    }

    private void UpdateMonsterCatalog()
    {
        if (currentEntry != null)
        {
            Destroy(currentEntry);
        }
        currentEntry = Instantiate(prefab, new Vector3(-357, 192, 0), Quaternion.Euler(0, 0, 0)) as GameObject;

        GameObject monster = Monsters[index];
        GameObject monster_head = monster.transform.GetChild(2).gameObject;
        GameObject monster_body = monster.transform.GetChild(3).gameObject;
        GameObject monster_tail = monster.transform.GetChild(4).gameObject;


        currentEntry.transform.SetParent(contentRect.transform, false);
        GameObject head = currentEntry.transform.GetChild(0).gameObject;
        GameObject body = currentEntry.transform.GetChild(1).gameObject;
        GameObject tail = currentEntry.transform.GetChild(2).gameObject;
        GameObject name = currentEntry.transform.GetChild(3).gameObject;
        GameObject stats = currentEntry.transform.GetChild(4).gameObject;
        GameObject ability = currentEntry.transform.GetChild(5).gameObject;
        GameObject description = currentEntry.transform.GetChild(6).gameObject;

        Image im1 = head.GetComponent<Image>();
        im1.sprite = monster_head.GetComponentInChildren<Head>().splash;
        Image im2 = body.GetComponent<Image>();
        im2.sprite = monster_body.GetComponentInChildren<Body>().splash;
        Image im3 = tail.GetComponent<Image>();
        im3.sprite = monster_tail.GetComponentInChildren<Tail>().splash;

        TMP_Text tmp = name.GetComponent<TMP_Text>();
        tmp.text = monster.name.Substring(0, monster.name.IndexOf("Monster"));

        tmp = stats.GetComponent<TMP_Text>();
        tmp.text = "Base Health: " + catalogInfo[monster.name]["health"] + "\nBase Attack: " + catalogInfo[monster.name]["attack"];

        tmp = ability.GetComponent<TMP_Text>();
        tmp.text = catalogInfo[monster.name]["ability"];

        tmp = description.GetComponent<TMP_Text>();
        tmp.text = catalogInfo[monster.name]["description"];
    }
}
