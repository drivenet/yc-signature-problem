Demoes signature validation problem with Yandex.Cloud S3 service. To illustrate the problem it tries to create several empty files with different names.

The names that do not contain characters that need URI escaping go through fine, but the ones that do, well, look for yourself.

To run it, you will need read access to an S3 bucket in Yandex.Cloud.

Could be run with `dotnet run <access key> <secret key> <bucket-name>`, or just open the solution in Visual Studio.
