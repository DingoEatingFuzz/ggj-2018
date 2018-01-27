﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCursor : MonoBehaviour {

	private Color _color = Color.white;
	public Color color {
		get {
			return _color;
		}
		set {
			_color = value;
			closedHandSprite.GetComponent<Image>().color = value;
			openHandSprite.GetComponent<Image>().color = value;
		}
	}
	public bool isOpen = true;
	public CarPart part;

	public GameObject openHandSprite;
	public GameObject closedHandSprite;

	public string inputX;
	public string inputY;

	private bool grabbing = false;

	public HandCursor(Color color, string inputX, string inputY) {
		this.color = color;
		this.inputX = inputX;
		this.inputY = inputY;
	}

	// Use this for initialization
	void Start () {
		closedHandSprite.SetActive(false);
		closedHandSprite.GetComponent<Image>().color = color;
		openHandSprite.GetComponent<Image>().color = color;
		RandomizeColor();
	}

	void RandomizeColor() {
		color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && !grabbing) {
			grabbing = true;
			Grab();
		}

		if (Input.GetMouseButtonUp(0) && grabbing) {
			grabbing = false;
		}

		MoveCursor();
	}

	void MoveCursor() {
		float speed = 15;
		Debug.Log("X:" + Input.GetAxis(inputX) * speed);
		Debug.Log("Y:" + Input.GetAxis(inputY) * speed);
		this.transform.position += new Vector3(Input.GetAxis(inputX) * speed, Input.GetAxis(inputY) * speed, 0);
	}

	void Grab() {
		CloseHand();

		// Check if successfully grabbed something, otherwise
		Invoke("OpenHand", 0.25f);
	}

	void OpenHand() {
		isOpen = true;
		closedHandSprite.SetActive(false);
		openHandSprite.SetActive(true);
	}

	void CloseHand() {
		isOpen = false;
		closedHandSprite.SetActive(true);
		openHandSprite.SetActive(false);
	}
}