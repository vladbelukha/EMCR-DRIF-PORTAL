﻿using System.Text;
using EMCR.DRR.API.Services.S3;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using File = EMCR.DRR.API.Services.S3.File;

namespace EMCR.Tests.Integration.DRR.Api.S3Storage
{
    public class S3StorageTests
    {
        private string TestPrefix = "autotest-dev";

        [Test]
        public async Task CanUploadFileToS3()
        {
            var host = Application.Host;
            var storageProvider = host.Services.GetRequiredService<IS3Provider>();

            var body = DateTime.Now.ToString();
            var fileName = "test.txt";
            byte[] bytes = Encoding.ASCII.GetBytes(body);
            var file = new File { FileName = fileName, Content = bytes, ContentType = "text/plain", };

            var ct = new CancellationTokenSource().Token;
            var ret = await storageProvider.HandleCommand(new UploadFileCommand { Folder = "autotest-dev", Key = fileName, File = file }, ct);
            ret.ShouldBe(fileName);

            var uploadedFile = await storageProvider.HandleQuery(new FileQuery { Key = fileName, Folder = "autotest-dev" }, ct);
            uploadedFile.ShouldNotBeNull().ShouldBeOfType<FileQueryResult>();
        }
    }
}