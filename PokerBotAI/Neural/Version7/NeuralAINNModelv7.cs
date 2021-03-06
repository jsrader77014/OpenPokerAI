using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Encog;

namespace PokerBot.AI.Neural.Version7
{
  public class NeuralAINNModelV7 : FBPNNMain<NNDataSource>
  {
    protected const int inputNeurons = 42;
    protected const int outputNeurons = 5;

    public static int Input_Neurons
    {
      get { return inputNeurons; }
    }

    public static int Output_Neurons
    {
      get { return outputNeurons; }
    }

    public NeuralAINNModelV7()
    {
      OtherSetup();
    }

    private void OtherSetup()
    {
      INPUT_NEURONS = inputNeurons;
      OUTPUT_NEURONS = outputNeurons;
      HIDDENLAYER1_NEURONS = 30;
      HIDDENLAYER2_NEURONS = 15;

      ANNEAL_STARTTEMP = 100;
      ANNEAL_ENDTEMP = 2;
      ANNEAL_ITERATIONS = 50;
      ANNEAL_MIN_ERROR = 0.10;

      ANNEAL_ATTEMPTS = 6;
      MAX_ANNEAL_START_ERROR = 0.0001;
      MIN_BACKPROP_ITERATIONS_ANNEAL_START = 20;

      ACTIVIATION_FUNCTION = 1;

      //MAX_RMS_ITERATION_NETWORK_ERROR = 0.000002; //TANH
      MAX_RMS_ITERATION_NETWORK_ERROR = 0.0000001; //SIG
      MAX_RMS_TOTAL_NETWORK_ERROR = 0.02;

      TRAIN_DATA_PERCENT = 0.8;

      neuralNetDataSource = new List<NNDataSource>();
    }

    public long[] getBotAiLogFileData(string logFileString, int[] columnsToZero, Dictionary<long, List<byte>> actionsToIgnore = null)
    {
      string[] allLines = File.ReadAllLines(logFileString);
      long[] handIds = new long[allLines.Length];

      for (int i = 0; i < allLines.Length; i++)
      {
        //Remove the actionId from the beginning of the string
        string trimedString = allLines[i].Substring(allLines[i].IndexOf(',', allLines[i].IndexOf(',') + 1) + 1, allLines[i].Length - allLines[i].IndexOf(',', allLines[i].IndexOf(',') + 1) - 1);

        string[] lineElements = allLines[i].Split(',');

        handIds[i] = long.Parse(lineElements[0]);
        byte seqIndex = Byte.Parse(lineElements[1]);

        //If we should be ignoring this then we just loop to the next action
        if (actionsToIgnore != null)
        {
          if (actionsToIgnore.ContainsKey(handIds[i]) && actionsToIgnore[handIds[i]].Contains(seqIndex))
            continue;
        }

        //Split the string into input and output
        string[] strValues = trimedString.Split(',');
        string inputStr = "";
        string idealOutputStr = "";

        //All but the last five are inputs
        //If this column occurs in columnsToZero we want to make it 0
        for (int j = 0; j < strValues.Length - Output_Neurons; j++)
        {
          if (columnsToZero == null)
            inputStr += strValues[j] + ",";
          else
          {
            //j+1 because we have removed the actionId column here
            if (columnsToZero.Contains(j + 1))
              inputStr += "0 ,";
            else
              inputStr += strValues[j] + ",";
          }
        }

        //The last 5 are outputs
        //These are never zeroed
        for (int j = strValues.Length - Output_Neurons; j < strValues.Length; j++)
          idealOutputStr += strValues[j] + ",";

        neuralNetDataSource.Add(new NNDataSource(inputStr, idealOutputStr, Input_Neurons, Output_Neurons, true));
      }

      neuralNetDataSource = shuffleList(neuralNetDataSource);

      return handIds;
    }

    public long[] getBotAiLogFileData(string logFileString, Dictionary<long, List<byte>> actionsToIgnore = null)
    {
      return getBotAiLogFileData(logFileString, null, actionsToIgnore);
    }

    public void getBotAiNNDatabaseData(short pokerClientId)
    {
      throw new NotImplementedException();
    }

    private struct VectorDistanceElement
    {
      public double totalVectorDistance;
      public NNDataSource element;

      public VectorDistanceElement(double distance, NNDataSource element)
      {
        this.totalVectorDistance = distance;
        this.element = element;
      }
    }

    /// <summary>
    /// Using the currently loaded NNDataSource list calculates vector distance for all and then returns the closest matches
    /// </summary>
    /// <param name="dataSourceToMatch"></param>
    /// <param name="numReturns"></param>
    /// <returns></returns>
    public NNDataSource[] ReturnBestNNDataSourceMatch(NNDataSource dataSourceToMatch, int numReturns)
    {
      throw new NotImplementedException("Not yet implmented in v5. Check v4 for similar.");
    }
  }
}
