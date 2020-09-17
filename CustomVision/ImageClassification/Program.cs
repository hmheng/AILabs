using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageClassification
{
    class Program
    {
        private const string SouthCentralUsEndpoint = "https://southeastasia.api.cognitive.microsoft.com";
        private const string PublishedModelName = "treeClassModel";
        private const string PredictionResourceId = "/subscriptions/a0c1d805-960b-4e6f-a8a6-7cba2d0de541/resourceGroups/UCTS/providers/Microsoft.CognitiveServices/accounts/UCTS_prediction";
        private static List<string> hemlockImages;
        private static List<string> japaneseCherryImages;
        private static List<string> surfaceProImages;
        private static List<string> surfaceStudioImages;
        private static bool IsSurfaceClassification = true;
        private static MemoryStream testImage;

        static void Main(string[] args)
        {
            // Add your training & prediction key from the settings page of the portal
            string trainingKey = "16e883656e9746e0a9dc2744bc5d511a";
            string predictionKey = "3b069625032d4d6686f7de3cb62e1c4a";

            // Create the Api, passing in the training key
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = trainingKey,
                Endpoint = SouthCentralUsEndpoint
            };

            // Create a new project
            Console.WriteLine("Creating new project:");

            Project project = null;
            if (!IsSurfaceClassification)
            {
                project = trainingApi.CreateProject("Flowers");

                // Make two tags in the new project
                var hemlockTag = trainingApi.CreateTag(project.Id, "Hemlock");
                var japaneseCherryTag = trainingApi.CreateTag(project.Id, "Japanese Cherry");

                // Add some images to the tags
                Console.WriteLine("\tUploading images");
                LoadFlowerImagesFromDisk();

                // Images can be uploaded one at a time
                foreach (var image in hemlockImages)
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(image)))
                    {
                        trainingApi.CreateImagesFromData(project.Id, stream, new List<Guid>() { hemlockTag.Id });
                    }
                }

                // Or uploaded in a single batch 
                var imageFiles = japaneseCherryImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
                trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFiles, new List<Guid>() { japaneseCherryTag.Id }));
            }
            else
            {
                project = trainingApi.CreateProject("Surface");

                // Make two tags in the new project
                var surfaceProTag = trainingApi.CreateTag(project.Id, "surface-pro");
                var surfaceStudioTag = trainingApi.CreateTag(project.Id, "surface-studio");

                // Add some images to the tags
                Console.WriteLine("\tUploading images");
                LoadSurfaceImagesFromDisk();

                // Images can be uploaded one at a time
                foreach (var image in surfaceProImages)
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(image)))
                    {
                        trainingApi.CreateImagesFromData(project.Id, stream, new List<Guid>() { surfaceProTag.Id });
                    }
                }

                // Or uploaded in a single batch 
                var imageFiles = surfaceStudioImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
                trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFiles, new List<Guid>() { surfaceStudioTag.Id }));
            }

            // Now there are images with tags start training the project
            Console.WriteLine("\tTraining");
            var iteration = trainingApi.TrainProject(project.Id);

            // The returned iteration will be in progress, and can be queried periodically to see when it has completed
            while (iteration.Status == "Training")
            {
                Thread.Sleep(1000);

                // Re-query the iteration to get it's updated status
                iteration = trainingApi.GetIteration(project.Id, iteration.Id);
            }

            // The iteration is now trained. Publish it to the prediction end point.
            var publishedModelName = PublishedModelName;
            var predictionResourceId = PredictionResourceId;
            trainingApi.PublishIteration(project.Id, iteration.Id, publishedModelName, predictionResourceId);
            Console.WriteLine("Done!\n");

            // Now there is a trained endpoint, it can be used to make a prediction

            // Create a prediction endpoint, passing in obtained prediction key
            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = predictionKey,
                Endpoint = SouthCentralUsEndpoint
            };

            // Make a prediction against the new project
            Console.WriteLine("Making a prediction:");
            var result = endpoint.ClassifyImage(project.Id, publishedModelName, testImage);

            // Loop over each prediction and write out the results
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
            }
            Console.ReadKey();
        }

        private static void LoadFlowerImagesFromDisk()
        {
            // this loads the images to be uploaded from disk into memory
            hemlockImages = Directory.GetFiles(Path.Combine("Images", "Hemlock")).ToList();
            japaneseCherryImages = Directory.GetFiles(Path.Combine("Images", "Japanese Cherry")).ToList();
            testImage = new MemoryStream(File.ReadAllBytes(Path.Combine("Images", "Test\\test_image.jpg")));
        }

        private static void LoadSurfaceImagesFromDisk()
        {
            // this loads the images to be uploaded from disk into memory
            surfaceProImages = Directory.GetFiles(Path.Combine("Images", "Surface Pro")).ToList();
            surfaceStudioImages = Directory.GetFiles(Path.Combine("Images", "Surface Studio")).ToList();
            testImage = new MemoryStream(File.ReadAllBytes(Path.Combine("Images", "Test\\studio_test.jpg")));
        }
    }
}
