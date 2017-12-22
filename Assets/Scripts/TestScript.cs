using System.Collections;
using System.Collections.Generic;
using TasiYokan.Audio;
using TasiYokan.EventsInUnity;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        NotificationCenter.Instance.AddEventListener<BaseEvent>("Ah", (_sender, _evt) =>
        {
            int a = 1;
            print("Ohhhhh");
        });
        print("Ah");
        NotificationCenter.Instance.DispatchEvent("Ah", new BaseEvent());

        float b = 2.2f;
        b.Sgn();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
