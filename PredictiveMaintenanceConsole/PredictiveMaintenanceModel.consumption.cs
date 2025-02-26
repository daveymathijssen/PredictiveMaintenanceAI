﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace PredictiveMaintenanceConsole
{
    public partial class PredictiveMaintenanceModel
    {
        /// <summary>
        /// model input class for PredictiveMaintenanceModel.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(1)]
            [ColumnName(@"Product ID")]
            public string Product_ID { get; set; }

            [LoadColumn(2)]
            [ColumnName(@"Type")]
            public string Type { get; set; }

            [LoadColumn(3)]
            [ColumnName(@"Air temperature")]
            public float Air_temperature { get; set; }

            [LoadColumn(4)]
            [ColumnName(@"Process temperature")]
            public float Process_temperature { get; set; }

            [LoadColumn(5)]
            [ColumnName(@"Rotational speed")]
            public float Rotational_speed { get; set; }

            [LoadColumn(6)]
            [ColumnName(@"Torque")]
            public float Torque { get; set; }

            [LoadColumn(7)]
            [ColumnName(@"Tool wear")]
            public float Tool_wear { get; set; }

            [LoadColumn(8)]
            [ColumnName(@"Machine failure")]
            public float Machine_failure { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for PredictiveMaintenanceModel.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Product ID")]
            public float[] Product_ID { get; set; }

            [ColumnName(@"Type")]
            public float[] Type { get; set; }

            [ColumnName(@"Air temperature")]
            public float Air_temperature { get; set; }

            [ColumnName(@"Process temperature")]
            public float Process_temperature { get; set; }

            [ColumnName(@"Rotational speed")]
            public float Rotational_speed { get; set; }

            [ColumnName(@"Torque")]
            public float Torque { get; set; }

            [ColumnName(@"Tool wear")]
            public float Tool_wear { get; set; }

            [ColumnName(@"Machine failure")]
            public uint Machine_failure { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"PredictedLabel")]
            public float PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float[] Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("PredictiveMaintenanceModel.mlnet");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);


        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        /// <summary>
        /// Use this method to predict scores for all possible labels.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static IOrderedEnumerable<KeyValuePair<string, float>> PredictAllLabels(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            var result = predEngine.Predict(input);
            return GetSortedScoresWithLabels(result);
        }

        /// <summary>
        /// Map the unlabeled result score array to the predicted label names.
        /// </summary>
        /// <param name="result">Prediction to get the labeled scores from.</param>
        /// <returns>Ordered list of label and score.</returns>
        /// <exception cref="Exception"></exception>
        public static IOrderedEnumerable<KeyValuePair<string, float>> GetSortedScoresWithLabels(ModelOutput result)
        {
            var unlabeledScores = result.Score;
            var labelNames = GetLabels(result);

            Dictionary<string, float> labledScores = new Dictionary<string, float>();
            for (int i = 0; i < labelNames.Count(); i++)
            {
                // Map the names to the predicted result score array
                var labelName = labelNames.ElementAt(i);
                labledScores.Add(labelName.ToString(), unlabeledScores[i]);
            }

            return labledScores.OrderByDescending(c => c.Value);
        }

        /// <summary>
        /// Get the ordered label names.
        /// </summary>
        /// <param name="result">Predicted result to get the labels from.</param>
        /// <returns>List of labels.</returns>
        /// <exception cref="Exception"></exception>
        private static IEnumerable<string> GetLabels(ModelOutput result)
        {
            var schema = PredictEngine.Value.OutputSchema;

            var labelColumn = schema.GetColumnOrNull("Machine failure");
            if (labelColumn == null)
            {
                throw new Exception("Machine failure column not found. Make sure the name searched for matches the name in the schema.");
            }

            // Key values contains an ordered array of the possible labels. This allows us to map the results to the correct label value.
            var keyNames = new VBuffer<float>();
            labelColumn.Value.GetKeyValues(ref keyNames);
            return keyNames.DenseValues().Select(x => x.ToString());
        }

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }
    }
}
