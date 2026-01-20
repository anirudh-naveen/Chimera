using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

//DO NOT EDIT!!!!! READ ONLY
[DefaultExecutionOrder(-150)]
public class Globals : MonoBehaviour
{
    public static List<string> Chimeras = new List<string>();
    public static List<GameObjectChimera> party = new List<GameObjectChimera>();
    public static Dictionary<NewChimeraStats, GameObjectChimera> active_party_objs = new Dictionary<NewChimeraStats, GameObjectChimera>();
    public static List<NewChimeraStats> party_game_objs = new List<NewChimeraStats>();
    public static List<int> party_indexes = new List<int>();
    public const int PARTY_SIZE = 5;
    public static int currency = 1000;
    public static int levelSelected = 0;
    public static int numKills = 0;
    public GameObject Chimerafab;
    [SerializeField] public Vector3 adjustedSpriteSize = new Vector3(1, 0, 0);
    public bool isDungeon = true;
    //public static int numMonsters = 1;
    public static int energy;
    public const int maxEnergy = 100;
    public static string SceneSelection = "";
    //should be held in game manager eventually maybe?
    public static int highestClearedLevel = 0; //misnamed; should be highest level accessible
    public static Vector2 default_kb = new Vector2(0.5f, 0.5f);
    public static int currentlyDeadChimeras = 0;
    private static NewChimeraStats[] chimerasInParty = new NewChimeraStats[5];
    public Button[] buttons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isDungeon)
        {
            energy = 0;
            numKills = 0;
            if (levelSelected == 0)
            {
                energy += 50;
            }
            //initialize all chimeras in party
            Vector3 add = new Vector3(0.5f, 0.5f, 0);
            Vector3 adjust = new Vector3(0, 0, -1);
            party = new List<GameObjectChimera>();
            /*
            foreach (int index in party_indexes)
            {
                GameObject temp = (GameObject) Resources.Load(Chimeras[index]);
                party.Add(temp);
            }
            
            for (int i = 0; i < party.Count; i++)
            {
                GameObject newChimera = Instantiate(party[i], add * i, Quaternion.identity);
                Debug.Log("new chimera instantiated");
                /*Type hscript = Type.GetType(hscripts[party[i].HeadInd]);
                Type bscript = Type.GetType(bscripts[party[i].BodyInd]);
                Type tscript = Type.GetType(tscripts[party[i].TailInd]);
                GameObject headChild = newChimera.transform.GetChild(0).gameObject;
                GameObject bodyChild = newChimera.transform.GetChild(1).gameObject;
                GameObject tailChild = newChimera.transform.GetChild(2).gameObject;
                Component headScript = headChild.AddComponent(hscript);
                Component bodyScript = bodyChild.AddComponent(bscript);
                Component tailScript = tailChild.AddComponent(tscript);
                ChimeraScript cs = newChimera.GetComponentInChildren<ChimeraScript>();
                cs.spot = i + 1;
                party_objs.Add(newChimera);
            }*/
            active_party_objs.Clear();
            for (int i = 0; i < party_indexes.Count; i++)
            {
                NewChimeraStats chimera = party_game_objs[party_indexes[i]];
                if (chimera != null)
                {
                    chimerasInParty[i] = chimera;
                }
                GameObject newChimera = Instantiate(chimera.BaseObject, add * i + adjust, Quaternion.identity);
                Debug.Log("new chimera instantiated");
                ChimeraScript cs = newChimera.GetComponentInChildren<ChimeraScript>();
                cs.spot = i + 1;
                cs.level = chimera.level;
                Debug.Log("Current chimera's level: "+cs.level+"  EXP: "+ chimera.exp);
                Vector3 spriteSize = new Vector3(chimera.Head.GetComponentInChildren<SpriteRenderer>().bounds.size.x, 0, 0);
                GameObject newHead = Instantiate(chimera.Head, newChimera.transform.position - adjustedSpriteSize, Quaternion.identity, newChimera.transform);
                GameObject newBody = Instantiate(chimera.Body, newChimera.transform.position, Quaternion.identity, newChimera.transform);
                GameObject newTail = Instantiate(chimera.Tail, newChimera.transform.position + adjustedSpriteSize, Quaternion.identity, newChimera.transform);
                GameObjectChimera active_chimera = new GameObjectChimera(newHead, newBody, newTail, newChimera);
                active_party_objs.Add(chimera, active_chimera);
            }
            for (int i = party_indexes.Count; i < 5; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
    } 

    // Update is called once per frame
    void Update()
    {
        party_game_objs = ChimeraParty.Chimeras;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game! This would've quit in an actual build.");
        Application.Quit(); // only works when built
    }
    /*
    public void Dungeon()
    {
        if (party_indexes.Count > 0)
        {
            SceneManager.LoadScene(SceneSelection);
        } else
        {
            Debug.Log("You must pick at least one Chimera before going into the dungeon!");
        }
    }
    public void PartySelect()
    {
        if (SceneSelection != "")
        {
            SceneManager.LoadScene("Chimera Select");
        }
        else
        {
            Debug.Log("must select a level");
        }
        
    }
    public void SelectDungeon(int d)
    {
        if (d > highestClearedLevel)
        {
            Debug.Log("You have not unlocked this level yet!");
        } else
        {
            if (d > 0)
            {
                SceneSelection = "Dungeon" + d;
            }
            else
            {
                SceneSelection = "Dungeon";
            }
            SceneManager.LoadScene("Chimera Select");
        }
    }*/

    public void Hacks()
    {
        highestClearedLevel = 3;
    }
    public static void ChimeraAbility(int x){
        //BIG ISSUE HERE COME BACK chimeras die during combat, party_indexes becomes out of date/out of range of the gameobjs
        if (x > -1 && x < chimerasInParty.Length)
        {
            NewChimeraStats chimera = chimerasInParty[x];
            Debug.Log(chimera);
            GameObjectChimera temp = active_party_objs[chimera];
            if (temp == null)
            {
                Debug.Log("This chimera is dead");
                return;
            }
            GameObject head_object = temp.Head;
            if (head_object == null)
            {
                return;
            }
            Head h = head_object.GetComponent<Head>();
            if (h != null)
            {
                if (energy >= 10)
                {
                    head_object.GetComponent<Animator>().SetTrigger("Ability");
                    h.UseAbility();
                    energy -= 10;
                }
                else
                {
                    Debug.Log("Not enough energy!");
                }
            }
        }
    }
    public void removeChimera(NewChimeraStats chimera)
    {
        Debug.Log("Removed Chimera");

    }

    public static int AmountOfChimerasLeftToAddInParty()
    {
        return Globals.PARTY_SIZE - Globals.party_indexes.Count;
    }

    public static NewChimeraStats FindChimeraInPartyByIndex(int i)
    {
        if (i >= party_indexes.Count)
        {
            return null;
        }
        return party_game_objs[party_indexes[i]];
    }

    public static GameObjectChimera FindChimeraGameObjectInPartyByIndex(int i)
    {
        if (i >= party_indexes.Count)
        {
            return null;
        }
        return active_party_objs[FindChimeraInPartyByIndex(i)];
    }

}
public class ChimeraStats{
        public int HeadInd;
        public int BodyInd;
        public int TailInd;
        public int level;
        public int exp;
        public ChimeraStats(int h, int b, int t) {
            HeadInd = h;
            BodyInd = b;
            TailInd = t;
            level = 1;
            exp = 0;
        }
    }


