﻿using System.Web;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace EMCR.DRR.API.Services.S3
{
    public class S3Provider : IS3Provider
    {
        protected readonly IAmazonS3 _amazonS3Client;
        private string? bucketName;

        public S3Provider(IAmazonS3 amazonS3Client, IConfiguration configuration)
        {
            _amazonS3Client = amazonS3Client;
            bucketName = configuration.GetValue<string>("S3:BucketName");
        }

        public async Task<string> HandleCommand(StorageCommand cmd, CancellationToken cancellationToken)
        {
            return cmd switch
            {
                UploadFileCommand c => await UploadStorageItem(c, cancellationToken),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<StorageQueryResults?> HandleQuery(StorageQuery query, CancellationToken cancellationToken)
        {
            return query switch
            {
                FileQuery q => await DownloadStorageItem(q.Key, q.Folder, cancellationToken),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        private async Task<string> UploadStorageItem(UploadFileCommand cmd, CancellationToken cancellationToken)
        {
            File file = cmd.File;
            var folder = cmd.Folder == null ? "" : $"{cmd.Folder}/";
            var key = $"{folder}{cmd.Key}";

            var request = new PutObjectRequest
            {
                Key = key,
                ContentType = cmd.File.ContentType,
                InputStream = new MemoryStream(file.Content),
                BucketName = bucketName,
                TagSet = GetTagSet(cmd.FileTag?.Tags ?? []),
            };
            request.Metadata.Add("contenttype", file.ContentType);
            request.Metadata.Add("filename", HttpUtility.HtmlEncode(file.FileName));
            if (file.Metadata != null)
            {
                foreach (FileMetadata md in file.Metadata)
                    request.Metadata.Add(md.Key, md.Value);
            }

            var response = await _amazonS3Client.PutObjectAsync(request, cancellationToken);
            response.EnsureSuccess();

            return cmd.Key;
        }

        private async Task<FileQueryResult> DownloadStorageItem(string key, string? folder, CancellationToken ct)
        {
            var dir = folder == null ? "" : $"{folder}/";
            var requestKey = $"{dir}{key}";

            //get object
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = requestKey,
            };
            var response = await _amazonS3Client.GetObjectAsync(request, ct);
            response.EnsureSuccess();
            using var contentStream = response.ResponseStream;
            using var ms = new MemoryStream();
            await contentStream.CopyToAsync(ms, ct);
            await contentStream.FlushAsync(ct);

            //get tagging
            var tagResponse = await _amazonS3Client.GetObjectTaggingAsync(
                new GetObjectTaggingRequest
                {
                    BucketName = bucketName,
                    Key = requestKey,
                }, ct);
            tagResponse.EnsureSuccess();

            return new FileQueryResult
            {
                Key = key,
                Folder = folder,
                File = new File
                {
                    ContentType = response.Metadata["contentType"],
                    FileName = response.Metadata["filename"],
                    Content = ms.ToArray(),
                    Metadata = GetMetadata(response.Metadata).AsEnumerable(),
                },
                FileTag = new FileTag
                {
                    Tags = GetTags(tagResponse.Tagging).AsEnumerable()
                }
            };
        }

        private static List<Amazon.S3.Model.Tag> GetTagSet(IEnumerable<Tag> tags)
            =>
            tags.Select(tag => new Amazon.S3.Model.Tag()
            {
                Key = tag.Key,
                Value = tag.Value
            }).ToList();

        private static List<FileMetadata> GetMetadata(MetadataCollection mc) =>
            mc.Keys.Where(key => key != "contentType" && key != "fileName")
                .Select(key => new FileMetadata { Key = key, Value = mc[key] })
                .ToList();

        private static List<Tag> GetTags(List<Amazon.S3.Model.Tag> tags) =>
            tags.ConvertAll(tag => new Tag { Key = tag.Key, Value = tag.Value });
    }

    internal static class S3ClientEx
    {
        public static void EnsureSuccess(this AmazonWebServiceResponse response)
        {
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new InvalidOperationException($"Operation failed with status {response.HttpStatusCode}");
        }

        public static void EnsureNoContent(this AmazonWebServiceResponse response)
        {
            if (response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                throw new InvalidOperationException($"Delete Operation failed with status {response.HttpStatusCode}");
        }
    }
}