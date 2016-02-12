using System;
using System.Configuration;
using System.IO;
using System.Web;
using hMailServer.Core.Protocols.SMTP;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace hMailServer.Smtp.AzureBlobStorage
{
    internal class AzureBlobStorageSmtpServerCommandHandler : ISmtpServerCommandHandler
    {
        private string _fromAddress;

        private readonly CloudStorageAccount _storageAccount;

        public AzureBlobStorageSmtpServerCommandHandler()
        {
            var connectionString = ConfigurationManager.AppSettings["MailStorageConnectionString"];
            _storageAccount = CloudStorageAccount.Parse(connectionString);

        }

        public SmtpCommandResult HandleRset()
        {
            return new SmtpCommandResult(250, "Hello.");
        }

        public SmtpCommandResult HandleHelo(string hostName)
        {
            return new SmtpCommandResult(250, "Ok.");
        }
        
        public SmtpCommandResult HandleEhlo(string hostName)
        {
            return new SmtpCommandResult(250, "Ok.");
        }

        public SmtpCommandResult HandleMailFrom(string fromAddress)
        {
            _fromAddress = fromAddress;
            return new SmtpCommandResult(250, "Ok.");
        }

        public SmtpCommandResult HandleRcptTo(string recipientAddress)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var recipientsContainer = blobClient.GetContainerReference("recipients");

            var recipientBlob = recipientsContainer.GetBlockBlobReference(recipientAddress);

            if (recipientBlob.Exists())
                return new SmtpCommandResult(250, "Ok.");
            else
                return new SmtpCommandResult(550, "No such user");
        }

        public SmtpCommandResult HandleData(MemoryStream stream)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var mailContainer = blobClient.GetContainerReference("mail");
            var mailBlobName = $"{Guid.NewGuid()}.eml";
            var mailBlob = mailContainer.GetBlockBlobReference(mailBlobName);

            mailBlob.UploadFromStream(stream);
            return new SmtpCommandResult(250, "Ok.");
        }

    }
}