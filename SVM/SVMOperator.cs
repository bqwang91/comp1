using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp.Helpers;
using LibSVMsharp.Extensions;
using LibSVMsharp;

namespace StockSenti
{
    class SVMOperator
    {
        public void generateTrainingOrTestingSet(List<Article> articleList, string FileDest)
        {
            
            Article[] articles = articleList.ToArray();
            List<string> documents = new List<string>();

            foreach (Article a in articleList)
            {
                documents.Add(a.Content);
            }

            if (documents != null)
            {
                //List<string> tokens = dataProc.dataTokenisingAndPreprocessing(test[0]);
                double[][] inputs = TFIDF.Transform(documents.ToArray(), 0);
                inputs = TFIDF.Normalize(inputs);

                int documentLength = 0;
                int indexLength = 0;
                // Display the output.
                string output = "";
                for (int index = 0; index < inputs.Length; index++)
                {
                    if (articles[index].Classification == "good")
                    {
                        output = output + "1 ";
                    }
                    else if (articles[index].Classification == "bad")
                    {
                        output = output + "2 ";
                    }
                    else
                    {
                        continue;
                    }

                    documentLength = documents[index].Length;
                    indexLength = inputs[index].Length;
                    //Console.WriteLine(documents[index]);

                    int indexCount = 0;
                    foreach (double value in inputs[index])
                    {
                        indexCount++;
                        output = output + indexCount.ToString() + ":" + value + " ";
                    }
                    output = output + "\n";
                }
                System.IO.File.WriteAllText(FileDest, output);
            }
        }

        public void trainSVM(int foldNo, double gamma, string trainingSetFile, string trainedFileDestination)
        {
            // Load the datasets: In this example I use the same datasets for training and testing which is not suggested
            SVMProblem trainingSet = SVMProblemHelper.Load(trainingSetFile);

            // Select the parameter set
            SVMParameter parameter = new SVMParameter();
            parameter.Type = SVMType.C_SVC;
            parameter.Kernel = SVMKernelType.RBF;
            parameter.C = 1;
            parameter.Gamma = gamma;

            // Do cross validation to check this parameter set is correct for the dataset or not
            double[] crossValidationResults; // output labels
            int nFold = foldNo;
            trainingSet.CrossValidation(parameter, nFold, out crossValidationResults);

            // Evaluate the cross validation result
            // If it is not good enough, select the parameter set again
            double crossValidationAccuracy = trainingSet.EvaluateClassificationProblem(crossValidationResults);

            // Train the model, If your parameter set gives good result on cross validation
            SVMModel model = trainingSet.Train(parameter);

            // Save the model
            SVM.SaveModel(model, trainedFileDestination);
            
            // Print the resutls
            Console.WriteLine("\n\nCross validation accuracy: " + crossValidationAccuracy);
        }

        public void testSVM(SVMModel model, string testSetFile)
        {
            SVMProblem testSet = SVMProblemHelper.Load(testSetFile);

            // Predict the instances in the test set
            double[] testResults = testSet.Predict(model);

            // Evaluate the test results
            int[,] confusionMatrix;
            double testAccuracy = testSet.EvaluateClassificationProblem(testResults, model.Labels, out confusionMatrix);

            Console.WriteLine("\nTest accuracy: " + testAccuracy);
            Console.WriteLine("\nConfusion matrix:\n");

            // Print formatted confusion matrix
            Console.Write(String.Format("{0,6}", ""));
            for (int i = 0; i < model.Labels.Length; i++)
                Console.Write(String.Format("{0,5}", "(" + model.Labels[i] + ")"));
            Console.WriteLine();
            for (int i = 0; i < confusionMatrix.GetLength(0); i++)
            {
                Console.Write(String.Format("{0,5}", "(" + model.Labels[i] + ")"));
                for (int j = 0; j < confusionMatrix.GetLength(1); j++)
                    Console.Write(String.Format("{0,5}", confusionMatrix[i, j]));
                Console.WriteLine();
            }

        }






        
            
    }
}
