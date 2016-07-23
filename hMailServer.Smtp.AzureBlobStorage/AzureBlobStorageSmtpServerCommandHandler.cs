using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
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

        public Task<SmtpCommandReply> HandleRset()
        {
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleHelo(string hostName)
        {
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }
        
        public Task<SmtpCommandReply> HandleEhlo(string hostName)
        {
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleMailFrom(string fromAddress)
        {
            _fromAddress = fromAddress;
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleRcptTo(string recipientAddress)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var recipientsContainer = blobClient.GetContainerReference("recipients");

            var recipientBlob = recipientsContainer.GetBlockBlobReference(recipientAddress);

            if (recipientBlob.Exists())
                return SmtpCommandReply.CreateDefault250SuccessTask();
            else
                return Task.Run(() => new SmtpCommandReply(550, "No suchu ser."));
        }

        public Task<SmtpCommandReply> HandleData(Stream stream)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var mailContainer = blobClient.GetContainerReference("mail");
            var mailBlobName = $"{Guid.NewGuid()}.eml";
            var mailBlob = mailContainer.GetBlockBlobReference(mailBlobName);

            mailBlob.UploadFromStream(stream);

            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

    }
}