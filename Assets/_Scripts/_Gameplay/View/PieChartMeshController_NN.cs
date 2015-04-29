using UnityEngine;

public class PieChartMeshController_NN : MonoBehaviour
{
    PieChartMesh mPieChart;
    float[] mData;
	public bool isActive = false;

    void Start()
    {
        mPieChart = gameObject.AddComponent<PieChartMesh>() as PieChartMesh;
        if (mPieChart != null)
        {
            mPieChart.Init(mData, 100, 0, 100, null);
			mData =  new float[]{currentChartAmount, 100-currentChartAmount, 0, 0};
            mPieChart.Draw(mData);
			chartDelta = 0.05f * 10.0f * (currentChartAmount/100.0f) * 3;
        }
    }
	
	public float currentChartAmount = 100;
	float chartDelta = 1;
    void Update()
    {
		if( isActive ){
			float delta = Time.deltaTime * chartDelta * 1.33f;
			currentChartAmount -= delta;
//			print ("delta: " + delta + ", currentChartAmount: " + currentChartAmount);
			if( currentChartAmount >= 0 ){
				mData =  new float[]{ currentChartAmount, 100-currentChartAmount, 0, 0 };
				mPieChart.Draw (mData);
			}
			else{
				isActive = false;
			}
		}

        if (Input.GetKeyDown("a"))
        {
			isActive = !isActive;
        }
    }
}
