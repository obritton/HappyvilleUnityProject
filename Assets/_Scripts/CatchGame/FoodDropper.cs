using UnityEngine;
using System.Collections;
using Spine;

public class FoodDropper : MonoBehaviour {

	public GameObject foodPrefab;
	public string[] fruits;
	public string[] candies;

	public Transform[] dropSpots;
	public FruitKiller fruitKiller;

	void Start(){
		Physics.gravity = Vector3.down * 200;
	}

	public enum DropperMode{
		DroppingFood, DroppingCandyDiagonal, DroppingCandyStreaks, Inactive
	};

	public DropperMode dropperMode = DropperMode.Inactive;

	public void startFruitDrops(){
		dropperMode = DropperMode.DroppingFood;
		StartCoroutine (loopFruitDrops());
	}

	public void startFrenzyMode(){
		dropperMode = Random.value < 0.5f ? DropperMode.DroppingCandyDiagonal : DropperMode.DroppingCandyStreaks;

		if (dropperMode == DropperMode.DroppingCandyDiagonal)
			StartCoroutine(startDiagonalCandydrops());
		else if(dropperMode == DropperMode.DroppingCandyStreaks)
		     StartCoroutine(startStreaksCandydrops());
	}
		
	public GameObject diagonalDropper;
	public IEnumerator startDiagonalCandydrops(){
		animateDropperToAnotherPos ();
		while (dropperMode == DropperMode.DroppingCandyDiagonal) {
			GameObject food = createRandomFood(true);
			food.transform.position = diagonalDropper.transform.position;
			yield return new WaitForSeconds (0.4f);
		}
	}

	int currentDiagonalDropperSpot = 0;
	public void animateDropperToAnotherPos()
	{
		if (dropperMode != DropperMode.DroppingCandyDiagonal)	return;
		int lastSpot = currentDiagonalDropperSpot;
		while( lastSpot == currentDiagonalDropperSpot)
			currentDiagonalDropperSpot = Random.Range (0, dropSpots.Length);

		iTween.MoveTo (diagonalDropper, iTween.Hash ("time", 1, "easetype", iTween.EaseType.linear, "position", dropSpots [currentDiagonalDropperSpot].position,
		                                             "oncomplete", "animateDropperToAnotherPos", "oncompletetarget", gameObject));
	}

	public IEnumerator startStreaksCandydrops(){
		dropperMode = DropperMode.DroppingCandyStreaks;

		int candyIndex = 0;
		int spotIndex = 0;
		while (dropperMode == DropperMode.DroppingCandyStreaks) {
			if( candyIndex % 3 == 0 )
			{
				int lastSpotIndex = spotIndex;
				while( spotIndex == lastSpotIndex ){
					spotIndex = Random.Range (0, dropSpots.Length);
				}
				yield return new WaitForSeconds (0.5f);
			}
			candyIndex++;
			GameObject food = createRandomFood(true);
			Vector3 position = dropSpots [spotIndex].transform.position;
			food.transform.position = position;

			yield return new WaitForSeconds (0.35f);
		}
	}

	public IEnumerator loopFruitDrops(){
		while (dropperMode == DropperMode.DroppingFood) {
//			yield return new WaitForSeconds (Random.Range(0.4f, 1.4f));
			yield return new WaitForSeconds (1);
			dropRandomFood();
		}
	}

	void dropRandomFood(){
		GameObject food = createRandomFood (false);
		int spotIndex = Random.Range (0, dropSpots.Length);
		Vector3 position = dropSpots [spotIndex].transform.position;
		food.transform.position = position;
//		food.transform.localScale *= 2;
	}

	GameObject createRandomFood( bool forceCandy ){
		bool isCandy = (Random.value <= 0.2f) || forceCandy;
		
		int foodIndex = Random.Range (0, isCandy? candies.Length : fruits.Length);
		string foodSkinName = isCandy? candies[foodIndex] : fruits [foodIndex];
		
		GameObject food = Instantiate (foodPrefab, Vector3.up * 1000, Quaternion.identity) as GameObject;
		fruitKiller.addLiveFruit (food);
		food.name = foodSkinName;
		food.tag = isCandy ? "candy" : "food";
		Vector3 scale = food.transform.localScale;
//		scale *= 0.58f;
		food.transform.localScale = scale;
		SkeletonAnimation skelAnim = ((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ());
		skelAnim.skeleton.SetSkin (foodSkinName);

		return food;
	}
}
