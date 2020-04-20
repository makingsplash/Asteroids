using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMet : MonoBehaviour
{
	private Rigidbody2D rgbd;

	private void Start()
	{
		rgbd = GetComponent<Rigidbody2D>();

		rgbd.velocity = Vector2.up * 4;
	}

}
