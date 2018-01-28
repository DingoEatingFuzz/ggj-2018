﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarBuilder : MonoBehaviour {

	public uint playerCount = 2;
	public PartPicker PartPicker;
	public GameObject partPickerContainer;
	public PartPlacementTile PartPlacementTile;
	public GameObject partPlacementContainer;
	private bool[,] grid;
	private PartPlacementTile[,] tileGrid;
	public List<PartPlacement> parts = new List<PartPlacement>();

	public GameObject PartSheet;
	public HandCursor[] cursors;

	// Use this for initialization
	void Start () {
		// Create a part for each player + 2 for some options
		var pickers = new List<PartPicker>();
		var height = this.GetComponent<RectTransform>().rect.height;
		grid = new bool[playerCount + 2, playerCount + 2];
		tileGrid = new PartPlacementTile[playerCount + 2, playerCount + 2];

		for (var i = 0; i < playerCount + 2; i++) {
			var partPicker = Instantiate(PartPicker);
			var partPickerSize = partPicker.GetComponent<RectTransform>().rect.height + 5;
			partPicker.transform.SetParent(this.partPickerContainer.transform, false);
			partPicker.transform.position += new Vector3(
				(i % 2 * -partPickerSize) + partPickerSize*0.75f,
				-partPickerSize*(i / 2),
				0
			);
			pickers.Add(partPicker);
		}

		// Create a grid (scaled by the number of players)
		// 4 x 4
		// 5 x 5
		// 6 x 6
		float size = 0;
		var tiles = new List<PartPlacementTile>();
		for (var i = 0; i < playerCount + 2; i++) {
			for (var j = 0; j < playerCount + 2; j++) {
				var tile = Instantiate(PartPlacementTile);
				tile.x = (uint)i;
				tile.y = (uint)(playerCount + 1 - j);
				tile.transform.SetParent(this.partPlacementContainer.transform, false);

				var rect = tile.GetComponent<RectTransform>().rect;
				size = rect.height;
				tile.transform.position += new Vector3(
					size * i,
					size * j,
					0
				);
				tileGrid[i, playerCount + 1 - j] = tile;
				tiles.Add(tile);
			}
		}

		foreach (var cursor in cursors)
		{
			cursor.partOptions = pickers.ToArray();
			cursor.tileOptions = tiles.ToArray();
		}

		// Position the entire grid so it is roughly centered in the screen aside from the tray
		// Due to relative resizing of elements, it isn't quite centered. Oh well.
		var sizeX = size * (playerCount / 2 + 0.5f);
		var containerRect = this.partPlacementContainer.GetComponent<RectTransform>().rect;
		this.partPlacementContainer.transform.position -= new Vector3(sizeX, sizeX, 0);

		// Populate the sprite map
	}

	// Update is called once per frame
	void Update () {
		foreach (var tile in tileGrid) {
			tile.GetComponent<Image>().color = Color.white;
		}
	}

	bool PlacementIsValid(CarPart part, uint x, uint y) {
		// Part needs to fit in the grid
		if (y + part.height > grid.GetLength(1) || y + part.height < 0) {
			return false;
		}
		// Part need to fit in the grid
		if (x + part.width > grid.GetLength(0) || x + part.width < 0) {
			return false;
		}

		// Part can't be placed on top of an existing part
		for (var yCell = y; yCell < y + part.height; yCell++) {
			for (var xCell = x; xCell < x + part.width; xCell++) {
				if (grid[xCell, yCell]) return false;
			}
		}

		return true;
	}

	public void IntentToPlacePart(CarPart part, uint x, uint y) {
		var isValid = PlacementIsValid(part, x, y);
		for (var yCell = y; yCell < y + part.height; yCell++) {
			for (var xCell = x; xCell < x + part.width; xCell++) {
				if (xCell < tileGrid.GetLength(0) && yCell < tileGrid.GetLength(1)) {
					tileGrid[xCell,yCell].GetComponent<Image>().color = isValid ? Color.yellow : Color.red;
				}
			}
		}
	}

	public bool PlacePart(CarPart part, uint x, uint y) {
		if (PlacementIsValid(part, x, y)) {
			// Add a part with coordinates to the list of parts
			parts.Add(new PartPlacement(part, x, y));

			var sprite = Instantiate(part.sprite);
			sprite.transform.SetParent(this.partPlacementContainer.transform, false);

			var rect = tileGrid[0,0].GetComponent<RectTransform>().rect;
			var size = rect.height;

			// Correct for position
			sprite.transform.position += new Vector3(
				size * x,
				size * (playerCount + 1 - y),
				0
			);

			// Correct for tile size
			Debug.Log(part.width + "  " + part.height);
			sprite.transform.position -= new Vector3(
				(part.width - 1) * (size / 2),
				(part.height - 1) * (size / 2),
				0
			);

			// Update the grid so parts can't be placed on top of each other
			for (var yCell = 0; yCell < part.height; yCell++) {
				for (var xCell = 0; xCell < part.width; xCell++) {
					grid[xCell + x, yCell + y] = true;
				}
			}

			return true;
		}
		return false;
	}
}
