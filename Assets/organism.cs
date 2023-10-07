using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class organism : MonoBehaviour
{
    [SerializeField] private WorldGeneration worldGeneration;
    public float organismCost = 1;
    public float food = 0;
    public float foodgeneration = 0.2f;
    public float foodlost = 0.1f;
    [SerializeField] private float lenght;
    [SerializeField] private float Growdistance;
    public GameObject ThisGameObject;

    private void Awake()
    {
        StartCoroutine(Tick());
        ThisGameObject = gameObject;
        
    }
    IEnumerator Tick()
    {
        while (true)
        {
            CreateFood();
            Duplicate();
            Die();
            yield return new WaitForSeconds(1);
        }
    }
    void CreateFood()
    {
        food += foodgeneration / (1+transform.position.y / 20f) - foodlost;      
    }
    void Duplicate()
    {
        if (organismCost*1.5f > food) return;
        food = food * 0.333333333f;
        GameObject child = Instantiate(ThisGameObject);
        child.transform.position = worldGeneration.CalculateHight(transform.position.x+ Growdistance * Random.Range(-1f, 1f), lenght, transform.position.z+ Growdistance * Random.Range(-1f, 1f));
        child.transform.parent = transform.parent;
        organism Organism = child.GetComponent<organism>();
        Organism.food = 0;
        Organism.organismCost = organismCost;
    }
    void Die()
    {
        if (0 > food | Vector2.Distance( new Vector2(transform.position.x,transform.position.z), Vector2.zero)> worldGeneration.widthSegments*0.8f*worldGeneration.detail) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false) return;
        if (other.gameObject.GetComponent<organism>().foodgeneration >= foodgeneration & other.gameObject.transform.parent != transform) foodgeneration = 0;
    }
}
