using UnityEngine;
using MQTTnet.Client;
using MQTTnet;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System.Text;
using System;
using System.Linq;

public class CircuitManager : MonoBehaviour
{
    public static event Action<CircuitState> StateChange;

    public static CircuitManager instance;

    public static string k_server = "mqtt.eclipseprojects.io";
    public static string k_clientID = Guid.NewGuid().ToString();
    public static int k_port = 1883;
    public static MqttClientOptions k_mqttOptions;
    public static IMqttClient k_mqttClient;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
        Setup();

    }

    public static async void Setup()
    {
        k_mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(k_server, k_port)
            // .WithTlsOptions(new MqttClientTlsOptionsBuilder().Build())
            .WithClientId(k_clientID)
            .Build();
        await ConnectClient();
        PublishTest();
        Subscribe();

        // AND
        TruthTable and = new TruthTable(new CircuitState[] {
                new CircuitState(false, false, false, false, false, false, false, false),
                new CircuitState(true, false, false, false, false, false, false, false),
                new CircuitState(false, true, false, false, false, false, false, false),
                new CircuitState(true, true, false, false, true, false, false, false),
                });
        //SendTestResultRequest(and);
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

    public static async void Subscribe()
    {
        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        k_mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Debug.Log("Received application message.");
            string data = e.ApplicationMessage.ConvertPayloadToString();
            Debug.Log(data);
            switch(data[0])
            {
                // state change update
                case 's':
                    int stateInt = int.Parse(data.Substring(1));
                    byte state = (byte) stateInt;
                    Debug.Log(state);

                    CircuitState newState = CircuitState.FromBitmask(state);
                    StateChange?.Invoke(newState);
                    Debug.Log(newState.toString());
                    break;

                // result from circuittest
                case 'r':
                    break;
            }


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

    public static async void SendTestResultRequest(TruthTable ttable)
    {
        string payload = ttable.ToPayload();
        Debug.Log($"requesting: {payload}");
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("DDU4/DigitalLogik/testrequest")
            .WithPayload(payload)
            .Build();

        await k_mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        Debug.Log("MQTT application message is published.");
    }
}

public class CircuitState
{
    public bool in1, in2, in3, in4;
    public bool out1, out2, out3, out4;

    public CircuitState() {}
    public CircuitState(bool in1, bool in2, bool in3, bool in4, bool out1, bool out2, bool out3, bool out4)
    {
        this.in1 = in1;
        this.in2 = in2;
        this.in3 = in3;
        this.in4 = in4;

        this.out1 = out1;
        this.out2 = out2;
        this.out3 = out3;
        this.out4 = out4;
    }

    public static CircuitState FromBitmask(byte mask) {
        CircuitState state = new CircuitState();
        state.in1 = (mask & 1) != 0;
        state.in2 = ((mask >> 1) & 1) != 0;
        state.in3 = ((mask >> 2) & 1) != 0;
        state.in4 = ((mask >> 3) & 1) != 0;

        state.out1 = ((mask >> 4) & 1) != 0;
        state.out2 = ((mask >> 5) & 1) != 0;
        state.out3 = ((mask >> 6) & 1) != 0;
        state.out4 = ((mask >> 7) & 1) != 0;
        return state;
    }
    public string toString() {
        return $"{in1} {out1}\n{in2} {out2}\n{in3} {out3}\n{in4} {out4}";
    }
    public byte toPayload()
    {
        byte payload = (byte) (in1 ? 1 : 0);
        payload |= (byte) ((in2 ? 1 : 0) << 1);
        payload |= (byte) ((in3 ? 1 : 0) << 2);
        payload |= (byte) ((in4 ? 1 : 0) << 3);

        payload |= (byte) ((out1 ? 1 : 0) << 4);
        payload |= (byte) ((out2 ? 1 : 0) << 5);
        payload |= (byte) ((out3 ? 1 : 0) << 6);
        payload |= (byte) ((out4 ? 1 : 0) << 7);
        return payload;
    }
}

public class TruthTable
{
    public CircuitState[] rows;

    public TruthTable(CircuitState[] rows_) { this.rows = rows_; }

    public string ToPayload()
    {
        string payload = "";
        foreach (var row in rows)
        {
            payload += row.toPayload() + " ";
        }

        return payload;
    }
}
