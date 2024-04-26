using UnityEngine;
using MQTTnet.Client;
using MQTTnet;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Packets;
using MQTTnet.Protocol;

public class CircuitManager : MonoBehaviour
{
    public static string k_server = "mqtt.nextservices.dk";
    public static string k_clientID = "ddu4-digitallogik-unity";
    public static int k_port = 8883;
    public static MqttClientOptions k_mqttOptions;
    public static IMqttClient k_mqttClient;

    private void Start()
    {
        k_mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(k_server, k_port)
            .WithTlsOptions(new MqttClientTlsOptionsBuilder().Build())
            .WithClientId(k_clientID)
            .Build();
        Setup();
    }

    public static async void Setup()
    {
        await ConnectClient();
        PublishTest();
        SubscribeToStatusChange();
    }

    public static async Task ConnectClient()
    {
        var mqttFactory = new MqttFactory();
        k_mqttClient = mqttFactory.CreateMqttClient();

        await k_mqttClient.ConnectAsync(k_mqttOptions, CancellationToken.None);

        // Register reconnector
        k_mqttClient.DisconnectedAsync += async e =>
        {
            if (e.ClientWasConnected)
            {
                // Use the current options as the new options.
                await k_mqttClient.ConnectAsync(k_mqttClient.Options);
            }
        };
    }

    public static async void PublishTest()
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("DDU4/DigitalLogik/unity_online")
            .WithPayload("1")
            .Build();

        await k_mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        Debug.Log("MQTT application message is published.");
    }

    public static async void SubscribeToStatusChange()
    {
        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        k_mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Debug.Log("Received application message.");
            Debug.Log(e.ApplicationMessage.ConvertPayloadToString());

            return Task.CompletedTask;
        };

        var mqttSubscribeOptions = new MqttFactory().CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f =>
                {
                    f.WithTopic("DDU4/DigitalLogik/state");
                })
            .Build();

        await k_mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        Debug.Log("MQTT client subscribed to topic.");
    }
}
