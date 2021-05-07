using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Flashlight_PRO : MonoBehaviour 
{
	[Space(10)]
	[SerializeField()] GameObject Lights; // all light effects and spotlight
	[SerializeField()] AudioSource switch_sound; // audio of the switcher
	[SerializeField()] ParticleSystem dust_particles; // dust particles



	private Light spotlight;
	private Material ambient_light_material;
	private Color ambient_mat_color;
	private bool is_enabled = false;
	public Image Gauge_Image; //플래시의 배터리잔량을 보여줄 슬라이더를 받아온다.



	IEnumerator Battery_Gauge()
	{
        while (true)
        {
			yield return new WaitForSeconds(0.1f);
			if (GameManager.instance.Battery_Gauge <= 0) //배터리 잔량이 0이하 일경우
			{
				Lights.SetActive(false); //플래시를 끈다.
			}
			else if (GameManager.instance.Battery_Gauge > 0 && is_enabled) //배터리가 0이상이고 전원이 켜져 있을 경우
			{
				GameManager.instance.Battery_Gauge -= 0.03f; //배터리 게이지를 초당 0.3씩 줄인다.
				Lights.SetActive(true); //플래시를 킨다.
			}
			Gauge_Image.fillAmount = GameManager.instance.Battery_Gauge * 0.01f;
		}
		
	}





	// Use this for initialization
	void Start () 
	{
		// cache components
		spotlight = Lights.transform.Find ("Spotlight").GetComponent<Light> ();
		ambient_light_material = Lights.transform.Find ("ambient").GetComponent<Renderer> ().material;
		ambient_mat_color = ambient_light_material.GetColor ("_TintColor");

		StartCoroutine("Battery_Gauge");
	}
	





	/// <summary>
	/// changes the intensivity of lights from 0 to 100.
	/// call this from other scripts.
	/// </summary>
	public void Change_Intensivity(float percentage)
	{
		percentage = Mathf.Clamp (percentage, 0, 100);


		spotlight.intensity = (8 * percentage) / 100;

		ambient_light_material.SetColor ("_TintColor", new Color(ambient_mat_color.r , ambient_mat_color.g , ambient_mat_color.b , percentage/2000)); 
	}




	/// <summary>
	/// switch current state  ON / OFF.
	/// call this from other scripts.
	/// </summary>
	public void Switch()
	{

		if (switch_sound != null)
			switch_sound.Play ();
		if(GameManager.instance.Battery_Gauge > 0)
        {
			is_enabled = !is_enabled;
			Lights.SetActive(is_enabled);
		}

	
	}





	/// <summary>
	/// enables the particles.
	/// </summary>
	public void Enable_Particles(bool value)
	{
		if(dust_particles != null)
		{
			if(value)
			{
				dust_particles.gameObject.SetActive(true);
				dust_particles.Play();
			}
			else
			{
				dust_particles.Stop();
				dust_particles.gameObject.SetActive(false);
			}
		}
	}


}
