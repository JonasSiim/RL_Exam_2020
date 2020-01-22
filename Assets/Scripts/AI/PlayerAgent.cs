using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent
{
    //Initialize Variables
    public CharacterController2D controller;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    [SerializeField] private GameObject[] spawnPos;
    private int n;
    private bool collidedWithPit;
    private bool collidedWithReward;
    private int simulatedMovement;
    [SerializeField] private GameObject rewardTransform;
    [SerializeField] private GameObject wallLeft;
    [SerializeField] private GameObject pitOne;
    [SerializeField] private GameObject pitTwo;
    [SerializeField] private GameObject pitThree;
    [SerializeField] private GameObject wallRight;
    [SerializeField] private GameObject rewardText;
    [SerializeField] private GameObject resetText;
    [SerializeField] private GameObject PlayerSpawn;
    [SerializeField] private GameObject Ground;
    private int rewardScore;
    private int resetScore = -1;



    private void Start()
    {
        
    }

    public override void AgentReset() //defining what happens when the agent collides with an undesired object
    {
        
        gameObject.transform.position = PlayerSpawn.transform.position; //respawning the player to a default position
        this.collidedWithPit = false; //Flip boolean switch to false
        this.resetScore++; //Increment UI score

    }

    public override void CollectObservations() //Collect observations of the environment. Overall = 22 observations with 6 stacked vectors.
    {
        AddVectorObs(this.transform.localPosition);
        AddVectorObs(this.rewardTransform.transform.localPosition);
        AddVectorObs(this.wallLeft.transform.localPosition);
        AddVectorObs(this.pitOne.transform.localPosition);
        AddVectorObs(this.pitTwo.transform.localPosition);
        AddVectorObs(this.pitThree.transform.localPosition);
        AddVectorObs(this.wallRight.transform.localPosition);
        AddVectorObs(this.simulatedMovement);

    }

    // Update is called once per frame
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        horizontalMove = simulatedMovement * runSpeed; //Creating variable for the movement function from character controller

        var movement = (int)vectorAction[0]; //set  input signals to var in order to set up switch statement



        switch (movement)
        {
            case 0:
                simulatedMovement = 0; //Agent stands still
                break;
            case 1:
                simulatedMovement = 1; //Agent moves right
                break;
            case 2:
                simulatedMovement = -1; //Agent moves left
                break;
            case 3:
                jump = true; //Agent jumps
                break;
        }


        if (collidedWithReward) 
        {
            AddReward(0.5f); //add reward
            this.collidedWithReward = false; //flip boolean to false
            this.rewardScore++; //update UI
        }
        else if(collidedWithPit)
        {
            Done(); //call Reset
        }

        
    }

    public override void AgentOnDone()
    {

    }

    private void Update()
    {
        this.rewardText.GetComponent<TextMesh>().text = "Rewards: " + this.rewardScore.ToString(); //Declare UI text
        this.resetText.GetComponent<TextMesh>().text = "Resets: " + this.resetScore.ToString(); //Declare UI text

        if (gameObject.transform.localPosition.y < Ground.transform.localPosition.y) //If agent goes out of bounds
        {
           gameObject.transform.position = PlayerSpawn.transform.position; //Reset position
        }
    }

    void FixedUpdate()
    {
        //move our character
        this.controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump); //calling movement function from character controller script
        this.jump = false; //to avoid double jumping

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Reward") //if the player enters a reward
        {
            this.collidedWithReward = true; //flip collect reward to true
            this.n++; //increment index integer
            collision.gameObject.transform.position = spawnPos[n].transform.position; //spawn reward at new position

            if (n >= spawnPos.Length - 1) //avoid index integer from going out of bounds
            {
                this.n = -1;
            }
        }
        else if (collision.gameObject.tag == "pit") //if collided with pit
        {
            this.collidedWithPit = true; //flip reset to true

        }
    }
}
