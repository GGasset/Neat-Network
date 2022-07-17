﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace NeatNetwork.Libraries
{
    public class NeuronConnectionsInfo
    {
        public int Length => Weights.Count;
        internal List<Point> ConnectedNeuronsPos { get; private set; }
        internal List<double> Weights { get; private set; }

        internal NeuronConnectionsInfo()
        {
            Weights = new List<double>();
            ConnectedNeuronsPos = new List<Point>();
        }

        internal NeuronConnectionsInfo(List<Point> connectedNeuronsPos, List<double> weights)
        {
            this.ConnectedNeuronsPos = connectedNeuronsPos;
            this.Weights = weights;
        }

        /// <summary>
        /// Layer 0 is input layer
        /// </summary>
        internal void AddNewConnection(int layerIndex, int neuronIndex, double minValue, double maxValue, double valueClosestTo0)
        {
            AddNewConnection(layerIndex, neuronIndex, GenerateWeight(minValue, maxValue, valueClosestTo0));
        }

        /// <summary>
        /// Layer 0 is input layer
        /// </summary>
        internal void AddNewConnection(int layerIndex, int neuronIndex, double weight)   
        {
            ConnectedNeuronsPos.Add(new Point(layerIndex, neuronIndex));
            Weights.Add(weight);
        }

        internal void AdjustToNewLayerBeingAdded(int layerInsertionIndex, bool isinsertedInPreviousLayer, int insertedLayerLength, double minWeight, double maxWeight, double weightClosestTo0)
        {
            for (int i = 0; i < ConnectedNeuronsPos.Count; i++)
            {
                ConnectedNeuronsPos[i].Offset(Convert.ToInt32(ConnectedNeuronsPos[i].X >= layerInsertionIndex), 0);
            }

            if (!isinsertedInPreviousLayer)
                return;

            for (int i = 0; i < insertedLayerLength; i++)
            {
                AddNewConnection(layerInsertionIndex, i, minWeight, maxWeight, weightClosestTo0);
            }
        }


        static int randomI = int.MinValue;

        public static double GenerateWeight(double minValue, double maxValue, double weightClosestTo0)
        {
            Random r = new Random(DateTime.Now.Millisecond + randomI);
            randomI++;

            (minValue, maxValue) = (Math.Min(minValue, maxValue), Math.Max(minValue, maxValue));

            double v;
            // set is negative to -1 or 1
            int isPositive = r.Next(0, 2);
            isPositive -= Convert.ToInt32(isPositive == 0);

            //if max value is negative convert is negative to -1
            isPositive -= 2 * Convert.ToInt32(maxValue < 0);
            //if min value is positive convert is negative to 1
            isPositive += 2 * Convert.ToInt32(minValue >= 0);

            weightClosestTo0 = Math.Abs(weightClosestTo0);

            // Set value closest to 0 to the closest value to 0 in respect with min/max value only if both values are positive or negative
            weightClosestTo0 += (minValue - weightClosestTo0) * Convert.ToInt32(minValue >= 0);
            weightClosestTo0 -= (weightClosestTo0 - maxValue) * Convert.ToInt32(maxValue < 0);

            v = weightClosestTo0 * isPositive;
            double randomness = r.NextDouble();
            // from v which equals WeightClosestTo0 move up to max value or min value depending if its negative
            v += randomness * (maxValue - weightClosestTo0) * Convert.ToInt32(isPositive == 1);
            v += randomness * (minValue + weightClosestTo0) * Convert.ToInt32(isPositive == -1);

            return v;
        }
    }
}
