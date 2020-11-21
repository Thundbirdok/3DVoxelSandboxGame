using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

	[SerializeField]
	private Transform cam = null;
	[SerializeField]
	private World world = null;

	[SerializeField]
	private float walkSpeed = 0;
	[SerializeField]
	private float sprintSpeed = 0;
	[SerializeField]
	private float jumpForce = 0;
	[SerializeField]
	private float gravity = 0;

	[SerializeField]
	private float playerWidth = 0;

	[SerializeField]
	private int ViewDistanceInChunks = 0;

	private bool isAlive;

	//Movement
	private bool isGrounded;
	private bool isSprinting;
	private bool jumpRequest;

	private float horizontal;
	private float vertical;
	private float mouseHorizontal;
	private float mouseVertical;
	private Vector3 velocity;
	private float verticalMomentum = 0;

	/// <summary>
	/// Chunks that in player view and active
	/// </summary>
	private List<Vector2Int> ChunksInView;

	private Vector2Int CurrentChunkCoord;
	private Vector2Int PreviousChunkCoord;

	//Toolbar
	[SerializeField]
	private Toolbar toolbar = null;
	[SerializeField]
	private UIController uicontroller = null;

	public byte selectedBlockIndex = 1;

	//Create and Destroy
	public Transform highlightBlock;
	public Transform placeBlock;
	public float checkIncrement = 0.1f;
	public float reach = 8f;

	private Vector3 destroyedBlockPosition;
	private float destroyedBlockDurability;
	private BlockType destroyedBlock;

	[SerializeField]
	private float DefaultDestructionPower = 1;

	private void Start()
	{

		ChunksInView = new List<Vector2Int>();

		Cursor.lockState = CursorLockMode.Locked;

	}

	private void FixedUpdate()
	{

		if (!uicontroller.IsInUI)
		{

			CheckWorldBounds();
			CalculateVelocity();

			if (jumpRequest)
			{

				Jump();

			}

			transform.Rotate(Vector3.up * mouseHorizontal);

			mouseVertical = Mathf.Clamp(mouseVertical, -85, 85);
			var euler = cam.transform.localEulerAngles;
			euler.x = mouseVertical;
			cam.transform.localEulerAngles = euler;			

			transform.Translate(velocity, Space.World);

		}

	}

	private void Update()
	{

		if (!uicontroller.IsInUI)
		{

			GetPlayerInputs();
			placeCursorBlocks();

		}

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

			velocity.y = CheckDownSpeed(velocity.y);

		}
		else if (velocity.y > 0)
		{

			velocity.y = CheckUpSpeed(velocity.y);

		}

	}

	private void GetPlayerInputs()
	{

		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		mouseHorizontal = Input.GetAxis("Mouse X");
		mouseVertical -= Input.GetAxis("Mouse Y");

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

		if (highlightBlock.gameObject.activeSelf)
		{

			// Destroy block.
			if (Input.GetMouseButton(0))
			{

				DestroyBlock();

			}

			if (Input.GetMouseButtonUp(0))
			{

				destroyedBlockPosition = Vector3.one * -1;

			}

			// Place block.
			if (Input.GetMouseButtonDown(1))
			{

				if (toolbar.HasItemInSlot())
				{

					world.EditVoxel(placeBlock.position, toolbar.GetSelectedItemID());
					toolbar.TakeItemFromSlot(1);

				}

			}

		}

	}

	private void DestroyBlock()
	{

		if (destroyedBlockPosition == Vector3.one * -1)
		{

			destroyedBlockPosition = highlightBlock.position;
			destroyedBlock = world.BlocksAttributes.Blocktypes[world.GetBlockID(destroyedBlockPosition)];
			destroyedBlockDurability = destroyedBlock.Durability;

		}

		if (highlightBlock.position == destroyedBlockPosition)
		{

			float power;

			if (toolbar.HasItemInSlot())
			{

				byte id = toolbar.GetSelectedItemID();

				var item = world.ItemsAttributes.GetItem(id);

				if (item != null && item.Type == destroyedBlock.TargetTool)
				{

					power = item.Power;

				}
				else
				{

					power = DefaultDestructionPower;

				}

			}
			else
			{

				power = DefaultDestructionPower;

			}

			destroyedBlockDurability -= power * Time.fixedDeltaTime;


			if (destroyedBlockDurability <= 0)
			{

				world.EditVoxel(highlightBlock.position, 0);
				destroyedBlockPosition = Vector3.one * -1;

				if (destroyedBlock.Drop != 0)
				{

					toolbar.PutStack(new ItemStack(destroyedBlock.Drop, 1, 64));

				}

			}

		}
		else
		{

			destroyedBlockPosition = Vector3.one * -1;

		}

	}

	private void placeCursorBlocks()
	{

		float step = checkIncrement;
		Vector3 lastPos = new Vector3();

		while (step < reach)
		{

			Vector3 pos = cam.position + (cam.forward * step);

			if (world.IsVoxelSolid(pos))
			{

				highlightBlock.position = Vector3Int.FloorToInt(pos);
				placeBlock.position = lastPos;

				highlightBlock.gameObject.SetActive(true);
				placeBlock.gameObject.SetActive(true);

				return;

			}

			lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

			step += checkIncrement;

		}

		highlightBlock.gameObject.SetActive(false);
		placeBlock.gameObject.SetActive(false);

	}

	private float CheckDownSpeed(float downSpeed)
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

	private float CheckUpSpeed(float upSpeed)
	{

		if (
			world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z - playerWidth) ||
			world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z - playerWidth) ||
			world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z + playerWidth) ||
			world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z + playerWidth)
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

		CheckChunksInViewDistance();

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

				// If the current chunk is in the world add in view
				if (world.IsChunkInWorld(chunkCoord))
				{

					world.Chunks[x, z].isActive = true;

					ChunksInView.Add(chunkCoord);

				}

				// Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
				for (int i = 0; i < previouslyActiveChunks.Count;)
				{

					if (previouslyActiveChunks[i].Equals(chunkCoord))
					{

						previouslyActiveChunks.RemoveAt(i);

					}
					else
					{

						++i;

					}

				}

			}

		}

		// Any chunks left in the previousActiveChunks list are no longer in the player's view distance, so loop through and disable them.
		foreach (Vector2Int chunk in previouslyActiveChunks)
		{

			world.Chunks[chunk.x, chunk.y].isActive = false;

			for (int i = 0; i < ChunksInView.Count;)
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