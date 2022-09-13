using System.Collections.Generic;
using UnityEngine;

public static class NormalDistribution {

    public static readonly Dictionary<float, float> ZTable = new Dictionary<float, float>();

    private static void ConstructZTable() {
        ZTable.Add(0.5000f, 0f);
        ZTable.Add(0.5040f, 0.01f);
        ZTable.Add(0.5080f, 0.02f);
        ZTable.Add(0.5120f, 0.03f);
        ZTable.Add(0.5160f, 0.04f);
        ZTable.Add(0.5199f, 0.05f);
        ZTable.Add(0.5239f, 0.06f);
        ZTable.Add(0.5279f, 0.07f);
        ZTable.Add(0.5319f, 0.08f);
        ZTable.Add(0.5359f, 0.09f);
        ZTable.Add(0.5398f, 0.1f);
        ZTable.Add(0.5438f, 0.11f);
        ZTable.Add(0.5478f, 0.12f);
        ZTable.Add(0.5517f, 0.13f);
        ZTable.Add(0.5557f, 0.14f);
        ZTable.Add(0.5596f, 0.15f);
        ZTable.Add(0.5636f, 0.16f);
        ZTable.Add(0.5675f, 0.17f);
        ZTable.Add(0.5714f, 0.18f);
        ZTable.Add(0.5753f, 0.19f);
        ZTable.Add(0.5793f, 0.2f);
        ZTable.Add(0.5832f, 0.21f);
        ZTable.Add(0.5871f, 0.22f);
        ZTable.Add(0.5910f, 0.23f);
        ZTable.Add(0.5948f, 0.24f);
        ZTable.Add(0.5987f, 0.25f);
        ZTable.Add(0.6026f, 0.26f);
        ZTable.Add(0.6064f, 0.27f);
        ZTable.Add(0.6103f, 0.28f);
        ZTable.Add(0.6141f, 0.29f);
        ZTable.Add(0.6179f, 0.3f);
        ZTable.Add(0.6217f, 0.31f);
        ZTable.Add(0.6255f, 0.32f);
        ZTable.Add(0.6293f, 0.33f);
        ZTable.Add(0.6331f, 0.34f);
        ZTable.Add(0.6368f, 0.35f);
        ZTable.Add(0.6406f, 0.36f);
        ZTable.Add(0.6443f, 0.37f);
        ZTable.Add(0.6480f, 0.38f);
        ZTable.Add(0.6517f, 0.39f);
        ZTable.Add(0.6554f, 0.4f);
        ZTable.Add(0.6591f, 0.41f);
        ZTable.Add(0.6628f, 0.42f);
        ZTable.Add(0.6664f, 0.43f);
        ZTable.Add(0.6700f, 0.44f);
        ZTable.Add(0.6736f, 0.45f);
        ZTable.Add(0.6772f, 0.46f);
        ZTable.Add(0.6808f, 0.47f);
        ZTable.Add(0.6844f, 0.48f);
        ZTable.Add(0.6879f, 0.49f);
        ZTable.Add(0.6915f, 0.5f);
        ZTable.Add(0.6950f, 0.51f);
        ZTable.Add(0.6985f, 0.52f);
        ZTable.Add(0.7019f, 0.53f);
        ZTable.Add(0.7054f, 0.54f);
        ZTable.Add(0.7088f, 0.55f);
        ZTable.Add(0.7123f, 0.56f);
        ZTable.Add(0.7157f, 0.57f);
        ZTable.Add(0.7190f, 0.58f);
        ZTable.Add(0.7224f, 0.59f);
        ZTable.Add(0.7257f, 0.6f);
        ZTable.Add(0.7291f, 0.61f);
        ZTable.Add(0.7324f, 0.62f);
        ZTable.Add(0.7357f, 0.63f);
        ZTable.Add(0.7389f, 0.64f);
        ZTable.Add(0.7422f, 0.65f);
        ZTable.Add(0.7454f, 0.66f);
        ZTable.Add(0.7486f, 0.67f);
        ZTable.Add(0.7517f, 0.68f);
        ZTable.Add(0.7549f, 0.69f);
        ZTable.Add(0.7580f, 0.7f);
        ZTable.Add(0.7611f, 0.71f);
        ZTable.Add(0.7642f, 0.72f);
        ZTable.Add(0.7673f, 0.73f);
        ZTable.Add(0.7704f, 0.74f);
        ZTable.Add(0.7734f, 0.75f);
        ZTable.Add(0.7764f, 0.76f);
        ZTable.Add(0.7794f, 0.77f);
        ZTable.Add(0.7823f, 0.78f);
        ZTable.Add(0.7852f, 0.79f);
        ZTable.Add(0.7881f, 0.8f);
        ZTable.Add(0.7910f, 0.81f);
        ZTable.Add(0.7939f, 0.82f);
        ZTable.Add(0.7967f, 0.83f);
        ZTable.Add(0.7995f, 0.84f);
        ZTable.Add(0.8023f, 0.85f);
        ZTable.Add(0.8051f, 0.86f);
        ZTable.Add(0.8078f, 0.87f);
        ZTable.Add(0.8106f, 0.88f);
        ZTable.Add(0.8133f, 0.89f);
        ZTable.Add(0.8159f, 0.9f);
        ZTable.Add(0.8186f, 0.91f);
        ZTable.Add(0.8212f, 0.92f);
        ZTable.Add(0.8238f, 0.93f);
        ZTable.Add(0.8264f, 0.94f);
        ZTable.Add(0.8289f, 0.95f);
        ZTable.Add(0.8315f, 0.96f);
        ZTable.Add(0.8340f, 0.97f);
        ZTable.Add(0.8365f, 0.98f);
        ZTable.Add(0.8389f, 0.99f);
        ZTable.Add(0.8413f, 1.0f);
        ZTable.Add(0.8438f, 1.01f);
        ZTable.Add(0.8461f, 1.02f);
        ZTable.Add(0.8485f, 1.03f);
        ZTable.Add(0.8508f, 1.04f);
        ZTable.Add(0.8531f, 1.05f);
        ZTable.Add(0.8554f, 1.06f);
        ZTable.Add(0.8577f, 1.07f);
        ZTable.Add(0.8599f, 1.08f);
        ZTable.Add(0.8621f, 1.09f);
        ZTable.Add(0.8643f, 1.1f);
        ZTable.Add(0.8665f, 1.11f);
        ZTable.Add(0.8686f, 1.12f);
        ZTable.Add(0.8708f, 1.13f);
        ZTable.Add(0.8729f, 1.14f);
        ZTable.Add(0.8749f, 1.15f);
        ZTable.Add(0.8770f, 1.16f);
        ZTable.Add(0.8790f, 1.17f);
        ZTable.Add(0.8810f, 1.18f);
        ZTable.Add(0.8830f, 1.19f);
        ZTable.Add(0.8849f, 1.2f);
        ZTable.Add(0.8869f, 1.21f);
        ZTable.Add(0.8888f, 1.22f);
        ZTable.Add(0.8907f, 1.23f);
        ZTable.Add(0.8925f, 1.24f);
        ZTable.Add(0.8944f, 1.25f);
        ZTable.Add(0.8962f, 1.26f);
        ZTable.Add(0.8980f, 1.27f);
        ZTable.Add(0.8997f, 1.28f);
        ZTable.Add(0.9015f, 1.29f);
        ZTable.Add(0.9032f, 1.3f);
        ZTable.Add(0.9049f, 1.31f);
        ZTable.Add(0.9066f, 1.32f);
        ZTable.Add(0.9082f, 1.33f);
        ZTable.Add(0.9099f, 1.34f);
        ZTable.Add(0.9115f, 1.35f);
        ZTable.Add(0.9131f, 1.36f);
        ZTable.Add(0.9147f, 1.37f);
        ZTable.Add(0.9162f, 1.38f);
        ZTable.Add(0.9177f, 1.39f);
        ZTable.Add(0.9192f, 1.4f);
        ZTable.Add(0.9207f, 1.41f);
        ZTable.Add(0.9222f, 1.42f);
        ZTable.Add(0.9236f, 1.43f);
        ZTable.Add(0.9251f, 1.44f);
        ZTable.Add(0.9265f, 1.45f);
        ZTable.Add(0.9279f, 1.46f);
        ZTable.Add(0.9292f, 1.47f);
        ZTable.Add(0.9306f, 1.48f);
        ZTable.Add(0.9319f, 1.49f);
        ZTable.Add(0.9332f, 1.5f);
        ZTable.Add(0.9345f, 1.51f);
        ZTable.Add(0.9357f, 1.52f);
        ZTable.Add(0.9370f, 1.53f);
        ZTable.Add(0.9382f, 1.54f);
        ZTable.Add(0.9394f, 1.55f);
        ZTable.Add(0.9406f, 1.56f);
        ZTable.Add(0.9418f, 1.57f);
        ZTable.Add(0.9429f, 1.58f);
        ZTable.Add(0.9441f, 1.59f);
        ZTable.Add(0.9452f, 1.6f);
        ZTable.Add(0.9463f, 1.61f);
        ZTable.Add(0.9474f, 1.62f);
        ZTable.Add(0.9484f, 1.63f);
        ZTable.Add(0.9495f, 1.64f);
        ZTable.Add(0.9505f, 1.65f);
        ZTable.Add(0.9515f, 1.66f);
        ZTable.Add(0.9525f, 1.67f);
        ZTable.Add(0.9535f, 1.68f);
        ZTable.Add(0.9545f, 1.69f);
        ZTable.Add(0.9554f, 1.7f);
        ZTable.Add(0.9564f, 1.71f);
        ZTable.Add(0.9573f, 1.72f);
        ZTable.Add(0.9582f, 1.73f);
        ZTable.Add(0.9591f, 1.74f);
        ZTable.Add(0.9599f, 1.75f);
        ZTable.Add(0.9608f, 1.76f);
        ZTable.Add(0.9616f, 1.77f);
        ZTable.Add(0.9625f, 1.78f);
        ZTable.Add(0.9633f, 1.79f);
        ZTable.Add(0.9641f, 1.8f);
        ZTable.Add(0.9649f, 1.81f);
        ZTable.Add(0.9656f, 1.82f);
        ZTable.Add(0.9664f, 1.83f);
        ZTable.Add(0.9671f, 1.84f);
        ZTable.Add(0.9678f, 1.85f);
        ZTable.Add(0.9686f, 1.86f);
        ZTable.Add(0.9693f, 1.87f);
        ZTable.Add(0.9699f, 1.88f);
        ZTable.Add(0.9706f, 1.89f);
        ZTable.Add(0.9713f, 1.9f);
        ZTable.Add(0.9719f, 1.91f);
        ZTable.Add(0.9726f, 1.92f);
        ZTable.Add(0.9732f, 1.93f);
        ZTable.Add(0.9738f, 1.94f);
        ZTable.Add(0.9744f, 1.95f);
        ZTable.Add(0.9750f, 1.96f);
        ZTable.Add(0.9756f, 1.97f);
        ZTable.Add(0.9761f, 1.98f);
        ZTable.Add(0.9767f, 1.99f);
        ZTable.Add(0.9772f, 2.0f);
        ZTable.Add(0.9778f, 2.01f);
        ZTable.Add(0.9783f, 2.02f);
        ZTable.Add(0.9788f, 2.03f);
        ZTable.Add(0.9793f, 2.04f);
        ZTable.Add(0.9798f, 2.05f);
        ZTable.Add(0.9803f, 2.06f);
        ZTable.Add(0.9808f, 2.07f);
        ZTable.Add(0.9812f, 2.08f);
        ZTable.Add(0.9817f, 2.09f);
        ZTable.Add(0.9821f, 2.1f);
        ZTable.Add(0.9826f, 2.11f);
        ZTable.Add(0.9830f, 2.12f);
        ZTable.Add(0.9834f, 2.13f);
        ZTable.Add(0.9838f, 2.14f);
        ZTable.Add(0.9842f, 2.15f);
        ZTable.Add(0.9846f, 2.16f);
        ZTable.Add(0.9850f, 2.17f);
        ZTable.Add(0.9854f, 2.18f);
        ZTable.Add(0.9857f, 2.19f);
        ZTable.Add(0.9861f, 2.2f);
        ZTable.Add(0.9864f, 2.21f);
        ZTable.Add(0.9868f, 2.22f);
        ZTable.Add(0.9871f, 2.23f);
        ZTable.Add(0.9875f, 2.24f);
        ZTable.Add(0.9878f, 2.25f);
        ZTable.Add(0.9881f, 2.26f);
        ZTable.Add(0.9884f, 2.27f);
        ZTable.Add(0.9887f, 2.28f);
        ZTable.Add(0.9890f, 2.29f);
        ZTable.Add(0.9893f, 2.3f);
        ZTable.Add(0.9896f, 2.31f);
        ZTable.Add(0.9898f, 2.32f);
        ZTable.Add(0.9901f, 2.33f);
        ZTable.Add(0.9904f, 2.34f);
        ZTable.Add(0.9906f, 2.35f);
        ZTable.Add(0.9909f, 2.36f);
        ZTable.Add(0.9911f, 2.37f);
        ZTable.Add(0.9913f, 2.38f);
        ZTable.Add(0.9916f, 2.39f);
        ZTable.Add(0.9918f, 2.4f);
        ZTable.Add(0.9920f, 2.41f);
        ZTable.Add(0.9922f, 2.42f);
        ZTable.Add(0.9925f, 2.43f);
        ZTable.Add(0.9927f, 2.44f);
        ZTable.Add(0.9929f, 2.45f);
        ZTable.Add(0.9931f, 2.46f);
        ZTable.Add(0.9932f, 2.47f);
        ZTable.Add(0.9934f, 2.48f);
        ZTable.Add(0.9936f, 2.49f);
        ZTable.Add(0.9938f, 2.5f);
        ZTable.Add(0.9940f, 2.51f);
        ZTable.Add(0.9941f, 2.52f);
        ZTable.Add(0.9943f, 2.53f);
        ZTable.Add(0.9945f, 2.54f);
        ZTable.Add(0.9946f, 2.55f);
        ZTable.Add(0.9948f, 2.56f);
        ZTable.Add(0.9949f, 2.57f);
        ZTable.Add(0.9951f, 2.58f);
        ZTable.Add(0.9952f, 2.59f);
        ZTable.Add(0.9953f, 2.6f);
        ZTable.Add(0.9955f, 2.61f);
        ZTable.Add(0.9956f, 2.62f);
        ZTable.Add(0.9958f, 2.63f);
        ZTable.Add(0.9959f, 2.64f);
        ZTable.Add(0.9960f, 2.65f);
        ZTable.Add(0.9961f, 2.66f);
        ZTable.Add(0.9962f, 2.67f);
        ZTable.Add(0.9963f, 2.68f);
        ZTable.Add(0.9964f, 2.69f);
        ZTable.Add(0.9965f, 2.70f);
        ZTable.Add(0.9966f, 2.71f);
        ZTable.Add(0.9967f, 2.72f);
        ZTable.Add(0.9968f, 2.73f);
        ZTable.Add(0.9969f, 2.74f);
        ZTable.Add(0.9970f, 2.75f);
        ZTable.Add(0.9971f, 2.76f);
        ZTable.Add(0.9972f, 2.77f);
        ZTable.Add(0.9973f, 2.78f);
        ZTable.Add(0.9974f, 2.79f);
        ZTable.Add(0.9975f, 2.81f);
        ZTable.Add(0.9976f, 2.82f);
        ZTable.Add(0.9977f, 2.84f);
        ZTable.Add(0.9978f, 2.85f);
        ZTable.Add(0.9979f, 2.87f);
        ZTable.Add(0.9980f, 2.88f);
        ZTable.Add(0.9981f, 2.90f);
        ZTable.Add(0.9982f, 2.92f);
        ZTable.Add(0.9983f, 2.93f);
        ZTable.Add(0.9984f, 2.95f);
        ZTable.Add(0.9985f, 2.97f);
        ZTable.Add(0.9986f, 2.99f);
        ZTable.Add(0.9987f, 3.02f);
        ZTable.Add(0.9988f, 3.04f);
        ZTable.Add(0.9989f, 3.07f);
        ZTable.Add(0.9990f, 3.1f);
        ZTable.Add(0.9991f, 3.13f);
        ZTable.Add(0.9992f, 3.17f);
        ZTable.Add(0.9993f, 3.21f);
        ZTable.Add(0.9994f, 3.26f);
        ZTable.Add(0.9995f, 3.32f);
        ZTable.Add(0.9996f, 3.38f);
        ZTable.Add(0.9997f, 3.48f);
        ZTable.Add(0.9998f, 3.54f);
        ZTable.Add(0.9999f, 3.72f);
        ZTable.Add(1f, 3.89f);
    }


    public static float RandomOverNormalDistribution(float mean, float standardDeviation) {
        //If the ZTable has not been created, create it
        if (ZTable.Count == 0) {
            ConstructZTable();
        }

        //converts the random int to a decimal and checks if it is greater or less than .5
        decimal p = Random.Range(0, 10001) / 10000m;
        bool smallP = p < .5m;

        //makes p a value from .5 - 1
        p = smallP ? 1 - p : p;

        //start increasing p until that p is a value in our ZTable
        int failsafeCounter = 0;
        while (!ZTable.ContainsKey((float)p) && p < 1) {
            p += .0001m;

            if (failsafeCounter > 50) {
                p = 0.5m;
                break;
            }

            failsafeCounter++;
        }

        //use the ZTable to output an appropriate z
        float z;

        if (smallP) {
            ZTable.TryGetValue((float)p, out z);
            z = -z;
        } else {
            ZTable.TryGetValue((float)p, out z);
        }

        //uses z-score formula to output a value using the appropriate mean and standard deviation
        return mean + z * standardDeviation;
    }

}
