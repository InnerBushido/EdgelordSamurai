using UnityEngine;
using System;


public class RandomRotate:MonoBehaviour{
    Quaternion rotTarget;
    public float rotateEverySecond = 1.0f;
    float lerpCounter;
    
    public void Start() {
    	randomRot ();
    	InvokeRepeating("randomRot", 0.0f,rotateEverySecond);
    }
    
    public void Update(){
    	transform.rotation = Quaternion.Lerp(transform.rotation, rotTarget, lerpCounter*Time.deltaTime);
    	lerpCounter++;
    }
    
    public void randomRot() {
    	 rotTarget = UnityEngine.Random.rotation;
    	 lerpCounter = 0.0f;
    }
}