﻿using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using Interact.Library;
using System.Net;
using Interact.Instance.Exceptions;

namespace Interact.Instance.Components.Amazon
{
    public class AWSComponent
    {
        private BasicAWSCredentials _Credentials;

        public AWSComponent(string accessKey, string secretKey)
        {
            _Credentials = new BasicAWSCredentials(accessKey, secretKey);
        }

        public ICollection<Message> RetriveQueueObjects(string queueUrl, RegionEndpoint region, int visibilityTimeout, int maxNumberOfMessages, int waitTimeSeconds = 0)
        {
            if (queueUrl.NullOrEmpty())
            {
                throw new ArgumentException("Queue url is required.");
            }

            if (!visibilityTimeout.ValidRange(1, int.MaxValue))
            {
                throw new ArgumentOutOfRangeException("Visibility timeout must be a positive integer value.");
            }

            if (!waitTimeSeconds.ValidRange(0, 30))
            {
                throw new ArgumentOutOfRangeException("Wait time for wait objects is not required. But if used must be a integer in a range of 1 to 30.");
            }

            if (!maxNumberOfMessages.ValidRange(0, 10))
            {
                throw new ArgumentOutOfRangeException("Max number of messages must be a integer in a range of 1 to 10");
            }

            using (var client = new AmazonSQSClient(_Credentials, region))
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = maxNumberOfMessages,
                    VisibilityTimeout = visibilityTimeout,
                    WaitTimeSeconds = waitTimeSeconds
                };

                var response = client.ReceiveMessageAsync(receiveMessageRequest).Result;

                return response.Messages;
            }
        }

        public void RemoveQueueObjects(string queueUrl, RegionEndpoint region, string receiptHandle)
        {
            if (queueUrl.NullOrEmpty())
            {
                throw new ArgumentException("Queue url is required.");
            }

          
            using (var client = new AmazonSQSClient(_Credentials, region))
            {
                DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();

                deleteMessageRequest.QueueUrl = queueUrl;
                deleteMessageRequest.ReceiptHandle = receiptHandle;

                DeleteMessageResponse response = client.DeleteMessageAsync(deleteMessageRequest).Result;

                var acceptedCodes = new List<HttpStatusCode>
                {
                    HttpStatusCode.Accepted,
                    HttpStatusCode.OK,
                    HttpStatusCode.NoContent
                };

                if (!acceptedCodes.Contains(response.HttpStatusCode))
                {
                    throw new AWSQueueMessageException($"Error on remove the message: {response.ResponseMetadata}");
                }
            }
        }
    }
}