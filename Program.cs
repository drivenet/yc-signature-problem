using System;
using System.Reflection;
using System.Threading.Tasks;

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

using static System.FormattableString;

namespace YcSignatureProblem
{
    public static class Program
    {
        public async static Task<int> Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine(Invariant($"Usage: {Assembly.GetEntryAssembly().GetName().Name} <access key> <secret key> <bucket name>"));
                return -1;
            }

            var credentials = new BasicAWSCredentials(args[0], args[1]);
            var bucketName = args[2];

            var config = new AmazonS3Config
            {
                LogMetrics = true,
                ServiceURL = "https://storage.yandexcloud.net/",
            };

            var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogTo = LoggingOptions.Console;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;

            var client = new AmazonS3Client(credentials, config);
            foreach (var name in new[] { "test-object", "test object" })
            {
                await Put(client, bucketName, name);
            }

            return 0;
        }

        private static async Task Put(AmazonS3Client client, string bucketName, string objectName)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
            };

            try
            {
                (await client.GetObjectAsync(request)).Dispose();
            }
            catch (AmazonS3Exception exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine(Invariant($"Missing object \"{objectName}\"."));
                return;
            }
            catch (AmazonS3Exception exception) when (exception.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.Error.WriteLine(Invariant($"Failed to get \"{objectName}\"."));
                return;
            }

            Console.WriteLine(Invariant($"Successfully get \"{objectName}\"."));
        }
    }
}
