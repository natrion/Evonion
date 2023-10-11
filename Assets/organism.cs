using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class organism : MonoBehaviour
{
    [System.Serializable]
    public struct BodyPart
    {
        public string name;
        public float foodGenerated;
        public float foodlost;
        public float BodyPartHealt;
        public float BodyPartDamage;
        public float price;
        //public List<Vector3> BodyPartEdges;
        public GameObject BodyPartReal;
    }
    [SerializeField] private BodyPart[] AllBodyParts;
    [SerializeField] private float mutationChance;
    [SerializeField] private WorldGeneration worldGeneration;
    [SerializeField] private float FloraCompetitionDitance;
    [SerializeField] private float Growdistance;
    [SerializeField] private float foodMultyPlayer;
    [SerializeField] private float bodySize;
    [Space(20)]
    public float organismCost = 1;
    public float food = 0;
    public float foodgeneration = 0.2f;
    public float foodlost = 0.1f;
    [SerializeField] private float lenght;

    private GameObject ThisGameObject;
    //[SerializeField] private int numOrgansOnOrganism;
    //public List<Vector3> edges;
    public List<GameObject> lastBodyPart;
    public List<BodyPart> lastBodyPartIformation;
    public float foodActualyGenerated;
    [SerializeField] public float MaxHealth;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    private void Awake()
    {
        StartCoroutine(Tick());
        ThisGameObject = gameObject;
        foodActualyGenerated = foodgeneration;
        health = MaxHealth;
        gameObject.GetComponent<SphereCollider>().radius = Growdistance ; // food * FloraCompetitionDitance;
    }
    IEnumerator Tick()
    {
        while (true)
        {
            CreateFood();
            Duplicate();
            Die();
            yield return new WaitForSeconds(1);

            foodActualyGenerated = foodgeneration;

            yield return new WaitForSeconds(0.5f);
        }
    }
    void CreateFood()
    {
        food += foodActualyGenerated / (1+transform.position.y / 20f) * foodMultyPlayer - foodlost;      
    }
    void Duplicate()
    {
        if (organismCost*1.5f > food) return;
        food = 0;
        GameObject child = Instantiate(ThisGameObject);
        child.transform.position = worldGeneration.CalculateHight(transform.position.x+ Growdistance /* foodgeneration * FloraCompetitionDitance*1.5f*/ * UnityEngine.Random.Range(-1f, 1f) * 3f, lenght, transform.position.z+ Growdistance /* * foodgeneration  * FloraCompetitionDitance*/ * UnityEngine.Random.Range(-1f, 1f) * 3f);
        child.transform.parent = transform.parent;
        organism Organism = child.GetComponent<organism>();
        Organism.food = 10000000000;//food/3;
        Organism.organismCost = organismCost;
        ////Mutations
        float mutationNumber = UnityEngine.Random.Range(0, mutationChance * 2f + 1f);
        if (mutationNumber < 1f)
        {
            BodyPart bodyPart = AllBodyParts[UnityEngine.Random.Range(0, AllBodyParts.Length )];
            Organism.foodgeneration += bodyPart.foodGenerated;
            Organism.foodlost += bodyPart.foodlost;
            Organism.organismCost += bodyPart.price;
            Organism.MaxHealth += bodyPart.BodyPartHealt;
            Organism.damage += bodyPart.BodyPartDamage;
            GameObject bodyPartObject = Instantiate(bodyPart.BodyPartReal);
            Organism.lastBodyPartIformation.Add(bodyPart);

            Organism.lastBodyPart.Add(bodyPartObject);
            bodyPartObject.transform.parent = child.transform;
            bodyPartObject.transform.position = child.transform.GetChild(UnityEngine.Random.Range(0, child.transform.childCount - 1)).position +new Vector3(UnityEngine.Random.Range(-0.7f,0.7f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.4f, 0.4f)*bodySize);
            //Organism.edges.AddRange(bodyPart.BodyPartEdges);
        }
        if(mutationNumber > mutationChance * 2 - 1 & child.transform.childCount>1 & Organism.lastBodyPart.Count>1)
        {
            int num = UnityEngine.Random.Range(0, Organism.lastBodyPart.Count);
            Organism.foodgeneration -= Organism.lastBodyPartIformation[num].foodGenerated;
            Organism.foodlost -= Organism.lastBodyPartIformation[num].foodlost;
            Organism.MaxHealth += Organism.lastBodyPartIformation[num].BodyPartHealt;
            Organism.organismCost -= Organism.lastBodyPartIformation[num].price;
            Organism.damage -= Organism.lastBodyPartIformation[num].BodyPartDamage;
            Organism.lastBodyPartIformation.RemoveAt(num);
            Destroy(Organism.lastBodyPart[num]);
            Organism.lastBodyPart.RemoveAt(num);
        }
    }
    void Die()
    {
        if (0 > food | Vector2.Distance(new Vector2(transform.position.x, transform.position.z), Vector2.zero) > worldGeneration.widthSegments * 0.8f * worldGeneration.detail | health < 0)
        { 
            Destroy(gameObject); 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //fotosintesis competision
        if (other.isTrigger == false) return;
        if (other.gameObject.GetComponent<organism>().foodgeneration >= foodgeneration & other.gameObject.transform.parent != transform) foodActualyGenerated = 0;
        //fight
        other.gameObject.GetComponent<organism>().health -= damage;
        food += damage * 2;
    }
}
