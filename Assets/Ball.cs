using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody rb;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }
}
