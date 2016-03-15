using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Setup : MonoBehaviour {

	public GameObject panel;
	public GridLayoutGroup layout;
	public GameObject cell;
	public int gridSize;
	public GameObject[,] cells;
	public SIR_model[,] sirs;
	public bool initialized;
	public float Stotal;
	public float Itotal;
	public float Rtotal;
	public Text Stext;
	public Text Itext;
	public Text Rtext;

	void Start()
	{
		gridSize = 9;
		initialized = false;
	}

	void Update()
	{
		if (Input.GetKeyDown("s")) GridLayout();
		if (Input.GetKeyDown("f")) FindNeighbors();
		if(Input.GetKeyDown("w"))
		{
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					sirs[j,i].Restart();
				}
			}
		}
		
		if(Input.GetKeyDown("p"))
		{
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					sirs[j,i].TogglePlay();
				}
			}
		}
		
		if(Input.GetKeyDown("t"))
		{
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					sirs[j,i].ToggleInfect();
				}
			}
		}

		if (initialized)
		{
			float Scurrent = 0;
			float Icurrent = 0;
			float Rcurrent = 0;
			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					sirs[j,i].SIR();
				}
			}

			for (int i = 0; i < gridSize; i++)
			{
				for (int j = 0; j < gridSize; j++)
				{
					sirs[j,i].Visualize();
					Scurrent += sirs[j,i].S;
					Icurrent += sirs[j,i].I;
					Rcurrent += sirs[j,i].R;
				}
			}
			Stotal = Mathf.Floor(Scurrent);
			Stext.text = "S = " + Stotal;
			Itotal = Mathf.Floor(Icurrent);
			Itext.text = "I = " + Itotal;
			Rtotal = Mathf.Floor(Rcurrent);
			Rtext.text = "R = " + Rtotal;

			if (Stotal == 0 || Itotal == 0)
			{
				for (int i = 0; i < gridSize; i++)
				{
					for (int j = 0; j < gridSize; j++)
					{
						sirs[j,i].isPaused = true;
					}
				}
			}
		}
	}
	
	void GridLayout()
	{
		layout.constraintCount = gridSize;
		cells = new GameObject[gridSize,gridSize];
		sirs = new SIR_model[gridSize, gridSize];

		for (int i = 0; i < gridSize; i++)
		{
			for (int j = 0; j < gridSize; j++)
			{
				cells[j,i] = Instantiate(cell, panel.transform.position, panel.transform.rotation) as GameObject;
				cells[j,i].transform.SetParent(panel.transform);
				cells[j,i].name = "cell " + (j+1).ToString() + "," + (i+1).ToString();
				sirs[j,i] = cells[j,i].GetComponent<SIR_model>();
			}
		}
		initialized = true;
	}

	void FindNeighbors()
	{
		for (int i = 0; i < gridSize; i++)
		{
			for (int j = 0; j < gridSize; j++)
			{
				SIR_model x = cells[j,i].GetComponent<SIR_model>();
				x.up = i-1 < 0 ? null : cells[j,i-1];
				x.down = i+1 == gridSize ? null : cells[j,i+1];
				x.right = j+1 == gridSize ? null : cells[j+1,i];
				x.left = j-1 < 0 ? null : cells[j-1,i];
				x.SetNeighbors();
			}

		}
	}
}
