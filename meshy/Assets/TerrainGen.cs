using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGen : MonoBehaviour {

	
	public int mapSize = 32;
	public float noiseScale = 0.25f;
	public float distance = 1.0f;
	public int smoothness = 3;
	public float power = 1;


	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();


	private Mesh mesh;
	private MeshCollider meshCollider;


	private int uvCount = 0;


	void Fractate (float[,] h, int order, float val){
		//O(n^4) of nested loops in one function!
		float ordersqrt2 = order * 1.414f / 2.0f;


		for(int i = 0; i < mapSize/order; i++){
			for(int j = 0; j < mapSize/order; j++){
				float displacement = Random.Range(-val, val);

				for(int k = 0; k < order; k++){
					for(int l = 0; l < order; l++){
						//shitty way of doing a kernel about the center of each order segment
						float xdisp = (order/2.0f) - k;
						float ydisp = (order/2.0f) - l;


						float scalar = 1.0f - Mathf.Sqrt(xdisp*xdisp + ydisp*ydisp)/ordersqrt2;


						//apply displacement
						h[i*order+k,j*order+l] += displacement * scalar;
					}
				}

			}
		}

	}

	void Plateau(float[,] h, float dist, float weight){
		float aweight = 1.0f - weight;
		for(int k = 0; k < mapSize-1; k++){
			for(int j = 0; j < mapSize-1; j++){
				if(Mathf.Abs(h[k,j]-h[k+1,j]) < dist){
					h[k,j] = aweight * h[k,j] + weight * h[k+1,j];
				}
				if(Mathf.Abs(h[k,j]-h[k,j+1]) < dist){
					h[k,j] = aweight * h[k,j] + weight * h[k,j+1];
				}
			}
		}

	}


	void Smooth(float[,] h, int iterations, float weight){
		float aweight = (1.0f - weight)/4.0f;

		float weight2 = weight + 2*aweight;
		float aweight2 = 2*aweight;

		for(int k = 0; k < iterations; k++){
			
			for(int i = 1; i < mapSize-1; i++){
				h[i,0] = weight2*h[i,0] + aweight2*h[i-1,0] + aweight2*h[i+1,0];
				h[i,mapSize-1] = weight2*h[i,mapSize-1] + aweight2*h[i-1,mapSize-1] + aweight2*h[i+1,mapSize-1];
				h[0,i] = weight2*h[0,i] + aweight2*h[0,i-1] + aweight2*h[0,i+1];
				h[mapSize-1,i] = weight2*h[mapSize-1,i] + aweight2*h[mapSize-1,i-1] + aweight2*h[mapSize-1,i+1];
			}
			
			for(int i = 1; i < mapSize-1; i++){
				for(int j = 1; j < mapSize-1; j++){
					h[i,j] = h[i,j] * weight +
						h[i-1,j] * aweight +
							h[i+1,j] * aweight +
							h[i,j-1] * aweight +
							h[i,j+1] * aweight;
				}
			}
		}
	}

	float safeGet(float[,] h, int x, int y){
		if(x >= mapSize || x < 0 || y >= mapSize || y < 0){
			return 0;
		}
		return h[x,y];
	}

	int[] fit(int x, int y, int z, int w){
		int[] n = {x,y,z,w};
		return n;
	}

	void DiamondSquare(float[,] h, int xminarg, int xmaxarg, int yminarg, int ymaxarg){



		Queue<int[]> toProcess = new Queue<int[]>();

		toProcess.Enqueue(fit (xminarg, xmaxarg,yminarg,ymaxarg));

		while(toProcess.Count > 0){
			int[] vals = toProcess.Dequeue();

			int xmin = vals[0];
			int xmax = vals[1];
			int ymin = vals[2];
			int ymax = vals[3];

			//stop at this point for the test
			if(xmax - xmin < 2){
				//	Debug.Log("xmin xmax: " + xmin + " " + xmax);
				continue;
			}
			
			int size = xmax - xmin;
			int half = size/2;
			
			
			float scale = Mathf.Pow(size,power) * noiseScale;

			//square
			float average = (h[xmin,ymin] + h[xmin,ymax] + h[xmax,ymax] + h[xmax,ymin])/4.0f;
			h[xmin + half,ymin + half] = average + Random.Range(-scale,scale); 
			
			
			//diamond
			float bottomAverage = (h[xmin,ymin] + h[xmax,ymin] + h[xmin+half,ymin+half] + safeGet(h,xmin+half,ymin-half))/4.0f;
			float leftAverage = (h[xmin,ymin] + h[xmin,ymax] + h[xmin+half, ymin+half] + safeGet (h,xmin-half,ymin+half))/4.0f;
			float topAverage = (h[xmin,ymax] + h[xmax,ymax] + h[xmin+half,ymin+half] + safeGet (h,xmin+half,ymax+half))/4.0f;
			float rightAverage = (h[xmax,ymin] + h[xmax,ymax] + h[xmin+half,ymin+half] + safeGet (h,xmax+half,ymin+half))/4.0f;
			
			h[xmin+half,ymin] = bottomAverage + Random.Range(-scale,scale);
			h[xmin,ymin+half] = leftAverage + Random.Range(-scale,scale);
			h[xmin+half,ymax] = topAverage + Random.Range(-scale,scale);
			h[xmax,ymin+half] = rightAverage + Random.Range(-scale,scale);





			toProcess.Enqueue(fit (xmin,xmin+half,ymin,ymin+half));
			toProcess.Enqueue(fit (xmin+half,xmax,ymin,ymin+half));

			toProcess.Enqueue(fit (xmin,xmin+half,ymin+half, ymax));
			toProcess.Enqueue(fit (xmin+half,xmax,ymin+half, ymax));



		}




	}

	
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter> ().mesh; // ugh that autoformatting
		meshCollider = GetComponent<MeshCollider> ();
		
		Create();
	}

	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Space)){
			uvCount = 0;
			vertices = new List<Vector3>();
			uvs = new List<Vector2>();
			triangles = new List<int>();
			Create ();
		}

	}


	void Create () {

		if(mapSize > 128){
			mapSize = 128;
			Debug.Log("Meshes cannot be over 128x128");
		}


		float[,] h = new float[mapSize,mapSize];


//		for(int k = 16; k != 0; k /=2){
//			Fractate(h,k,k*k/16.0f);
//			Smooth (h,2,0.85f);
//		}

		DiamondSquare(h,0,mapSize-1,0,mapSize-1);
//		Smooth (h,smoothness,0.75f);
		for(int i = 0; i < smoothness; i++){
			Plateau(h,distance,0.5f);
		}
		//generate the mesh data
		for(int i = 0; i < mapSize-1; i++){
			for(int j = 0; j < mapSize-1; j++){
				AddConQuad(i-mapSize/2,j-mapSize/2, h[i,j], h[i+1,j], h[i+1,j+1], h[i,j+1]);
			}
		}

		UpdateMesh ();

	}

	void AddConQuad(float x, float z, float ya, float yb, float yc, float yd){
		vertices.Add (new Vector3 (x,	ya,	 z));
		vertices.Add (new Vector3 (x,	yd, z+1));
		vertices.Add (new Vector3 (x+1, yc, z+1));
		vertices.Add (new Vector3 (x+1, yb,	 z));

		triangles.Add (uvCount+0);
		triangles.Add (uvCount+1);
		triangles.Add (uvCount+3);
		triangles.Add (uvCount+1);
		triangles.Add (uvCount+2);
		triangles.Add (uvCount+3);
		
		uvCount += 4;
		
		float uvscale = 0.25f;
		//Vector2 tex = new Vector2 (Random.Range (0, 3), Random.Range (0, 3));
		float ave = ((ya+yb+yc+yd)/4.0f);

		Vector2 tex = new Vector2(2,1);

		if(Mathf.Abs(ave - ya) + Mathf.Abs(ave - yb) + Mathf.Abs(ave - yc) + Mathf.Abs(ave - yd) > distance * 1.5){
			tex = new Vector2(3,1);
		}

		uvs.Add (uvscale * (tex + new Vector2 (0, 1)));
		uvs.Add (uvscale * (tex + new Vector2 (1, 1)));
		
		uvs.Add (uvscale * (tex + new Vector2 (1, 0)));
		uvs.Add (uvscale * (tex + new Vector2 (0, 0)));



	}


	void UpdateMesh(){
		mesh.Clear ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.uv = uvs.ToArray ();
		mesh.Optimize ();
		mesh.RecalculateNormals ();

		Mesh colliderMesh = new Mesh ();
		colliderMesh.vertices = vertices.ToArray ();
		colliderMesh.triangles = triangles.ToArray ();
		colliderMesh.Optimize ();

		meshCollider.sharedMesh = colliderMesh;

	}

}
