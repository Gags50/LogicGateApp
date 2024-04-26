using UnityEngine;
using MQTTnet.Client;
using MQTTnet;
using System.Threading;

public class CircuitManager : MonoBehaviour
{
    public static string k_server = "mqtt.nextservices.dk";
    public static string k_clientID = "ddu4-digitallogik-unity";
    public static int k_port = 8883;

    private void Start()
    {
        PublishTest();
    }

    public static async void PublishTest()
    {
        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(k_server, k_port)
                .WithTlsOptions(new MqttClientTlsOptionsBuilder().Build())
                .WithClientId(k_clientID)
                .Build();
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("DDU4/DigitalLogik/unitytest")
                .WithPayload("wokrs?")
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            await mqttClient.DisconnectAsync();
            Debug.Log("MQTT application message is published.");
        }
    }
}
