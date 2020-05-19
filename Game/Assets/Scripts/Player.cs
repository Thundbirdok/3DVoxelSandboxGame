using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    

    [SerializeField]
    private Transform cam;
    [SerializeField]
    private World world;

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float sprintSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float gravity;

    [SerializeField]
    private float playerWidth;

    [SerializeField]
    private int ViewDistanceInChunks;

    private bool isAlive;

    private bool isGrounded;
    private bool isSprinting;
    private bool jumpRequest;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;    

    private List<Vector2Int> ChunksInView;

    private Vector2Int CurrentChunkCoord;
    private Vector2Int PreviousChunkCoord;    

    private void Start()
    {

        ChunksInView = new List<Vector2Int>();

        CheckChunksInViewDistance();

    }

    private void FixedUpdate()
    {

        CheckWorldBounds();
        CalculateVelocity();        

        if (jumpRequest)
        {

            Jump();

        }

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);

    }    

    private void Update()
    {

        GetPlayerInputs();

        CurrentChunkCoord = world.GetChunkCoord(transform.position);

        // Only update the chunks if the player has moved from the chunk they were previously on.
        if (!CurrentChunkCoord.Equals(PreviousChunkCoord))
        {

            CheckChunksInViewDistance();

        }

    }

    void Jump()
    {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }

    private void CalculateVelocity()
    {

        // Affect vertical momentum with gravity.
        if (verticalMomentum > -gravity)
        {

            verticalMomentum += Time.fixedDeltaTime * -gravity;

        }

        // if we're sprinting, use the sprint multiplier.
        if (isSprinting)
        {

            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;

        }
        else
        {

            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        }

        // Apply vertical momentum (falling/jumping).
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
        {

            velocity.z = 0;

        }

        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
        {

            velocity.x = 0;

        }

        if (velocity.y < 0)
        {

            velocity.y = checkDownSpeed(velocity.y);

        }
        else if (velocity.y > 0)
        {

            velocity.y = checkUpSpeed(velocity.y);

        }

    }

    private void GetPlayerInputs()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint") && isGrounded)
        {

            isSprinting = true;

        }

        if (Input.GetButtonUp("Sprint") && isGrounded)
        {

            isSprinting = false;

        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {

            jumpRequest = true;

        }

    }

    private float checkDownSpeed(float downSpeed)
    {

        if (
            world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
            world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
            world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) ||
            world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)
           )
        {

            isGrounded = true;

            return 0;

        }
        else
        {

            isGrounded = false;

            return downSpeed;

        }

    }

    private float checkUpSpeed(float upSpeed)
    {

        if (
            world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth) ||
            world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth) ||
            world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth) ||
            world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)
           )
        {

            return 0;

        }
        else
        {

            return upSpeed;

        }

    }

    public bool front
    {

        get
        {
            if (
                world.IsVoxelSolid(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
                world.IsVoxelSolid(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)
                )
            {

                return true;

            }
            else
            {

                return false;

            }
        }

    }

    public bool back
    {

        get
        {
            if (
                world.IsVoxelSolid(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
                world.IsVoxelSolid(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

    }

    public bool left
    {

        get
        {
            if (
                world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
                world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

    }

    public bool right
    {

        get
        {
            if (
                world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y, transform.position.z) ||
                world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z)
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

    }

    public void Spawn(Vector3 pos)
    {

        transform.position = pos;

        isAlive = true;

    }

    private void CheckWorldBounds()
    {

        if (transform.position.x <= 1)
        {

            transform.position += new Vector3(1, 0, 0);

        }
        else if (transform.position.x >= world.WorldAttributes.WorldSizeInBlocks - 1)
        {

            transform.position += new Vector3(-1, 0, 0);

        }

        if (transform.position.z <= 1)
        {

            transform.position += new Vector3(0, 0, 1);

        }
        else if (transform.position.z >= world.WorldAttributes.WorldSizeInBlocks - 1)
        {

            transform.position += new Vector3(0, 0, -1);

        }

    }

    void CheckChunksInViewDistance()
    {
        
        if (!isAlive)
        {

            return;

        }

        PreviousChunkCoord = CurrentChunkCoord;

        List<Vector2Int> previouslyActiveChunks = new List<Vector2Int>(ChunksInView);

        // Loop through all chunks currently within view distance of the player.
        for (int x = CurrentChunkCoord.x - ViewDistanceInChunks; x <= CurrentChunkCoord.x + ViewDistanceInChunks; ++x)
        {

            for (int z = CurrentChunkCoord.y - ViewDistanceInChunks; z <= CurrentChunkCoord.y + ViewDistanceInChunks; ++z)
            {

                Vector2Int chunkCoord = new Vector2Int(x, z);

                // If the current chunk is in the world...
                if (world.IsChunkInWorld(chunkCoord))
                {

                    world.Chunks[x, z].isActive = true;

                    ChunksInView.Add(chunkCoord);

                }

                // Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {

                    if (previouslyActiveChunks[i].Equals(chunkCoord))
                        previouslyActiveChunks.RemoveAt(i);

                }

            }

        }

        // Any chunks left in the previousActiveChunks list are no longer in the player's view distance, so loop through and disable them.
        foreach (Vector2Int chunk in previouslyActiveChunks)
        {

            world.Chunks[chunk.x, chunk.y].isActive = false;
            
            for (int i = 0; i < ChunksInView.Count; )
            {

                if (world.Chunks[chunk.x, chunk.y].Equals(ChunksInView[i]))
                {

                    ChunksInView.RemoveAt(i);

                }
                else
                {

                    ++i;

                }

            }

        }

    }

}