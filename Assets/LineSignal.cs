using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSignal : MonoBehaviour
{
    public GameObject asset;
    Character character;
    Character other;
    public float offset = 1;

    void Start()
    {
        this.character = GetComponentInParent<Character>();
        this.gameObject.SetActive(false);
    }
    public void SetOn(bool isOn)
    {
        this.gameObject.SetActive(isOn);
        if (isOn)
            CheckIfPasePossible();
        else
            CancelInvoke();
    }
    void CheckIfPasePossible()
    {
        other = Game.Instance.charactersManager.GetCharacterEnPase(character);
        if (other == null)
            asset.SetActive(false);
        else
            asset.SetActive(true);
        Invoke("CheckIfPasePossible", 0.25f);
    }
    void Update()
    {
        if(other != null)
        {
            Vector3 lookTo = other.transform.position;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            asset.transform.LookAt(lookTo);
            asset.transform.localScale = new Vector3(1, 1, dist * offset);
        }
    }
}
