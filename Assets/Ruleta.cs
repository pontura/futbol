using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Ruleta : MonoBehaviour {

    public GameObject tapa;
    public float spacing;
    float InitialSpeed;
    public float acceleration;
    public int selectedID;

    public Transform container;
    public RuletaItem ruletaItem;
    public List<RuletaItem> items;
    public int itemsHeight;
    public Color[] colors;

    public float totalHeight;

    [Serializable]
    public class ItemData
    {
        public Sprite sprite;
        public int id;
    }
    public states state;
    public enum states
    {
        IDLE,
        ROLLING,
        REPOSITION,
        FINISH
    }
    public float speed;
    float repositionTo;
    System.Action<int> OnDone;

    public void Init (List<Sprite> all) {
        print("__________________________________" + all.Count);
        tapa.SetActive(true);
        items.Clear();

        int id = 0;
        foreach (Sprite sprite in all)
        {            
            Add(id, sprite);           
            id++;
        }
        totalHeight = items.Count * (itemsHeight + spacing);
        Add(id, all[0]);
    }
    void Add(int id, Sprite sprite)
    {
        ItemData item = new ItemData();
        item.sprite = sprite;
        item.id = id;
        RuletaItem newItem = Instantiate(ruletaItem);
        newItem.transform.SetParent(container);
        newItem.transform.transform.localScale = Vector2.one;
        newItem.transform.localPosition = Vector3.zero;
        newItem.Init(item, itemsHeight);
        items.Add(newItem);
    }
    public void SetOn(System.Action<int> OnDone)
    {
        print("__________________________________SetOn ");
        tapa.SetActive(false);
        this.OnDone = OnDone;
        InitialSpeed = UnityEngine.Random.Range(5, 20);
        speed = InitialSpeed;
        state = states.ROLLING;
    }
    void Update()
    {
        if (state == states.ROLLING) Rolling();
        else if (state == states.REPOSITION) Repositionate();
    }
   
    void Rolling()
    {

        speed -= Time.deltaTime + acceleration;
        float newY = container.transform.localPosition.y + speed;

        if (speed <= 0)
        {
            CalculateItem();
            state = states.REPOSITION;            
        }
        if (container.localPosition.y > (totalHeight))
            ResetPosition();
        else
            container.localPosition = new Vector3(0, newY, 0);
    }
    private void CalculateItem()
    {
        selectedID = (int)Mathf.Round((container.localPosition.y) / itemsHeight);
        repositionTo = (itemsHeight+spacing) * selectedID;
        if (selectedID >= items.Count-1) selectedID = 0;
    }
    void Repositionate()
    {
        if (container.transform.localPosition.y > repositionTo)
            RepositionateUp();
        else
            RepositionateDown();
    }
    void RepositionateUp()
    {
        speed += Time.deltaTime + acceleration;
        float newY = container.transform.localPosition.y - speed;
        container.transform.localPosition = new Vector3(0, newY, 0);

        if (container.transform.localPosition.y <= repositionTo) Ready();

    }
    void RepositionateDown()
    {
        speed += Time.deltaTime + acceleration;
        float newY = container.transform.localPosition.y + speed;
        container.transform.localPosition = new Vector3(0, newY, 0);

        if (container.transform.localPosition.y >= repositionTo) Ready();

    }
    void Ready()
    {
       
        state = states.FINISH;
         OnDone(selectedID);
    }
    void ResetPosition()
    {
        container.transform.localPosition = Vector3.zero;     
    }
}
