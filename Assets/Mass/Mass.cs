using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour {
    GameObject[] balls;
    int numBalls = 100;
    Vector3[] dirs;
    float[] mass;
    GameObject[] worlds;
    float massBallMin = 10f;
    float massBallMax = 30f;
    float radWorld = 50;
    float speed = 2f;
    Color[] cols;
    int numWorlds;
    GameObject parentWorld;
    int numParts; //6;
    int numAngles; //16*2;

	// Use this for initialization
	void Start () {
        initCols();
        initWorld();
        initBalls();	


	}
	
	// Update is called once per frame
	void Update () {
        updateBalls();		
	}

    void updateBalls() {
        for (int b = 0; b < numBalls; b++)
        {
            for (int w = 0; w < numWorlds; w++) 
            {
                checkNearWorld(b, w);
            }
            for (int bCheck = 0; bCheck < numBalls; bCheck++)
            {
                if (b != bCheck)
                {
                    checkNearBall(b, bCheck);
                }
            }
            balls[b].transform.position += dirs[b]; // * mass[b];
            balls[b].transform.LookAt(balls[b].transform.position + dirs[b]);
        }
    }

    void checkNearWorld(int b, int w)
    {
        if (worlds[w] == null) return;
        Vector3 posLocal = worlds[w].transform.InverseTransformPoint(balls[b].transform.position);
        float sx = balls[b].transform.localScale.x / 2;
        float sy = balls[b].transform.localScale.y / 2;
        float sz = balls[b].transform.localScale.z / 2;
        float wx = worlds[w].transform.localScale.x / 2;
        float wy = worlds[w].transform.localScale.y / 2;
        float wz = worlds[w].transform.localScale.z / 2;
        if (posLocal.y <= sy + wy && posLocal.y > 0)
        {
            if (Mathf.Abs(posLocal.x * worlds[w].transform.localScale.x) <= wx && Mathf.Abs(posLocal.z * worlds[w].transform.localScale.z) <= wz) {
                posLocal.y = sy + wy;
                balls[b].transform.position = worlds[w].transform.TransformPoint(posLocal);
                dirs[b] = Vector3.Reflect(dirs[b], worlds[w].transform.up);          
            } 
        }
    }

    void checkNearBall(int b, int bCheck)
    {
        float dist = Vector3.Distance(balls[b].transform.position, balls[bCheck].transform.position);
        float rads = balls[b].transform.localScale.x / 2 + balls[bCheck].transform.localScale.x / 2;
        if (dist < rads)
        {
            float ratio = balls[b].transform.localScale.x / (balls[b].transform.localScale.x + balls[bCheck].transform.localScale.x);
            Vector3 up = balls[bCheck].transform.position - balls[b].transform.position;
            Vector3 pos = balls[b].transform.position + (up * ratio);
            Plane pl = new Plane();
            up = Vector3.Normalize(up);
            pl.SetNormalAndPosition(up, pos);
            dirs[b] = Vector3.Reflect(dirs[bCheck], up);          
        }
    }

    void setColorDefault(int b) {
        Color col = cols[Random.Range(0, cols.Length)];
        balls[b].GetComponent<Renderer>().material.color = col;
    }

    void setColor(int b, Color col) {
        balls[b].GetComponent<Renderer>().material.color = col;
    }

    void separate(int b, int bCheck) {
        Vector3 posCenter = (balls[b].transform.position + balls[bCheck].transform.position) / 2;
        touch(b, posCenter);
        touch(bCheck, posCenter);
    }

    void touch(int b, Vector3 pos) {
        float dist = Vector3.Distance(balls[b].transform.position, pos);
        Vector3 dir = Vector3.Normalize(pos - balls[b].transform.position);
        balls[b].transform.position += dir * (dist - balls[b].transform.localScale.x / 2);        
    }

    void initCols() {
        cols = new Color[6];
        cols[0] = Color.cyan;
        cols[1] = Color.red;
        cols[2] = Color.green;
        cols[3] = Color.gray;
        cols[4] = Color.yellow;
        cols[5] = Color.magenta;
    }

    void initWorld() {
        initWorldSquare();
        return;
        //initWorldSeg();
        //return;
        int angIncr = 360 / numAngles;
        numWorlds = numAngles * numParts;
        worlds = new GameObject[numWorlds];
        Debug.Log(worlds.Length + " " + angIncr);
        parentWorld = new GameObject("parentWorld");
        float rad = radWorld * 5;
        Vector3 posCenter = new Vector3(-rad, 0, 0);
        posCenter = Vector3.zero;
        Vector3 sca = new Vector3(radWorld * 2, 1, radWorld * 2);
        int w = 0;
        for (int p = 0; p < numParts; p++)
        {
            for (int a = 0; a < numAngles; a++)
            {
                float ang = a * angIncr;
                float x = posCenter.x + rad * Mathf.Cos(ang * Mathf.Deg2Rad);
                float y = 0;
                float z = posCenter.z + rad * Mathf.Sin(ang * Mathf.Deg2Rad);
                Vector3 pos = new Vector3(x, y, z);
                Vector3 rot = new Vector3(0, -ang, 0);
                createPlane(w, pos, rot, sca);
                w++; 
            }
        }
    }

    void initWorldSquare() {
        numParts = 6;
        int numSegs = 1;
        numWorlds = numParts * numSegs;
        worlds = new GameObject[numWorlds];
        Debug.Log("square " + worlds.Length);
        parentWorld = new GameObject("parentWorld");
        Vector3 posStart = Vector3.zero;
        Vector3 posEnd = new Vector3(radWorld * 2, 0, 0);
        Vector3 posMid = (posStart + posEnd) / 2;
        //
        Vector3 sca = new Vector3(radWorld * 2, 1, radWorld * 2);
        Vector3 posBottom = posMid + Vector3.up * -1 * radWorld;
        Vector3 posTop = posMid + Vector3.up * radWorld;
        Vector3 posLeft = posStart + Vector3.right * -1 * radWorld;
        Vector3 posRight = posEnd + Vector3.right * radWorld;
        Vector3 posBack = posMid + Vector3.forward * -1 * radWorld;
        Vector3 posFront = posMid + Vector3.forward * radWorld;
        Vector3 rotBottom = new Vector3(0, 0, 0);
        Vector3 rotTop = new Vector3(0, 0, 180);
        Vector3 rotLeft = new Vector3(0, 0, -90);
        Vector3 rotRight = new Vector3(0, 0, 90);
        Vector3 rotBack = new Vector3(90, 0, 0);
        Vector3 rotFront = new Vector3(-90, 0, 0);
        Vector3 scaBottomTop = new Vector3(radWorld * 2 * 2, 1, radWorld * 2);
        Vector3 scaBack = new Vector3(radWorld * 2, 1, radWorld * 2);
        Vector3 scaFront = new Vector3(radWorld * 2 * 2, 1, radWorld * 2);
        //
        createPlane(0, posBottom, rotBottom, scaBottomTop);
        createPlane(1, posTop, rotTop, scaBottomTop);
        createPlane(2, posLeft, rotLeft, sca);
        createPlane(3, posRight, rotRight, sca);
        createPlane(4, posBack, rotBack, scaBack);
        createPlane(5, posFront, rotFront, scaFront);
     }

    void createPlane(int w, Vector3 pos, Vector3 rot, Vector3 sca) {
        worlds[w] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        worlds[w].name = "world " + w;
        worlds[w].transform.parent = parentWorld.transform;
        makeMaterialTransparent(worlds[w].GetComponent<Renderer>().material);
        worlds[w].GetComponent<Renderer>().material.color = new Color(.25f, .25f, .25f, .25f);
        worlds[w].transform.localScale = sca;
        worlds[w].transform.position = pos;
        worlds[w].transform.eulerAngles = rot;
        worlds[w].name += " " + rot.y;
    }

    void initWorldSeg() {
        worlds = new GameObject[numWorlds];
        for (int w = 0; w < numWorlds; w++) {
            worlds[w] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            makeMaterialTransparent(worlds[w].GetComponent<Renderer>().material);
            worlds[w].GetComponent<Renderer>().material.color = new Color(.25f, .25f, .25f, .25f);
            worlds[w].transform.localScale = new Vector3(radWorld * 2, 1, radWorld * 2);
            if (w == 0)
            {
                worlds[w].name = "bottom";
                worlds[w].transform.position = new Vector3(0, -1 * radWorld, 0);
                worlds[w].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (w == 1)
            {
                worlds[w].name = "top";
                worlds[w].transform.position = new Vector3(0, radWorld, 0);
                worlds[w].transform.eulerAngles = new Vector3(0, 0, 180);
            }
            //
            if (w == 2)
            {
                worlds[w].name = "left";
                worlds[w].transform.position = new Vector3(-1 * radWorld, 0, 0);
                worlds[w].transform.eulerAngles = new Vector3(0, 0, -90);
            }
            if (w == 3)
            {
                worlds[w].name = "right";
                worlds[w].transform.position = new Vector3(radWorld, 0, 0);
                worlds[w].transform.eulerAngles = new Vector3(0, 0, 90);
            }
            
            if (w == 4)
            {
                worlds[w].name = "back";
                worlds[w].transform.position = new Vector3(0, 0, -1 * radWorld);
                worlds[w].transform.eulerAngles = new Vector3(90, 0, 0);
            }
            if (w == 5)
            {
                worlds[w].name = "front";
                worlds[w].transform.position = new Vector3(0, 0, radWorld);
                worlds[w].transform.eulerAngles = new Vector3(-90, 0, 0);
            }
        }
    }

    void initBalls() {
        GameObject parentBalls = new GameObject("parentBalls");
        balls = new GameObject[numBalls];
        dirs = new Vector3[numBalls];
        mass = new float[numBalls];
        for (int b = 0; b < numBalls; b++) {
            balls[b] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            balls[b].transform.parent = parentBalls.transform;
            balls[b].name = "ball " + b;
            balls[b].transform.position = Random.insideUnitSphere * radWorld;
            mass[b] = Random.Range(massBallMin, massBallMax);
            dirs[b] = Random.insideUnitSphere * Random.Range(0, speed);
            balls[b].transform.localScale = new Vector3(mass[b], mass[b], mass[b]);
            setColorDefault(b);
        }
    }

    void makeMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
