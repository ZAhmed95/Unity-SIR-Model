using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SIR_model : MonoBehaviour {

	public RectTransform button;
	//lines 9-16 store references to the neighbor cells
	public GameObject up;
	public SIR_model north;
	public GameObject down;
	public SIR_model south;
	public GameObject left;
	public SIR_model west;
	public GameObject right;
	public SIR_model east;
	public GameObject[] grid;
	public RectTransform[] gridRect;
	public Image image;
	public float S; //# of susceptibles
	public float I; //# of infected
	public float R; //# of dead
	//public float M; //# of military personnel
	public float N; //total population
	public float Noriginal; //initial N value of cell
	public float Nmax; //maximum capacity of a cell
	public float Sn; //fraction of susceptible
	public float In; //fraction of infected
	public float Rn; //fraction of dead
	//public float Mn; //fraction of military
	public float a; //infection rate; defined as number of people one infected person will infect per time step
	public float b; //death rate; defined as the number of infected people that will die per time step
	//public float c; //infection rate for military; same idea as 'a' but lower value because soldiers are trained to avoid infecteds
	//public float k; //kill rate; same as 'b' but higher value because soldiers actively hunt and kill infected
	public float ss; //max susceptible migration rate per direction; how many susceptibles can escape to neighboring cells per time step
	public float ii; //max infected migration rate per direction; how many infecteds can spread to neighboring cells per time step
	//public float mm; //max military migration rate per direction; how many soldiers can enter from neighboring cells per time step
	public int clock; //time interval
	public float deltaT; //time step
	public int clockmax; //simulation range
	public float[] Slist; //to store all S values
	public float[] Ilist; //to store all I values
	public float[] Rlist; //to store all R values
	//public float[] Mlist; //to store all M values
	public float[] Nlist; //to store all N values
	public bool isPaused;
	public bool infect;

	void Start()
	{

		button = (RectTransform)gameObject.transform;
		grid = GameObject.FindGameObjectsWithTag ("Button");
		gridRect = new RectTransform[grid.Length];

		for (int i = 0; i < grid.Length; i++)
		{
			gridRect[i] = (RectTransform)grid[i].transform;
		}

		image = GetComponent<Image> ();
		clockmax = 5000;
		clock = 0;
		Slist = new float[clockmax+1];
		Ilist = new float[clockmax+1];
		Rlist = new float[clockmax+1];
		//Mlist = new float[clockmax+1];
		Nlist = new float[clockmax+1];
		Slist [0] = 7900000;
		Ilist [0] = 0;
		Rlist [0] = 0;
		//Mlist [0] = 0;
		Nlist [0] = Slist [0] + Ilist [0] + Rlist[0];
		Noriginal = 7900000;
		Nmax = 12000000;
		a = 2f;
		b = 0.33f;
		//c = 1f;
		//k = 10000f;
		ss = 45000;
		ii = 20000f;
		//mm = 100;
		deltaT = 1f;
		isPaused = true;
		infect = true;
	}

	public void SIR()
	{
		if (clock < clockmax && !isPaused)
			/*using if statement instead 
			of for loop*/
		{
			S = Slist [clock]; //get current S
			I = Ilist [clock]; //get current I
			R = Rlist [clock]; //get current R
			N = S + I + R;
			//set the fractions
			Sn = (S / N);
			In = (I / N);
			Rn = (R / N);

			//run the numerical methods
			Slist [clock + 1] = (S - a * Sn * I * deltaT);
			//some get infected
			if(Slist[clock+1]<1) Slist[clock+1] = 0;
			Ilist [clock + 1] = (I + (a*Sn*I - b*I) * deltaT);
			//new infections from S; natural recovery;
			Rlist [clock + 1] = (R + (b * I * In) * deltaT); 
			//recovered from infecteds
			Nlist[clock + 1] = Slist[clock+1] + Ilist[clock+1]
							   + Rlist[clock+1];
			SusceptibleSpread(clock); 
			//run susceptible migration function
			InfectionSpread(clock);
			//run infected migration function
			clock++; //iterate clock
		}


	}

	public void Restart()
	{
		clock = 0;
	}

	public void TogglePlay()
	{
		isPaused = !isPaused;
	}

	/*control spread of infecteds from
	 *this cell to neighbors*/
	void InfectionSpread(int time)
	{
		float rateI = (ii * In);
		/* rate dependent on In because
		 * more infecteds means more
		 * chance for them to migrate*/
		//repeat for all directions
		if (north != null) 
		{
			if (north.Nlist[time] + rateI <= Nmax)
			{
				//transfer infecteds from here to there
				Ilist[time] -= rateI;
				north.Ilist[time] += rateI;
			}
		}
		if (south != null) 
		{
			if (south.Nlist[time] + rateI <= Nmax)
			{
				Ilist[time] -= rateI;
				south.Ilist[time] += rateI;
			}
		}
		if (west != null) 
		{
			if (west.Nlist[time] + rateI <= Nmax)
			{
				Ilist[time] -= rateI;
				west.Ilist[time] += rateI;
			}
		}
		if (east != null) 
		{
			if (east.Nlist[time] + rateI <= Nmax)
			{
				Ilist[time] -= rateI;
				east.Ilist[time] += rateI;
			}
		}
	}

	//controls spread of susceptibles to neighbors (basically almost a clone of the infected spread function)
	void SusceptibleSpread(int time)
	{
		float rateS = (ss * In * Sn); //rate dependent on In because susceptibles want to escape infecteds 
		if (north != null) 
		{
			if (north.Nlist[time] + rateS <= Nmax)
			{
				//transfer susceptibles from here to there
				Slist[time] -= rateS;
				north.Slist[time] += rateS;
			}
		}
		if (south != null) 
		{
			if (south.Nlist[time] + rateS <= Nmax)
			{
				Slist[time] -= rateS;
				south.Slist[time] += rateS;
			}
		}
		if (west != null) 
		{
			if (west.Nlist[time] + rateS <= Nmax)
			{
				Slist[time] -= rateS;
				west.Slist[time] += rateS;
			}
		}
		if (east != null) 
		{
			if (east.Nlist[time] + rateS <= Nmax)
			{
				Slist[time] -= rateS;
				east.Slist[time] += rateS;
			}
		}
	}

	//controls military deployment (again, a near clone of the above two functions)
	/*void Military(int time)
	{
		float rateM = Mathf.Round(mm * In); //rate dependent on In because military go to most infected areas

		//transfer soldiers from here to there. This time it's a bit different because the infected cell 'pulls' soldiers instead of pushing them out
		//which means we must also include a new check to make sure the neighbors actually have any soldiers to pull in
		if (north != null && north.Mlist[time] >= rateM) 
		{
			Mlist[time] += rateM;
			north.Mlist[time] -= rateM;
		}
		if (south != null && south.Mlist[time] >= rateM) 
		{
			Mlist[time] += rateM;
			south.Mlist[time] -= rateM;
		}
		if (west != null && west.Mlist[time] >= rateM) 
		{
			Mlist[time] += rateM;
			west.Mlist[time] -= rateM;
		}
		if (east != null && east.Mlist[time] >= rateM) 
		{
			Mlist[time] += rateM;
			east.Mlist[time] -= rateM;
		}
	}*/

	public void SetNeighbors()
	{
		north = up == null ? null : up.GetComponent<SIR_model>();
		south = down == null ? null : down.GetComponent<SIR_model>();
		west = left == null ? null : left.GetComponent<SIR_model>();
		east = right == null ? null : right.GetComponent<SIR_model>();
	}

	//infect a cell
	public void Infect()
	{
		if (infect)
		{
			Ilist [0] += 10;
			Noriginal += 10;
			Nlist[0] += 10;
		}
	}

	//deploy soldiers to a cell
	public void Deploy()
	{
		if (!infect)
		{
			//Mlist [0] += 1000;
			Noriginal += 1000;
			Nlist[0] += 1000;
		}
	}

	//toggle between infecting or deploying
	public void ToggleInfect()
	{
		infect = !infect;
	}

	//visualize data (cell size, color)
	public void Visualize()
	{
		if (clock < clockmax && !isPaused)
		{
			button.localScale = new Vector3(1,1,1)* (Rlist[clock]+Slist[clock])/Noriginal;
			image.color = new Vector4(In,0,0,1);
		}
	}
}
